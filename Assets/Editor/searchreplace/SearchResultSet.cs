using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace sr
{
  /**
   * A SearchResultSet is the results found from a corresponding SearchItemSet.
   * This provides a paginated interface into results so that the UI doesn't get
   * bogged down.
   */
  public class SearchResultSet
  {
    public List<SearchResultGroup> results = new List<SearchResultGroup>(100); //TODO: make it possible to search for > 100 things?

    public int resultsCount = 0;
    public int searchedItems = 0;
    int resultsPerPage = 50;
    int pageNum = 0;
    string[] pageNames;
    int start;
    int end;

    public SearchStatus status;

    public string statusMsg;

    Vector2 scrollPosition;

    public SearchResultSet(SearchItemSet searchSet)
    {
      for(int i = 0; i < 10; i++)
      {
        results.Add(null);
      }
      foreach(SearchItem item in searchSet.validatedItems)
      {
        results[item.sortIndex] = new SearchResultGroup(item);
      }
    }

    public void CopyToClipboard()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("Searched "+searchedItems);
      if(results.Count >= 1)
      {
        sb.Append("Found "+resultsCount+" Total Results.\n");
      }else{
        sb.Append("No results found.");
      }

      foreach(SearchResultGroup resultGroup in results)
      {
        sb.Append("\n");
        resultGroup.CopyToClipboard(sb);
      }
      EditorGUIUtility.systemCopyBuffer = sb.ToString();

    }

    public void Add(SearchResult result, SearchItem item)
    {
      SearchResultGroup resultGroup = results[item.sortIndex];
      // for(int i = 0; i < 10;i++)
      // {
        resultGroup.AddResult(result);
        resultsCount++;
      // }
    }

    public void OnSearchComplete()
    {
      bool foundNull = true;
      while(foundNull)
      {
        foundNull = results.Remove(null);
      }
      int pages = Mathf.CeilToInt((float)resultsCount / (float)resultsPerPage);
      pageNames = new String[pages];
      for(int i = 0; i < pages; i++)
      {
        pageNames[i] = (i+1).ToString();
      }

      int items = 1;
      for(int i = 0; i< results.Count; i++ )
      {
        SearchResultGroup result = results[i];
        result.startIndex = items - 1;
        // items += result.results.Count;
        // Debug.Log("[SearchResultSet] result:"+i+" startIndex:"+result.startIndex+" endIndex:"+result.endIndex);
        for(int j = 0; j < result.results.Count;j++)
        {
          SearchResult sr = result.results[j];
          sr.recordNum = items++;
        }
        result.endIndex = items - 1;
      }

    }

    public void Draw()
    {

      if(status != SearchStatus.Complete)
      {
        GUILayout.Label(statusMsg);
        return;
      }
      if(resultsCount >= 1)
      {
        start = pageNum * resultsPerPage;
        end = Math.Min(start + resultsPerPage, resultsCount);
        if(resultsCount > resultsPerPage)
        {
          GUILayout.Label("Showing " + start + "-" + end + " of <b>"+resultsCount+"</b> Total Results. Searched " + searchedItems + " assets." , SRWindow.richTextStyle);
        }else{
          GUILayout.Label("Found <b>"+resultsCount+"</b> Total Results. Searched " + searchedItems + " assets.", SRWindow.richTextStyle);
        }
      }else{
        GUILayout.Label("No results found, searched "+ searchedItems + " assets.", SRWindow.richTextStyle);

      }
      if(resultsCount >= 1)
      {
        drawPageSelector();
      }
      scrollPosition = GUILayout.BeginScrollView(scrollPosition);
      drawPage();
      // foreach(SearchResultGroup resultGroup in results)
      // {
      //   resultGroup.Draw();
      // }
      GUILayout.EndScrollView();
      if(SRWindow.Instance.Compact())
      {
        GUILayout.Label(new GUIContent("For more details, make the window wider.", EditorGUIUtility.FindTexture( "d_UnityEditor.InspectorWindow" )), SRWindow.richTextStyle);
      }
    }

    void drawPageSelector()
    {
      if(resultsCount <= resultsPerPage)
      {
        return;
      }else{
        int xCount = (int)((float)SRWindow.Instance.position.width / 40.0f);
        pageNum = GUILayout.SelectionGrid(pageNum, pageNames, xCount);
      }
    }

    void drawPage()
    {
      if(resultsCount == 0)
      {
        return;
      }else{
        // Debug.Log("[SearchResultSet] start:"+start+" end:"+end);
        int itemsDrawn = 0;
        int itemsToDraw = end - start;
        int itemsIndex = start;
        int currentResultIndex = getStartGroup(start);
        // Debug.Log("[SearchResultSet] drawing from result: "+currentResultIndex+ " itemsToDraw:"+itemsToDraw+" itemsIndex:"+itemsIndex + " resultsCount:"+resultsCount);
        while(itemsDrawn < itemsToDraw)
        {
          SearchResultGroup currentResult = results[currentResultIndex];
          // Debug.Log("[SearchResultSet] itemsIndex:"+itemsIndex + " currentResult.startIndex"+ currentResult.startIndex);
          int groupIndex = itemsIndex - currentResult.startIndex;
          int itemsToDrawInGroup = Math.Min(itemsToDraw - itemsDrawn, currentResult.results.Count - groupIndex);
          currentResult.Draw(groupIndex, itemsToDrawInGroup);
          itemsDrawn += itemsToDrawInGroup;
          itemsIndex += itemsToDrawInGroup;
          currentResultIndex++;
        }
        //Draw any empty results until the next record is found.
        bool foundResults = false;
        int tries = 0;
        do{
          if(currentResultIndex < results.Count)
          {
            SearchResultGroup currentResult = results[currentResultIndex];
            if(currentResult.results.Count == 0)
            {
              currentResult.Draw(0,0);
              //keep going.
              currentResultIndex++;
            }else{
              //we found a non-empty result, exit.
              foundResults = true;
            }
          }
          tries++;
        }while(!foundResults && tries < 10);
      }
    }

    int getStartGroup(int start)
    {
      for(int i = 0; i < results.Count; i++)
      {
        SearchResultGroup result = results[i];
        if(result.startIndex <= start && result.endIndex >= start)
        {
          // Debug.Log("[SearchResultSet] getStartGroup:"+i+ " start:"+start);
          return i;
        }
      }

      return results.Count - 1;
    }

    /**
     * This functions to select active items within the scene and project.
     * It cannot select items when the scene is closed and reopened, since 
     * the instanceIDs go away forEVER.
     */
    public void SelectAll()
    {
      HashSet<int> selections = new HashSet<int>();
      string openScenePath = SceneUtil.GuidPathForActiveScene();
      int unSelectableItems = 0;
      foreach(SearchResultGroup srg in results)
      {
        foreach(SearchResult sr in srg.results)
        {
          if(sr.pathInfo.objID.isSceneObject)
          {
            if(sr.pathInfo.objID.guid == openScenePath)
            {
              Debug.Log("[SearchResultSet] searching in scene:"+sr.pathInfo.FullPath());
              UnityEngine.Object obj = sr.pathInfo.objID.searchForObjectInScene();
              if(obj != null)
              {
                selections.Add(obj.GetInstanceID());
              }
            }else{
              unSelectableItems++;
            }
          }else{
            selections.Add(sr.pathInfo.gameObjectID);
          }
        }
      }
      if(unSelectableItems > 0)
      {
        Debug.LogWarning(  "[Search & Replace] " + unSelectableItems + " items were not selected because they are not within the active scene, have been moved, or deleted.");
      }

      int[] selectionsArray = new int[selections.Count];
      selections.CopyTo(selectionsArray);
      Selection.instanceIDs = selectionsArray;
    }

  }
}