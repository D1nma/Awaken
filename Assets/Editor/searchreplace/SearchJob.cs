#define PSR_FULL
#if DEMO
#undef PSR_FULL
#endif
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;

namespace sr
{
  /**
   * When a user executes a search, a SearchJob is generated. Execute() is called
   * and SearchSubJobs are created based on the type of data being searched.
   * SearchJob keeps track of searched assets to stop searches from accidentally
   * being executed multiple times (possibly due to Dependencies being checked).
   * This also contains a number of utility functions for subjobs and search items
   * to call during execution. Things such as recursively searching objects or
   * searching more complicated objects like AnimatorControllers.
   */
  public class SearchJob
  {
    //What we are searching for.
    public SearchItemSet search;
    // How we want to search for it. This is always a copy.
    public SearchOptions options;
    // The results of the search.
    public SearchResultSet result;
    // Where we should search.
    public SearchScopeData scope;

    //Our currently searched asset.
    public UnityEngine.Object asset;

    // The data model for our currently searched asset.
    public SearchAssetData assetData;

    public Dictionary<string, SearchAssetData> searchAssetsData = new Dictionary<string, SearchAssetData>();

    SceneSubJob sceneSubJob;

    //Progress watching
    int totalItems;
    int currentItem;
    int maxItemsAllowed = 10000;
    bool shownMaxItemsWarning = false;

    StringBuilder logBuilder = new StringBuilder();

    // A list of our subjobs.
    List<SearchSubJob> subjobs = new List<SearchSubJob>();
    public HashSet<string> supportedFileTypes = new HashSet<string>();

    public SearchJob(SearchItemSet s, SearchOptions o, SearchScopeData sc)
    {
      search = s;
      options = o;
      scope = sc;
      Execute();
    }

    public void Log(string message)
    {
      logBuilder.Append(message);
      logBuilder.Append("\n");
    }

    void addIgnorableResources()
    {
      addIgnorableResource("Library/unity default resources");
      addIgnorableResource("Resources/unity_builtin_extra");
    }

    void addIgnorableResource(string path)
    {
      SearchAssetData data = new SearchAssetData(path);
      data.hasBeenSearched = true;
      searchAssetsData[path] = data;
    }

    public void Execute()
    {

      addIgnorableResources();
      string[] assetPaths = AssetDatabase.GetAllAssetPaths();
      AnimationMode.StopAnimationMode(); // If recording is on, it will hose the animation.

      subjobs.Add( new SearchSubJob(this, AssetScope.Prefabs, assetPaths, new string[]{".prefab"}, false));

      //scenes are oh so 'special'.
      sceneSubJob = new SceneSubJob(this, AssetScope.Scenes, assetPaths, new string[]{".unity"});
      subjobs.Add(sceneSubJob);
      subjobs.Add( new SearchSubJob(this, AssetScope.Materials, assetPaths, new string[]{".mat"}, false));
      subjobs.Add( new SearchSubJob(this, AssetScope.ScriptableObjects, assetPaths, new string[]{".asset"}, false));
      subjobs.Add( new SearchSubJob(this, AssetScope.Animations, assetPaths, new string[]{".anim"}, false));
      subjobs.Add( new SearchSubJob(this, AssetScope.Animators, assetPaths, new string[]{".controller"}, false));
      subjobs.Add( new SearchSubJob(this, AssetScope.AudioClips, assetPaths, new string[]{".wav", ".mp3"}, true));
      subjobs.Add( new SearchSubJob(this, AssetScope.Textures, assetPaths, new string[]{".png", ".psd", ".tiff", ".tif",".tga",".gif",".bmp",".jpg",".jpeg",".iff",".pict"}, true));
      subjobs.Add( new SceneObjectSubJob(this, AssetScope.None, assetPaths, new string[]{""}));

      foreach(SearchSubJob subjob in subjobs)
      {
        totalItems += subjob.assetPaths.Count;
      }
      search.OnSearchBegin();
      result = new SearchResultSet(search);
      result.status = SearchStatus.InProgress;

      currentItem = 1;

      if(sceneSubJob.assetPaths.Count > 0 && scope.projectScope != ProjectScope.SceneView)
      {
        bool shouldContinue = SceneUtil.SaveDirtyScenes();
        if(!shouldContinue)
        {
          userAbortedSearch();
          return;
        }
      }
      bool cancelled = false;
      try{

        foreach(SearchSubJob subjob in subjobs)
        {
          
          while(!cancelled && subjob.SearchNextAsset() )
          {
            cancelled = progress();

            if(result.resultsCount > maxItemsAllowed && !shownMaxItemsWarning)
            {
              shownMaxItemsWarning = true;
              bool userContinues = EditorUtility.DisplayDialog("Too Many Results", "The search and replace plugin has found "+ result.resultsCount+" results so far. Do you want to continue searching?", "Continue", "Cancel");
              if(!userContinues)
              {
                cancelled = true;
              }
            }
            currentItem++;
          }
        }

      }catch(System.Exception ex){
        Debug.LogException(ex);
        result.status = SearchStatus.InProgress;
        result.statusMsg = "An exception occurred: "+ex.ToString();
        EditorUtility.ClearProgressBar();
      }

      EditorUtility.ClearProgressBar();

      foreach(SearchSubJob subjob in subjobs)
      {
        subjob.JobDone();
      }

      search.OnSearchEnd(this);

      if(cancelled)
      {
        userAbortedSearch();
      }

      //calculate searchedItems
      int searchedItems = 0;
      foreach(var kvp in searchAssetsData)
      {
        SearchAssetData data = kvp.Value;
        if(data.searchExecuted)
        {
          searchedItems++;
        }
      }
      if(result.status == SearchStatus.InProgress)
      {
        //standard termination.
        result.searchedItems = searchedItems;
        result.status = SearchStatus.Complete;
      }
      if(options.searchType == SearchType.SearchAndReplace && result.resultsCount > 0)
      {
        AssetDatabase.SaveAssets();
      }
      result.OnSearchComplete();

      string log = logBuilder.ToString();
      if(log.Length > 0)
      {
        Debug.Log("[Search & Replace] Log:\n"+log);
      }
    }

    void userAbortedSearch()
    {
      result.status = SearchStatus.UserAborted;
      result.statusMsg = "User cancelled search.";
    }


    bool progress()
    {
      if(totalItems > 0)
      {
        string assetName = asset != null ? asset.name : "";
        return EditorUtility.DisplayCancelableProgressBar("Progress", "Searching "+assetName+" "+currentItem+"/"+totalItems, (float)currentItem / (float) totalItems);
      }
      return false;
    }

    //Callback while searching.
    public void MatchFound(SearchResult r, SearchItem s)
    {
      if(r.actionTaken != SearchAction.Ignored)
      {
        result.Add(r, s);
      }
    }

    public void Draw()
    {
      GUILayout.BeginVertical();

      result.Draw();

      GUILayout.EndVertical();
      if(result.status == SearchStatus.Complete)
      {
        GUILayout.BeginHorizontal();
#if PSR_FULL
        if(GUILayout.Button("Copy To Clipboard"))
        {
          result.CopyToClipboard();
        }
        if(GUILayout.Button("Select Objects"))
        {
          result.SelectAll();
        }
#else
        GUILayout.FlexibleSpace();
#endif
        SRWindow.Instance.drawAbout();

        GUILayout.EndHorizontal();
      }else{
        GUILayout.FlexibleSpace();
        
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        SRWindow.Instance.drawAbout();
 
        GUILayout.EndHorizontal();

      }
  }

    // SUBJOB HELPER FUNCTIONS.

    public void searchObject(UnityEngine.Object obj)
    {
      if(obj == null)
      {
        // Debug.Log("[SearchJob] obj is null::"+assetData.assetPath);
        return;
      }else{
        // Debug.Log("[SearchJob] searching:"+PathInfo.GetPathInfo(obj).FullPath() , obj);
      }
      SerializedObject so = new SerializedObject(obj);
      SerializedProperty iterator = so.GetIterator();
      search.SearchProperty(this, iterator);
      search.SearchObject(this, obj);
    }

    public void searchGameObject(GameObject go)
    {
      // search the game object itself.
      searchObject(go);
      if(go == null)
      {
        return;
      }
      Component[] components = go.GetComponents<Component>();
      search.SearchGameObject(this, go);
      foreach(Component c in components)
      {
        if(c != null)
        {
          SerializedObject obj = new SerializedObject(c);
          SerializedProperty iterator = obj.GetIterator();
          search.SearchProperty(this, iterator);
        }else{
          // Debug.Log("[SearchJob] NULLLLLLLL on go "+go, go);
        }
      }
      if(go != null)
      {
        IEnumerator children = go.transform.GetEnumerator();
        while(children.MoveNext())
        {
          Transform child =(Transform)children.Current;
          if(child != null)
          {
            searchGameObject(child.gameObject);
          }else{
            break;
          }
        }
      }
    }

    List<string> internalPath = new List<string>();

    public void searchAnimatorController(AnimatorController ac)
    {
      SerializedObject so = new SerializedObject(ac);
      SerializedProperty layers = so.FindProperty("m_AnimatorLayers");

      if(layers != null)
      {
        layers.Next(true);
        layers.Next(true);
        int count = layers.intValue;
        for(int i=0;i < count; i++)
        {
          layers.Next(false);
          // Debug.Log("[SearchJob] found:"+layers.propertyPath);

          search.SearchProperty(this, layers.Copy());

          //grab the state machine from this layer.
          SerializedProperty stateMachine = layers.FindPropertyRelative("m_StateMachine");
          UnityEngine.Object stateMachineObj = stateMachine.objectReferenceValue;
          // Debug.Log("[SearchJob] stateMachineObj:"+stateMachineObj);
          pushPath(stateMachineObj);
          SerializedObject stateMachineSO = new SerializedObject(stateMachineObj);
          searchObject(stateMachineObj);
          searchArray(stateMachineSO.FindProperty("m_AnyStateTransitions"));
          List<SerializedObject> states = searchArrayRelative(stateMachineSO.FindProperty("m_ChildStates"), "m_State");
          foreach(SerializedObject stateSO in states)
          {
            pushPath(stateSO.targetObject);
            searchArray(stateSO.FindProperty("m_Transitions"));
            popPath();
          }
          popPath();
        }
      }
    }

    List<SerializedObject> searchArrayRelative(SerializedProperty arrayProp, string relativeName)
    {
      List<SerializedObject> retVal = new List<SerializedObject>();
      arrayProp.Next(true);
      arrayProp.Next(true);
      int count = arrayProp.intValue;
      for(int i=0; i < count; i++)
      {
        arrayProp.Next(false);
        SerializedProperty item = arrayProp.FindPropertyRelative(relativeName);
        pushPath(item.objectReferenceValue);
        searchObject(item.objectReferenceValue);
        retVal.Add(new SerializedObject(item.objectReferenceValue));
        popPath();

      }
      return retVal;
    }

    void searchArray(SerializedProperty arrayProp)
    {
      arrayProp.Next(true);
      arrayProp.Next(true);
      int count = arrayProp.intValue;
      for(int i=0; i < count; i++)
      {
        arrayProp.Next(false);
        pushPath(arrayProp.objectReferenceValue);
        searchObject(arrayProp.objectReferenceValue);
        popPath();
      }
    }

    public void pushPath(UnityEngine.Object obj)
    {
      if(obj != null)
      {
        internalPath.Add(obj.name);
        assetData.SetInternalAssetPath(internalPath);
      }
    }

    public void popPath()
    {
      internalPath.RemoveAt(internalPath.Count - 1);
    }

    //Instead of maintaining a separate hashset, we just put the internal asset
    //into the dictionary.
    public void addInternalAsset(string path, int instanceID)
    {
      SearchAssetData internalSearchObject = new SearchAssetData(path);
      searchAssetsData[path + instanceID] = internalSearchObject;
    }

    // DEPENDENCY HELPER FUNCTIONS

    /**
     * Called to handle whether we should search the given asset path.
     */
    public bool isSearchable(string assetPath)
    {
      return supportedFileTypes.Contains(Path.GetExtension(assetPath));
    }

    /**
     * Whether or not the currently searched item is of a type that a SearchItem
     * wants to process.
     */
    public bool searchItemCaresAboutAsset()
    {
      return search.searchItemCaresAboutAsset(this);
    }


    public void searchDependency(SearchAssetData data)
    {
      asset = (UnityEngine.Object) AssetDatabase.LoadAssetAtPath(data.assetPath, typeof(UnityEngine.Object) );
      assetData = data;
      if(asset is GameObject)
      {
        GameObject go = (GameObject) asset;
        searchGameObject(go);
      }else{
        searchObject(asset);
      }
      if(assetData.assetIsDirty)
      {
        //TODO save asset?
      }
      assetData.hasBeenSearched = true;
    }

    public void searchDependencies(UnityEngine.Object rootObject)
    {
      searchDependencies(new UnityEngine.Object[]{ rootObject });
    }

    public void searchDependencies(UnityEngine.Object[] rootObjects)
    {
      if(!scope.searchDependencies)
      {
        return;
      }
      if(scope.assetScope == SearchScope.allAssets && scope.projectScope == ProjectScope.EntireProject)
      {
        // We are searching literally everything we can, so no need to load
        // things more.
        return;
      }
      //search dependencies
      UnityEngine.Object[] dependencies = EditorUtility.CollectDependencies(rootObjects.ToArray());

      foreach(UnityEngine.Object dependency in dependencies)
      {
        string path = AssetDatabase.GetAssetPath(dependency);
        if(!searchAssetsData.ContainsKey(path))
        {
          // empty strings mean it is local object.
          if(path != string.Empty)
          {
            // do we support this file extension?
            if(isSearchable(path))
            {
              SearchAssetData data = new SearchAssetData(path);
              searchAssetsData.Add(path, data);
              searchDependency(data);
            }
          }
        }
      }
    }


  }
}