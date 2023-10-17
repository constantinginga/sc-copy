using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Security;

namespace StartupCentral.Code.Controllers
{
    public class PitchController : Controller
    {

        private IMember currentMember = null;



        public PitchController()
        {
            UmbracoHelper helper = Umbraco.Web.Composing.Current.UmbracoHelper;
            MembershipHelper membershipHelper = helper.MembershipHelper;
            currentMember = ApplicationContext.Current.Services.MemberService.GetByUsername(membershipHelper.CurrentUserName);

            // https://our.umbraco.com/forum/using-umbraco-and-getting-started/100868-get-current-user 
            /*
            var wrapper = new HttpContextWrapper(HttpContext.Current);
            var identity = wrapper.GetCurrentIdentity(true);
            currentMember = ApplicationContext.Current.Services.MemberService.GetByProviderKey(identity.Id);
            */
        }

        public IContent GetOrCreatePitch(string contentTypeAlias)
        {
            IContent student = StudentsController.GetOrCreateStudent(currentMember);
            if (student != null)
            {
                IContent pitch = ApplicationContext.Current.Services.ContentService.GetPagedChildren(student.Id, 0, int.MaxValue, out long totalRecords)
                    .Where(itm => itm.ContentType.Alias == contentTypeAlias && itm.GetValue<bool>("wwcompleted") != true).FirstOrDefault();
                if (pitch == null)
                {
                    pitch = ApplicationContext.Current.Services.ContentService.CreateAndSave("Pitch", student, contentTypeAlias);
                }

                return pitch;
            }

            return null;
        }





        //// GET: Pitch
        //public ActionResult Index()
        //{
        //    return View();
        //}
    }
}