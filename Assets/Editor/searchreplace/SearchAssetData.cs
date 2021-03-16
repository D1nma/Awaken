using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace sr
{
  /**
   * Data model of an object that is searched. Has it already been searched?
   * etc.
   */
  public class SearchAssetData
  {
    public bool hasBeenSearched = false;
    //The path of the asset relative to the project folder or "scene object" if this is a scene object.
    public string assetPath = "";
    public string assetExtension = "";
    // If this asset is internal to another asset (for example, a StateMachine internal to an Animator)
    // then this will provide a path that can be inserted into a path Info object.
    // This should be used only for 'internal' assets!
    // Formatted with backslashes, and should have backslash on the end if this is set.
    public string internalAssetPath = "";
    public List<SearchAssetData> dependencies;

    //Whether search and replace has modified the current asset.
    public bool assetIsDirty = false;

    //Whether saving the asset is enough, or potentially more stuff 
    // needs to occur.
    public bool assetRequiresRefresh = false;

    //Whether or not we need a specific AssetImporter to import this object.
    public bool assetRequiresImporter = false;

    // If a SearchItem has executed a search on this asset, then this asset was
    // searched.
    public bool searchExecuted = false;

    //The scope this asset was created in, or None if its a dependency.
    public AssetScope assetScope = AssetScope.None;

    // Helper function to set the internal asset path of an asset. This will
    // add the appropriate slashes, and if the list is empty, set to string.Empty.
    public void SetInternalAssetPath(List<string> internalPath)
    {
      if(internalPath.Count > 0)
      {
        internalAssetPath = "/" + string.Join("/", internalPath.ToArray());
      }else{
        internalAssetPath = string.Empty;
      }
    }

    public SearchAssetData(string path)
    {
      assetPath = path;
      assetExtension = Path.GetExtension(path);
    }

    public void addDependency(SearchAssetData dependency)
    {
      if(dependencies == null)
      {
        dependencies = new List<SearchAssetData>();
      }
      dependencies.Add(dependency);
    }
  }
}