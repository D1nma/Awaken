using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

namespace sr
{
  /**
   * This subjob handles the edge case of searching for an in-scene object. This will
   * search the object and return any values it can. Contains functionality to
   * normalize objects to stop redundant searches.
   * Doesn't use the superclass' assetPaths list since we're not dealing with 
   * assets here.
   */
  public class SceneObjectSubJob : SearchSubJob
  {

    //This is not the same thing as 'assets' but is a hash of all potentially
    // searchable objects so that we can normalize values.
    public HashSet<int> assetLookup = new HashSet<int>();
    public List<UnityEngine.Object> assets = new List<UnityEngine.Object>();

    bool assetRequiresRefresh = false;

    public SceneObjectSubJob(SearchJob j, AssetScope sc, string[] allAssets, string[] s): base(j, sc,  allAssets, s, false)
    {
    }

    protected override void filterAssetPaths(string[] allAssets)
    {
      assetPaths = new List<string>(); //default to nothing.

      if(job.scope.projectScope == ProjectScope.EntireProject)
      {
        //ignore
      }
      else if(job.scope.projectScope == ProjectScope.SpecificLocation)
      {
        if(job.scope.scopeObj.isDirectory)
        {
          //ignore
        }else{
          if(job.scope.scopeObj.isSceneObject)
          {
            addItem(job.scope.scopeObj.obj);
            // Debug.Log("[SearchSubJob] Ignoring because its a scene object.");
            // addItems( new string[] { "sceneobject" } );
            // Debug.Log("[SceneObjectSubJob] Searching:"+job.scope.scopeObj.obj.name);
          }
        }
      }
      else if(job.scope.projectScope == ProjectScope.CurrentSelection)
      {
        foreach(UnityEngine.Object obj in Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel))
        {
          ObjectID objID = new ObjectID(obj);
          if(objID.isSceneObject)
          {
            addItem(objID.obj);
          }
        }
      }
    }

    void addItem(UnityEngine.Object obj)
    {
      assets.Add(obj);
    }

    public override bool SearchNextAsset()
    {
      if(index >= assets.Count)
      {
        return false;
      }
      job.assetData = new SearchAssetData("scene object");
      job.assetData.assetScope = assetScope;

      processAsset(assets[index]);
      index++;
      return index != assets.Count;
    }

    protected void processAsset(UnityEngine.Object obj)
    {
      if(obj is GameObject)
      {
        GameObject go = (GameObject) obj;
        job.searchGameObject(go);
      }else{
        //monobehaviour or something!
        job.searchObject(obj); 
      }
      job.searchDependencies(obj);
      //we will save the scene after all prefabs have been modified.
      if(job.assetData.assetRequiresRefresh)
      {
        assetRequiresRefresh = true;
      }
    }

    public override void JobDone()
    {
      if(assetRequiresRefresh)
      {
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        EditorSceneManager.OpenScene(EditorSceneManager.GetActiveScene().path, UnityEditor.SceneManagement.OpenSceneMode.Single);
      }
    }

  }
}