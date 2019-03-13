using System.Runtime.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using AlloyDemoKit.Helpers;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace AlloyDemoKit.AdminTools.MedicoverPlugin
{
    [InitializableModule]
    public class MedicoverPluginInitialization : IInitializableModule, IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
            RouteTable.Routes.MapRoute(
                null,
                "custom-plugins/medicover-plugin",
                new { controller = "MedicoverPlugin", action = "Index" });
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            //var graphAuthProvider = new GraphAuthProvider(MemoryCache.Default);
            //var sdkHelper = new GraphSdkHelper(graphAuthProvider);
            //context.Services.Add(typeof(IGraphAuthProvider), graphAuthProvider);
            //context.Services.Add(typeof(IGraphSdkHelper), sdkHelper);
            //context.Services.Add(typeof(GraphService),
            //    new GraphService(MemoryCache.Default, sdkHelper, graphAuthProvider));

        }
    }
}