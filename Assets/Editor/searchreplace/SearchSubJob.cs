using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace sr
{
  /**
   * The base class for a search sub job. The SearchJob class executes a number
   * of SearchSubJobs classes based on the type of assets being searched. This
   * class also has sub-classes to help with searching unique object types or
   * locations (scenes, etc).
   */
  public class SearchSubJob
  {
    protected SearchJob job;
    protected int index = 0;
    public List<string> assetPaths;
    string[] suffixes;
    protected AssetScope assetScope;
    protected bool assetRequiresImporter = false;

    public SearchSubJob(SearchJob j, AssetScope sc, string[] allAssets, string[] s, bool ari)
    {
      job = j;
      suffixes = s;
      assetScope = sc;
      assetRequiresImporter = ari;
      foreach(string ss in s)
      {
        job.supportedFileTypes.Add(ss);
      }
      filterAssetPaths(allAssets);
    }

    /**
     * Before we can do the search, we must determine the list of potential
     * assets that can be searched.
     */
    protected virtual void filterAssetPaths(string[] allAssets)
    {
      assetPaths = new List<string>(); //default to nothing.

      if(job.scope.projectScope == ProjectScope.EntireProject)
      {
        //If the asset scope is set and has this value OR the scope is not set at all (everything)
        if((job.scope.assetScope & assetScope) == assetScope || job.scope.assetScope == AssetScope.None)
        {
          IEnumerable<string> filteredPaths = allAssets.Where( asset => asset.StartsWith("Assets/")).Where( asset => suffixes.Any( suffix => asset.EndsWith(suffix, System.StringComparison.OrdinalIgnoreCase)));
          addItems(filteredPaths);
        }else{
          // Debug.Log("[SearchSubJob] Ignoring, not in scope.");
        }
      }else if(job.scope.projectScope == ProjectScope.SpecificLocation)
      {
        addObjectsInLocation(job.scope.scopeObj, allAssets, job.scope.assetScope);

      }else if(job.scope.projectScope == ProjectScope.CurrentSelection)
      {
        foreach(UnityEngine.Object obj in Selection.objects)
        {
          ObjectID objID = new ObjectID(obj);
          objID.Clear();
          if(objID.isSceneObject)
          {
            //scene objects handled in scenesubjob.
          }else{
            //Ignore asset scope for current selection.
            addObjectsInLocation(objID, allAssets, SearchScope.allAssets);
          }
        }
      }else if(job.scope.projectScope == ProjectScope.SceneView){
        //this is handled in the SceneSubJob.
      }
    }

    void addObjectsInLocation(ObjectID scopeObj, string[] allAssets, AssetScope jobScope)
    {
      string[] addedPaths = new string[0];
      if(scopeObj.isDirectory)
      {
        if((jobScope & assetScope) == assetScope)
        {
          string scopePath = scopeObj.assetPath;
          addedPaths = allAssets.Where( asset => suffixes.Any( suffix => asset.EndsWith(suffix, System.StringComparison.OrdinalIgnoreCase))).ToArray();
          addedPaths = addedPaths.Where( asset => asset.StartsWith(scopePath)).ToArray();
          // Debug.Log("[SearchSubJob"+GetType()+"] Found "+addedPaths.Length + " things in "+scopePath);
          // foreach(string path in addedPaths)
          // {
          //   Debug.Log("[SearchSubJob] found:"+path);
          // }
        }else{
          // Debug.Log("[SearchSubJob] Ignoring, not in scope.");
        }
      }else{
        if(suffixes.Any( suffix => scopeObj.assetPath.EndsWith(suffix)))
        {
          // Debug.Log("[SearchSubJob] searching object:"+scopeObj.assetPath);
          addedPaths = new string[1];
          addedPaths[0] = scopeObj.assetPath;
        }
      }
      addItems(addedPaths);
      // Debug.Log("[SearchSubJob] added:"+addedPaths.Length);
    }

    //Verifies our addition to the list and maintains that no duplicates can occur.
    protected void addItems(IEnumerable<string> addedPaths)
    {
      foreach(string addedPath in addedPaths)
      {
        if(!job.searchAssetsData.ContainsKey(addedPath))
        {
          // Debug.Log("[SearchSubJob] Adding:"+addedPath);
          SearchAssetData data = new SearchAssetData(addedPath);
          data.assetRequiresImporter = assetRequiresImporter;
          data.assetScope = assetScope;
          job.searchAssetsData.Add(addedPath, data);
          assetPaths.Add(addedPath);
        }
      }
    }

    public virtual bool SearchNextAsset()
    {
      if(index >= assetPaths.Count)
      {
        return false;
      }
      string assetPath = assetPaths[index];
      index++;
      bool hasMoreItems = index != assetPaths.Count;
      SearchAssetData assetData = job.searchAssetsData[assetPath];
      if(assetData.hasBeenSearched)
      {
        //looks like the item has already been searched!
        return hasMoreItems;
      }
      assetData.hasBeenSearched = true;
      job.assetData = assetData;
      if(job.searchItemCaresAboutAsset())
      {
        // Debug.Log("[SearchSubJob] processing:"+assetPath);
        processAsset(assetPath);
      }else{
        // Debug.Log("[SearchSubJob] ignoring:"+assetPath);
      }
      return hasMoreItems;
    }

    public virtual void JobDone() 
    {
    }

    protected bool processingOneAsset()
    {
      return (job.scope.projectScope == ProjectScope.SpecificLocation && assetPaths.Count == 1);
    }

    protected virtual void processAsset(string assetPath)
    {
      // Don't assume that this asset's *the* asset!
      if(processingOneAsset() && job.scope.scopeObj.assetPath == assetPath)
      {
        //We only have one object, and its already loaded.
        job.asset = job.scope.scopeObj.obj;
      }else{
        //Load the object.
        job.asset = (UnityEngine.Object) AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object) );
      }
      if(job.asset == null)
      {
        return;
      }
      // GameObject foo = (GameObject) job.asset;
      // Debug.Log("[SearchSubJob] foo:"+foo);
      if(job.asset is GameObject)
      {
        job.searchGameObject((GameObject)job.asset);
      }else if(job.asset is AnimatorController)
      {
        job.searchAnimatorController((AnimatorController)job.asset);
      }else{
        job.searchObject(job.asset);
      }
      job.searchDependencies(job.asset);
    }



  }
}