using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Providers.Entities;
using AlloyDemoKit.Helpers;
using AlloyDemoKit.Models.GraphApi;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Forms.Helpers.Internal;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;

namespace AlloyDemoKit.Models.Pages
{
    [ContentType(DisplayName = "GraphApiPage", GUID = "50ed1e4d-ebb9-4192-a485-4630952baea7", Description = "")]
    public class GraphApiPage : StandardPage
    {
        [Ignore]
        public virtual CompositeGraphObject CompositeGraphObject { get; set; }

    }
}