using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.Mvc;
using System.Web.Mvc;

namespace StartupCentral.Controllers
{
    public class AboutController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/SC_About/";

        public ActionResult RenderAboutHeader()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_AboutHeader.cshtml");
        }
        public ActionResult RenderAboutHistory()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_AboutHistory.cshtml");
        }
        public ActionResult RenderAboutPictures()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_AboutPictures.cshtml");
        }
        public ActionResult RenderAboutTeam()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_AboutTeam.cshtml");
        }
    }
}