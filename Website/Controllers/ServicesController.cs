using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.Mvc;
using System.Web.Mvc;

namespace StartupCentral.Controllers
{
    public class ServicesController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/SC_Services/";

        public ActionResult RenderServicesHeader()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_ServicesHeader.cshtml");
        }
        public ActionResult RenderServicesOffers()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_ServicesOffers.cshtml");
        }
        public ActionResult RenderServicesServices()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_ServicesServices.cshtml");
        }
    }
}