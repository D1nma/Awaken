namespace sr
{
  public class SRVersion
  {
    public static string BuildNumber = "4";
    public static int MajorVersion = 1;
    public static string MinorVersion = "8";
#if DEMO
    public static string DemoVersion = "DEMO";
#else
    public static string DemoVersion = "";
#endif
    public static string GetVersion()
    {
      return string.Format("{0}.{1}.{2}{3}", MajorVersion, MinorVersion, BuildNumber, DemoVersion);
    }
  } 
}