using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecsFor.Mvc;
namespace LibraryIntegrationTest
{
    //This class use to initialize virtual server to simulate in integration test.
    [TestClass]
    public class HostConfig
    {
        private static SpecsForIntegrationHost host;
        [AssemblyInitialize]
        public static void InitialHost(TestContext testContext)
        {
            SpecsForMvcConfig config = new SpecsForMvcConfig();
           config.UseIISExpress().With(Project.Named("ParatabLib"))
                  .CleanupPublishedFiles()
                  .UseMSBuildExecutableAt(@"C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe")
                  .ApplyWebConfigTransformForConfig("Debug");
            config.BuildRoutesUsing(r => ParatabLib.RouteConfig.RegisterRoutes(r));
            config.UseBrowser(BrowserDriver.InternetExplorer);
            host = new SpecsForIntegrationHost(config);
            host.Start();
        }

        [AssemblyCleanup]
        public static void CloseHost()
        {
            host.Shutdown();
        }
    }
}
