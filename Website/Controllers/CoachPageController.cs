using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.Mvc;
using System.Web.Mvc;

namespace StartupCentral.Controllers
{
    public class CoachPageController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/SC_CoachPage/";

        public ActionResult RenderCoachPageHeader()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_CoachPageHeader.cshtml");
        }
        public ActionResult RenderCoachPageIntro()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_CoachPageIntro.cshtml");
        }
        public ActionResult RenderCoachPageContent()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_CoachPageContent.cshtml");
        }
    }
}
