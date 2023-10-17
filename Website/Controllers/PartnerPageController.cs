using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.Mvc;
using System.Web.Mvc;

namespace StartupCentral.Controllers
{
    public class PartnerPageController : SurfaceController
    {
        private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/SC_PartnerPage/";
        public ActionResult RenderPartnerPageHeader()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_PartnerPageHeader.cshtml");
        }
        public ActionResult RenderPartnerPageIntro()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_PartnerPageIntro.cshtml");
        }
        public ActionResult RenderPartnerPageContent()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_PartnerPageContent.cshtml");
        }
        public ActionResult RenderPartnerPageSectionOne()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_PartnerPageSectionOne.cshtml");
        }
        public ActionResult RenderPartnerPageSectionTwo()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_PartnerPageSectionTwo.cshtml");
        }
        public ActionResult RenderPartnerPageSectionThree()
        {
            return PartialView(PARTIAL_VIEW_FOLDER + "_PartnerPageSectionOne.cshtml");
        }
    }
}