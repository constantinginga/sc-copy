using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.Mvc;
using System.Web.Mvc;

namespace StartupCentral.Controllers
{
    public class StartupLoungeController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/SC_StartupLounge/";

        public ActionResult RenderStartupLoungeHeader()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_StartupLoungeHeader.cshtml");
        }        
        public ActionResult RenderStartupLoungeNetwork()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_StartupLoungeNetwork.cshtml");
        }        
        public ActionResult RenderStartupLoungePicture()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_StartupLoungePicture.cshtml");
        }
        public ActionResult RenderStartupLoungeValue()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_StartupLoungeValue.cshtml");
        }
    }
}