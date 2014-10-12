using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecsFor.Mvc;
namespace LibraryIntegrationTest
{
    [TestClass]
    public class HostConfig
    {
        private static SpecsForIntegrationHost host;
        [AssemblyInitialize]
        public static void InitialHost(TestContext testContext)
        {
            SpecsForMvcConfig config = new SpecsForMvcConfig();
            config.UseIISExpress().With(Project.Named("TestLibrary"))
                  .CleanupPublishedFiles()
                  .UseMSBuildExecutableAt(@"C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe")
                  .ApplyWebConfigTransformForConfig("Debug");
            config.BuildRoutesUsing(r => TestLibrary.RouteConfig.RegisterRoutes(r));
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
