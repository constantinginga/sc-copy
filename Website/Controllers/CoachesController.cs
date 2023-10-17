using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.Mvc;
using System.Web.Mvc;

namespace StartupCentral.Controllers
{
    public class CoachesController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/SC_Coaches/";
        public ActionResult RenderCoachesHeader()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_CoachesHeader.cshtml");
        }
        public ActionResult RenderCoachesContent15()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_CoachesContent15.cshtml");
        }
        public ActionResult RenderCoachesContent30()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_CoachesContent30.cshtml");
        }
        public ActionResult RenderCoachesContent45()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_CoachesContent45.cshtml");
        }
        public ActionResult RenderCoachesContent60()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_CoachesContent60.cshtml");
        }
    }
}