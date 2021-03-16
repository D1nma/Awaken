using UnityEngine;
using UnityEditor;
#if UNITY_2018_3_OR_NEWER
using UnityEditor.Experimental.SceneManagement;
#endif
namespace sr
{
  /**
   * Utility methods to find out useful bits about prefabs.
   */
  public class PrefabUtil
  {
    /**
     * Provides a method to determine if the given property on the given prefab instance is a modification to the existing prefab locally (in a scene).
     */
    public static bool isInstanceModification(SerializedProperty prop)
    {
      Object obj = prop.serializedObject.targetObject;

      PrefabTypes prefabType = PrefabUtil.GetPrefabType(obj);
      if(prefabType == PrefabTypes.PrefabInstance || prefabType == PrefabTypes.NestedPrefabInstance)
      {
        PropertyModification[] pms = PrefabUtility.GetPropertyModifications(obj);
#if UNITY_2018_2_OR_NEWER
        UnityEngine.Object parent = PrefabUtility.GetOutermostPrefabInstanceRoot(obj);
#else
        UnityEngine.Object parent = PrefabUtility.GetPrefabParent(obj);
#endif

        if(pms.Length > 0)
        {
          foreach(PropertyModification pm in pms)
          {
            // Debug.Log("[PrefabUtil] pm:"+pm.propertyPath);
            if(pm.target == parent && pm.propertyPath.StartsWith(prop.propertyPath) )
            {
              return true;
            }
          }
        }
      }
      return false;
    }

    public static bool isInPrefabInstance(UnityEngine.Object obj)
    {
      // Debug.Log("[PrefabUtil] prefab type:"+PrefabUtility.GetPrefabType(obj)+ " : "+ obj.name + " parent:"+PrefabUtility.GetPrefabObject(obj));
      var type = GetPrefabType(obj);
      return type == PrefabTypes.PrefabInstance || type == PrefabTypes.NestedPrefabInstance;
    }


    public static bool isPrefab(UnityEngine.Object obj)
    {
      // Debug.Log("[PrefabUtil] prefab type:"+PrefabUtility.GetPrefabType(obj)+ " : "+ obj.name + " parent:"+PrefabUtility.GetPrefabObject(obj));
      PrefabTypes p = PrefabUtil.GetPrefabType(obj);
      return p == PrefabTypes.Prefab || p == PrefabTypes.PrefabVariant || p == PrefabTypes.NestedPrefab;
    }

    public static bool isPrefabRoot(UnityEngine.Object obj)
    {
      if(obj == null)
      {
        return false;
      }
      if(obj is GameObject)
      {
        GameObject go = (GameObject) obj;
        if(isPrefab(go))
        {
#if UNITY_2018_3_OR_NEWER
          //we know this is a prefab.
          return go.transform.root.gameObject;
#else
          return PrefabUtility.FindPrefabRoot(go) == obj;
#endif
        }
      }
      return false;
    }

    /// <summary>
    /// Returns true if the prefab is nested
    /// </summary>
    public static bool IsNestedPrefab(Object obj)
    {
      if (obj == null)
        return false;
      
#if UNITY_2018_3_OR_NEWER
      var go = obj as GameObject;
      if (go == null)
        return false;
      var isAsset = go.scene.name == null;
      if (isAsset)
      {
        return go.transform.root.gameObject != go;
      }

      var outermost = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
      return outermost != null && outermost != go;
#else
      return false;
#endif
    }

    public static PrefabTypes GetPrefabType(UnityEngine.Object obj)
    {
#if UNITY_2018_3_OR_NEWER
      switch(PrefabUtility.GetPrefabAssetType(obj))
      {
        case PrefabAssetType.NotAPrefab:
        return PrefabTypes.NotAPrefab;
        case PrefabAssetType.Model:
        case PrefabAssetType.Regular:
        switch(PrefabUtility.GetPrefabInstanceStatus(obj))
        {
          case PrefabInstanceStatus.NotAPrefab:
            if (!PrefabUtility.IsPartOfAnyPrefab(obj))
              return PrefabTypes.NotAPrefab;
            return IsNestedPrefab(obj) ? PrefabTypes.NestedPrefab : PrefabTypes.Prefab;

          case PrefabInstanceStatus.Connected:
            return IsNestedPrefab(obj) ? PrefabTypes.NestedPrefabInstance : PrefabTypes.PrefabInstance;

          default:
          return PrefabTypes.MissingOrDisconnected;
        }
        case PrefabAssetType.Variant:
        switch(PrefabUtility.GetPrefabInstanceStatus(obj))
        {
          case PrefabInstanceStatus.NotAPrefab:
          return PrefabTypes.PrefabVariant;

          case PrefabInstanceStatus.Connected:
          return PrefabTypes.PrefabVariantInstance;

          default:
          return PrefabTypes.MissingOrDisconnected;

        }

        default:
        return PrefabTypes.MissingOrDisconnected;

      }
#else
      switch(PrefabUtility.GetPrefabType(obj))
      {
        case PrefabType.None:
        return PrefabTypes.NotAPrefab;
        
        case PrefabType.Prefab:
        return PrefabTypes.Prefab;
        
        case PrefabType.PrefabInstance: 
          return PrefabTypes.PrefabInstance;

        case PrefabType.ModelPrefab:
        return PrefabTypes.ModelPrefab;
        
        case PrefabType.ModelPrefabInstance:
        return PrefabTypes.PrefabInstance;

        default:
        return PrefabTypes.MissingOrDisconnected;
      }
#endif
    }

    public static GameObject getPrefabRoot(UnityEngine.Object obj)
    {
      if(obj == null)
      {
        return null;
      }
      if(obj is GameObject)
      {
        return getPrefabRoot((GameObject)obj);
      }
      if(obj is Component)
      {
        Component c = (Component)obj;
        return getPrefabRoot(c.gameObject);
      }
      return null;
    }

    public static GameObject getPrefabRoot(GameObject go)
    {
      PrefabTypes type = PrefabUtil.GetPrefabType(go);
      if(type == PrefabTypes.PrefabInstance)
      {
#if UNITY_2018_3_OR_NEWER
        go = (GameObject)PrefabUtility.GetCorrespondingObjectFromSource(go);
#else
        go = (GameObject)PrefabUtility.GetPrefabObject(go);
#endif
        return go;
      }
#if UNITY_2018_3_OR_NEWER
      if (type == PrefabTypes.NestedPrefabInstance)
      {
        go = (GameObject) PrefabUtility.GetOutermostPrefabInstanceRoot(go);
      }
#endif
      if(type == PrefabTypes.Prefab || type == PrefabTypes.PrefabVariant || type == PrefabTypes.NestedPrefab)
      {
        return go.transform.root.gameObject;
      }
      return null;
    }

    public static void SwapPrefab(SearchJob job, SearchResult result, GameObject gameObjToSwap, GameObject prefab, bool updateTransform, bool rename)
    {
      Transform swapParent = gameObjToSwap.transform.parent;
      int index = gameObjToSwap.transform.GetSiblingIndex();

      result.replaceStrRep = prefab.name;
      result.strRep = gameObjToSwap.name;
      // Debug.Log("[ReplaceItemSwapObject] Instantiating:"  +prefab, prefab);
      GameObject newObj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
      if(newObj != null)
      {
        newObj.transform.parent = swapParent;
        newObj.transform.SetSiblingIndex(index);
        Transform oldT = gameObjToSwap.transform;
        if(updateTransform)
        {
          newObj.transform.rotation = oldT.rotation;
          newObj.transform.localPosition = oldT.localPosition;
          newObj.transform.localScale = oldT.localScale;
        }
        if(rename)
        {
          newObj.name = gameObjToSwap.name;
        }
        result.pathInfo = PathInfo.GetPathInfo(newObj, job);

        UnityEngine.Object.DestroyImmediate(gameObjToSwap);
      }else{
        Debug.Log("[Search & Replace] No object instantiated...hrm!");
      }
    }
  }
}
