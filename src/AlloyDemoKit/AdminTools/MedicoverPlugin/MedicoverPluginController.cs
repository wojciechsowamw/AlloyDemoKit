using System.Web.Mvc;
using AlloyDemoKit.AdminTools.MedicoverPlugin.ViewModels;
using AlloyDemoKit.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.ServiceLocation;

namespace AlloyDemoKit.AdminTools.MedicoverPlugin
{
    [GuiPlugIn(
        Area = PlugInArea.AdminMenu,
        Url = "/custom-plugins/medicover-plugin",
        DisplayName = "Medicover Plugin")]
    [Authorize(Roles = "CmsAdmins")]
    public class MedicoverPluginController : Controller
    {
        // GET
        public ActionResult Index()
        {
            var model = new MedicoverPluginViewModel { Text = "Lorem Ipsum Dolor" };

            return View("~/AdminTools/MedicoverPlugin/Views/Index.cshtml", model);
        }

        [HttpPost]
        public ActionResult SetVisibilityInMenu(MedicoverPluginViewModel model)
        {
            if (model != null & ModelState.IsValid)
            {
                var cRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
                var page = cRepository.Get<ProductPage>(ContentReference.Parse(model.PageId.ToString()));

                if (page != null)
                {
                    var p = page.CreateWritableClone() as ProductPage;
                    p.VisibleInMenu = false;
                    p.UniqueSellingPoints.Clear();
                    p.MetaKeywords.Clear();
                    cRepository.Save(p, SaveAction.Publish, AccessLevel.Publish);
                    model.Text = "Should not be visible";
                }

            }

            return View("~/AdminTools/MedicoverPlugin/Views/Index.cshtml", model);
        }
    }
}