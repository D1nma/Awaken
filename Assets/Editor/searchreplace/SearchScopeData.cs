#define PSR_FULL
#if DEMO
#undef PSR_FULL
#endif
namespace sr
{
  
  public class SearchScopeData
  {
    public ProjectScope projectScope;
    public AssetScope assetScope;
    public ObjectID scopeObj;

    public bool searchDependencies;

    public SearchScopeData(ProjectScope ep, AssetScope s, ObjectID oid, bool sd)
    {
      projectScope = ep;
      assetScope = s;
      scopeObj = oid;
      searchDependencies = sd;
      searchDependencies = false;
    }
  }
}