#define PSR_FULL
#if DEMO
#undef PSR_FULL
#endif

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;

namespace sr
{
  [System.Serializable]
  public class ReplaceItemBool : ReplaceItem<DynamicTypeBool, bool>
  {

    protected override bool drawEditor()
    {
      return EditorGUILayout.Toggle(Keys.Replace, replaceValue);
    }

    protected override void replace(SearchJob job, SerializedProperty prop, SearchResult result)
    {
#if PSR_FULL
      prop.boolValue = replaceValue;
      result.replaceStrRep = replaceValue.ToString();
#endif
    }

    protected override void reimport(SearchJob job, SerializedProperty prop, SearchResult result)
    {
#if PSR_FULL
      AssetImporter importer = AssetImporter.GetAtPath(job.assetData.assetPath);
      MethodInfo m = AssetImporterMethodUtil.GetMethodForProperty(importer, prop);
      if(m != null)
      {
        m.Invoke(importer, new object[]{replaceValue});
        importer.SaveAndReimport();
        result.replaceStrRep = replaceValue.ToString();
      }else{
        result.replaceStrRep = "Unsupported";
      }
#endif
    }
  }

}