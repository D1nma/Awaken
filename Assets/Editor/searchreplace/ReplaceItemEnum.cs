#define PSR_FULL
#if DEMO
#undef PSR_FULL
#endif
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace sr
{
  [System.Serializable]
  public class ReplaceItemEnum : ReplaceItem<DynamicTypeEnum, int>
  {
    public string[] names;

    protected override int drawEditor()
    {
      names = System.Enum.GetNames(type); //todo: improve caching?
      return EditorGUILayout.Popup(Keys.Replace, replaceValue, names);
    }

    protected override void replace(SearchJob job, SerializedProperty prop, SearchResult result)
    {
#if PSR_FULL
      if(prop.propertyType == SerializedPropertyType.Enum)
      {
        prop.enumValueIndex = replaceValue;
      }else{
        prop.intValue = replaceValue;
      }
      result.replaceStrRep = names[replaceValue];
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
        result.replaceStrRep = names[replaceValue];
      }else{
        result.replaceStrRep = "Unsupported";
      }
#endif
    }

  }

 

}