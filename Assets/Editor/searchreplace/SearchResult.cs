using UnityEngine;
using UnityEditor;
using System.Text;
#if UNITY_2018_3_OR_NEWER
using UnityEditor.Experimental.SceneManagement;
#endif

namespace sr
{
  /**
   * When a search result is found, it is encapsulated in this class. This provides
   * a potential view onto a search result without loading the object into memory.
   */
  public class SearchResult
  {

    public bool alternate = false;
    public int recordNum;
    public SearchAction actionTaken = SearchAction.Found;
    // String representation of the search and replace items. Used for display.
    public string strRep;
    public string replaceStrRep;
    public string error;

    public const string notFound = "{6} Did not find <b>{0}</b>.";
    public const string notFoundCompact = "{0}";

    public const string found = "{6} Found <b>{0}</b> on {2}";
    public const string foundCompact = "{3}";
    
    public const string replaced = "{6} Replaced <b>{0}</b> with <b>{1}</b> on {2}";
    public const string replacedCompact = "{3}";

    public const string instanceFound = "{6} Instance Found <b>{0}</b> on {2}";
    public const string instanceFoundCompact = "{3}";

    public const string instanceReplaced = "{6} Replaced Instance <b>{0}</b> with <b>{1}</b> on {2}";
    public const string instanceReplacedCompact = "{3}";

    public const string errorTemplate = "{6} <color=red>ERROR:</color> {5} Found <b>{0}</b> on {2}";
    public const string errorTemplateCompact = "{5}";
    public const string unknown = "{6} Unknown Action Taken!";

    public static SearchResult selectedResult = null;

    public PathInfo pathInfo;

    public virtual void CopyToClipboard(StringBuilder sb)
    {
      string template = "";

      switch(actionTaken)
      {
        case SearchAction.Found:
        //search only.
        template = found;
        break;
        case SearchAction.Replaced:
        template = replaced;
        break;
        case SearchAction.InstanceFound:
        template = instanceFound;
        break;
        case SearchAction.InstanceReplaced:
        template = instanceReplaced;
        break;
        case SearchAction.Error:
        template = errorTemplate;
        break;
        case SearchAction.NotFound:
        template = notFound;
        break;
        default:
        template = unknown;
        break;
      }
      string labelStr = format(template);
      labelStr = labelStr.Replace("<b>", "").Replace("</b>", "");
      sb.Append(labelStr);
      sb.Append("\n");
    }


    public virtual void Draw()
    {
      GUIStyle resultStyle = alternate ? SRWindow.resultStyle1 : SRWindow.resultStyle2;
      if(selectedResult == this)
      {
        resultStyle = SRWindow.selectedStyle;
      }
      GUILayout.BeginHorizontal(resultStyle);
      string labelStr = "";
      string template = "";
      switch(actionTaken)
      {
        case SearchAction.Found:
        //search only.
        template = SRWindow.Instance.Compact() ? foundCompact : found;
        break;
        case SearchAction.Replaced:
        template = SRWindow.Instance.Compact() ? replacedCompact : replaced;
        break;
        case SearchAction.InstanceFound:
        template = SRWindow.Instance.Compact() ? instanceFoundCompact : instanceFound;
        break;
        case SearchAction.InstanceReplaced:
        template = SRWindow.Instance.Compact() ? instanceReplacedCompact : instanceReplaced;
        break;
        case SearchAction.Error:
        template = SRWindow.Instance.Compact() ? errorTemplateCompact : errorTemplate;
        break;
        case SearchAction.NotFound:
        template = SRWindow.Instance.Compact() ? notFoundCompact : notFound;
        break;

        default:
        template = unknown;
        break;
      }
      labelStr = format(template);
      float width = SRWindow.Instance.position.width - 80;
      GUIContent content = new GUIContent(labelStr);
      float height = SRWindow.richTextStyle.CalcHeight(content, width);
      EditorGUILayout.SelectableLabel(labelStr, SRWindow.richTextStyle, GUILayout.Height(height));
      Texture2D icon = SRWindow.prefabIcon;
      if(pathInfo.objID.isSceneObject)
      {
        icon = SRWindow.goIcon;
      } 
      if(GUILayout.Button(icon , new GUILayoutOption[]{GUILayout.Width(30), GUILayout.Height(20) } ))
      {
        if(pathInfo.objID.isSceneObject)
        {
          SceneUtil.OpenObjectInScene(pathInfo);
        }else{
          if (pathInfo.prefabType == PrefabTypes.NestedPrefab)
          {
            // open prefab stage for parent
            var assetPath = AssetDatabase.GetAssetPath(pathInfo.nestedParentGameObjectID);
            var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
            AssetDatabase.OpenAsset(obj);
            var isStage = SceneUtil.IsSceneStage();
            // select object in stage view
            if (isStage)
            {
#if UNITY_2018_3_OR_NEWER
              // var stage = PrefabStageUtility.GetCurrentPrefabStage();
              Selection.activeObject = pathInfo.objID.searchForObjectInScene();
#endif
            }
            else // fallback to selecting parent object.
            {
              Debug.LogWarning("[SearchResult] Could not open Prefab Stage for id " + pathInfo.nestedParentGameObjectID);
              Selection.activeInstanceID = pathInfo.nestedParentGameObjectID;
            }
            //Debug.Log("[SearchResult] nestedParentGameObjectID: " + pathInfo.nestedParentGameObjectID);
          }
          else
          {
            Selection.activeInstanceID = pathInfo.gameObjectID;
            var assetPath = AssetDatabase.GetAssetPath(pathInfo.nestedParentGameObjectID);
            var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
            AssetDatabase.OpenAsset(obj);

            //Debug.Log("[SearchResult] GameObjectID: " + pathInfo.gameObjectID);
          }
          if(AnimationUtil.isInternalAnimationObject(Selection.activeObject))
          {
            // first show the animator window.
            EditorApplication.ExecuteMenuItem("Window/Animator");
            // now get the animator to display it.
            string assetPath = AssetDatabase.GetAssetPath(pathInfo.gameObjectID);
            UnityEngine.Object assetObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
            //set it as the selection.
            Selection.activeObject = assetObj;
            //wait until inspectors are updated and now we can set the 
            // selection to the internal object.
            EditorApplication.delayCall += ()=>{ 
              Selection.activeInstanceID = pathInfo.gameObjectID;
            };
          }

        }
        selectedResult = this;
      }
      GUILayout.EndHorizontal();
    }


    string format(string template)
    {
      return string.Format(template, strRep, replaceStrRep, pathInfo.FullPath(), pathInfo.compactObjectPath, pathInfo.objectPath, error, recordNum.ToString());
    }

  }
}