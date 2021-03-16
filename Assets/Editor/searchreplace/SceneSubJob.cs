using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
#if UNITY_2018_3_OR_NEWER
using UnityEditor.Experimental.SceneManagement;

#endif

namespace sr
{

  /**
   * The search sub job that interfaces with searching scenes.
   */
  public class SceneSubJob : SearchSubJob
  {
    /** Don't count on sceneSetups having scenes! User could be in a new unsaved scene. */
    SceneSetup[] sceneSetups;
    /*
     * At startup we get the scenes that were in place before the job is run so
     * that we know how to get us back to our initial state.
     */
    Dictionary<string, SceneSetup> loadScenesHash = new Dictionary<string, SceneSetup>();

    public SceneSubJob(SearchJob j, AssetScope sc, string[] allAssets, string[] s): base(j, sc, allAssets, s, false)
    {
      sceneSetups = EditorSceneManager.GetSceneManagerSetup();
      foreach(SceneSetup sceneSetup in sceneSetups)
      {
        loadScenesHash[sceneSetup.path] = sceneSetup;
        // Debug.Log("[SceneSubJob] scene:"+sceneSetup.path);
      }
    }

    protected override void filterAssetPaths(string[] allAssets)
    {
      base.filterAssetPaths(allAssets);
      if(job.scope.projectScope == ProjectScope.SceneView)
      {
        // Let's add it!
        // Debug.Log("[SceneSubJob] Adding:"+SceneUtil.GuidPathForActiveScene());
        addItems(new string[] { SceneUtil.GuidPathForActiveScene() } );

      }
    }

    protected override void processAsset(string assetPath)
    { 
      Scene? currentScene = null;

      if(job.scope.projectScope == ProjectScope.SceneView)
      {
        currentScene = EditorSceneManager.GetActiveScene();
      }
      else
      {
        currentScene = SceneUtil.LoadScene(assetPath, OpenSceneMode.Single);
      }

#if UNITY_2018_3_OR_NEWER
      if (SceneUtil.IsSceneStage())
      {
        currentScene = PrefabStageUtility.GetCurrentPrefabStage().scene;
      }
#endif

      // Debug.Log("[SearchJob] scene:"+currentScene.path+" loaded.");
      GameObject[] rootObjects = currentScene.Value.GetRootGameObjects();

      foreach(GameObject go in rootObjects)
      {
        // Debug.Log("[SearchJob] root:"+go);
        job.searchGameObject(go);
      }
      if(job.assetData.assetIsDirty)
      {
        if(!SceneUtil.IsSceneStage())
        {
          EditorSceneManager.SaveScene(currentScene.Value);
        }else{
          // This will dirty the hierarchy, and this will get saved.
        }
      }
      
      job.searchDependencies(rootObjects.ToArray());
    }


    public override void JobDone()
    {
      if(job.scope.projectScope == ProjectScope.SceneView)
      {
        if(job.assetData.assetRequiresRefresh)
        {
          EditorSceneManager.RestoreSceneManagerSetup(sceneSetups);
          return;
        }
      }else{
        //scope isn't just the scene...and we're processing stuff.
        if(assetPaths.Count > 0 && sceneSetups.Length > 0)
        {
          EditorSceneManager.RestoreSceneManagerSetup(sceneSetups);
        }
      }
    }
  }
}