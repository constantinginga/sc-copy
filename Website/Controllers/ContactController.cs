using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.Mvc;
using System.Web.Mvc;

namespace StartupCentral.Controllers
{
    public class ContactController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/SC_Contact/";

        public ActionResult RenderContactHeader()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_ContactHeader.cshtml");
        }
        public ActionResult RenderContactContent()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_ContactContent.cshtml");
        }
    }
}