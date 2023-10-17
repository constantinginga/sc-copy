using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.Mvc;
using System.Web.Mvc;

namespace StartupCentral.Controllers
{
    public class PartnersController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/SC_Partners/";
        public ActionResult RenderPartnersHeader()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_PartnersHeader.cshtml");
        }
        public ActionResult RenderPartnersContent()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_PartnersContent.cshtml");
        }
    }
}