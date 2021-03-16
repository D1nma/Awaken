using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
#if UNITY_2018_3_OR_NEWER
using UnityEditor.Experimental.SceneManagement;
#endif
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace sr
{
  /**
   * Various utilities provides to simplify scene-related actions. Used to be
   * a wrapper for functionality before the 'new' Scene Management API.
   */ 
  public class SceneUtil
  {

    public static Scene? LoadScene(string assetPath, OpenSceneMode mode)
    {
      // Debug.Log("[SceneSubJob] assetPath:"+assetPath);
      UnityEngine.Object sceneObj = AssetDatabase.LoadMainAssetAtPath(assetPath);
      if(sceneObj == null)
      {
        // this probably means we are in a new scene.
        // Debug.Log("[SceneUtil] path is missing for " + assetPath);
        return EditorSceneManager.GetActiveScene();
        // return null;
      }
      // Debug.Log("[SceneUtil] " + assetPath + " sceneObj:"+sceneObj, sceneObj);
      Scene s = EditorSceneManager.GetSceneByPath(assetPath);
      if(!s.IsValid())
      {
        // Debug.Log("[SceneUtil] attempting to open scene");
        s = EditorSceneManager.OpenScene(assetPath, mode);
      }
      EditorSceneManager.SetActiveScene(s); // PathInfo/ObjectID uses GetActiveScene(). Importanté!
      return s;
    }

    public static bool IsSceneStage()
    {
#if UNITY_2018_3_OR_NEWER
     return PrefabStageUtility.GetCurrentPrefabStage() != null;
#else
      return false;
#endif
    }

    public static string GuidPathForActiveScene()
    {
#if UNITY_2018_3_OR_NEWER
      if (IsSceneStage())
      {
        return PrefabStageUtility.GetCurrentPrefabStage().scene.path;
      }
#endif
      return EditorSceneManager.GetActiveScene().path;
    }

    public static bool SaveDirtyScenes()
    {
      return EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
    }


    public static IEnumerable<GameObject> SceneRoots()
     {
         var prop = new HierarchyProperty(HierarchyType.GameObjects);
         var expanded = new int[0];
         while (prop.Next(expanded)) {
             yield return prop.pptrValue as GameObject;
         }
     }

     public static void OpenObjectInScene(PathInfo pathInfo)
     {
       // Debug.Log("[SceneUtil] Opening object:" + pathInfo.FullPath());
       Scene? scene;
       UnityEngine.Object obj;

#if UNITY_2018_3_OR_NEWER
       if (IsSceneStage())
       {
         // find object in stage
         var stage = PrefabStageUtility.GetCurrentPrefabStage();
         obj = pathInfo.objID.searchForObjectInScene(stage.scene);
         if (obj != null) 
         {
           Selection.activeObject = obj;
           return;
         }
         // unload stage first
         // TODO: Looking into unity's cs reference, they plan to allow multiple stages being opened, this might need to be looked at again later.
         StageUtility.GoToMainStage();
       }
#endif
       // if not in stage or object wasnt in the stage we need to load the scene
       scene = LoadScene(pathInfo.assetPath, OpenSceneMode.Single);
       if(scene.HasValue)
       {
         obj = pathInfo.objID.searchForObjectInScene(scene.Value);
         Selection.activeObject = obj;
       }
     }

  }
}