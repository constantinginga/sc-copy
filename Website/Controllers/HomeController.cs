using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace StartupCentral.Controllers
{
    public class HomeController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/SC_Home/";

        public ActionResult RenderHomeLanding()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_HomeLanding.cshtml");
        }
        public ActionResult RenderHomeOffers()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_HomeOffers.cshtml");
        }
        public ActionResult RenderHomePartners()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_HomePartners.cshtml");
        }
        public ActionResult RenderHomeServices()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_HomeServices.cshtml");
        }

        public ActionResult RenderHomeCoaches()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_HomeCoaches.cshtml");
        }
        public ActionResult RenderHomeStatistics()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_HomeStatistics.cshtml");
        }
    }
}