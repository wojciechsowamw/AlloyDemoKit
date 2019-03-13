using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI.WebControls;
using EPiServer.Personalization;
using EPiServer.PlugIn;
using EPiServer.Security;
using EPiServer.Util.PlugIns;
using System.Web.UI;
using AlloyDemoKit.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.ServiceLocation;

namespace AlloyDemoKit.AdminTools
{
    [GuiPlugIn(DisplayName = "MedicoverTools", Description = "", Area = PlugInArea.AdminConfigMenu, Url = "~/AdminTools/MedicoverTools.aspx")]
    public partial class MedicoverTools : System.Web.UI.Page
    {
        

        // TODO: Add your Plugin Control Code here.

        protected void SetVisibility_Click(object sender, EventArgs e)
        {
            var cRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var idTxt = pageIdTextBox.Text;
            if (!string.IsNullOrWhiteSpace(idTxt) && int.TryParse(idTxt, out int pageId))
            {
                var page = cRepository.Get<SitePageData>(ContentReference.Parse(idTxt));

                if (page != null)
                {
                    page.VisibleInMenu = false;
                    cRepository.Save(page, SaveAction.Publish, AccessLevel.Publish);

                }
            }
        }
    }
}