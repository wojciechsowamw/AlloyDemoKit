using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AlloyDemoKit.Helpers;
using AlloyDemoKit.Models.GraphApi;
using AlloyDemoKit.Models.Pages;
using AlloyDemoKit.Models.ViewModels;
using EPiServer.Social.Helpers;
using EPiServer.Web.Mvc;

namespace AlloyDemoKit.Controllers
{
    public class GraphController : PageController<GraphApiPage>
    {
        public async Task<ActionResult> Index(GraphApiPage currentPage)
        {
            ClaimsPrincipal principal = HttpContext.User as ClaimsPrincipal;
            if (principal.Identity.IsAuthenticated)
            {
                string upn = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")?.Value;
                upn = HttpUtility.UrlEncode(upn);

                //var graphService = ServiceLocator.Current.GetInstance<GraphService>();
                var graphProvider = new GraphAuthProvider(MemoryCache.Default);
                var graphService = new GraphService(MemoryCache.Default, new GraphSdkHelper(graphProvider), graphProvider);
                var picture = graphService.GetPictureBase64(upn);
                CompositeGraphObject compositeGraphObject = await graphService.GetUserJson(upn, null);

                currentPage.CompositeGraphObject = compositeGraphObject;

                return View(PageViewModel.Create(currentPage));
            }

            return View();
        }
    }
}