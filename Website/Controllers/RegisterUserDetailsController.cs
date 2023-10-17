using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using StartupCentral.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using StartupCentral.Code.Class;
using System.Threading.Tasks;

namespace StartupCentral.Controllers
{
    public class RegisterUserDetailsController : SurfaceController
    {

        [HttpPost]
        public async Task<ActionResult> Submit(RegisterUserDetails model)
        {
            var checkoutPage = Umbraco.Content(29767);

            if (ModelState.IsValid)
            {

                IMember member = Services.MemberService.CreateMemberWithIdentity(model.Email, model.Email, model.Email, "student");
                if (member != null)
                {
                    member.SetValue("wwmobile", Session["UserPhone"]);
                    member.SetValue("wwaddress", model.AddressLine);
                    member.SetValue("wwcity", model.City);
                    member.SetValue("wwpostnummer", model.ZipCode);

                    member.SetValue("wwMobilePayAbonnementsDato", null);
                    member.SetValue("wwemail", model.Email);
                    if (!string.IsNullOrEmpty(model.Cvr))
                    {
                        member.SetValue("cVR", model.Cvr);
                    }
                    if (!string.IsNullOrEmpty(model.UserName))
                    {
                        member.SetValue("wwname", model.UserName);
                    }
                    else
                    {

                    }
                    member.SetValue("wwname", (!string.IsNullOrEmpty(model.UserName) ? model.UserName : model.Email));
                    member.SetValue("umbracoNewsletterSubscriber", model.Newsletter);
                    member.IsApproved = false;
                    Services.MemberService.Save(member);
                    Services.MemberService.AssignRole(model.Email, "student");
                    Services.MemberService.SavePassword(member, model.Password);
                    Session["memberkey"] = member.Key;
                    Session["UserNewsletter"] = model.Newsletter;
                }
                if((string)Session["PlanType"] == "basic")
                {
                    member.SetValue("basicMembershipUser", true);
                    member.SetValue("isFreeLoungeUser", false);
                    Services.MemberService.Save(member);
                    await Emailing.SendFreeUserEmail(member);
                    //return Redirect(Umbraco.Content(34169).Url);
                }
                var redirectLink = (Session["LandingPromoCode"] == null)
                    ? checkoutPage.Url + "?plan=" + (string)Session["PlanType"] + "&memberkey=" + member.Key + "&newsletter=" + model.Newsletter
                    : checkoutPage.Url + "?plan=" + (string)Session["PlanType"] + "&memberkey=" + member.Key + "&newsletter=" + model.Newsletter + "&landingpromo=" + Session["LandingPromoCode"];
                    return Redirect(redirectLink);
            }
            return Redirect((Session["LandingPromoCode"] == null) ? checkoutPage.Url + "?plan=" + (string)Session["PlanType"] : checkoutPage.Url + "?plan=" + (string)Session["PlanType"] + "&landingpromo=" + Session["LandingPromoCode"]);
        }

        [HttpPost]
        public JsonResult ValidateEmail(string email)
        {

            Code.Model.ValidationResponse response = Code.Class.Emailing.ValidateEmail(email);

            if (!response.IsValid)
            {
                return Json(response.Message);
            }

            return Json(true);
        }

        [HttpPost]
        public ActionResult ExtendTab(int tab)
        {
            switch (tab)
            {
                case 0:
                    return Json("");
                case 1:
                    return (Session["PlanType"] != null) ? Json($"?plan={Session["PlanType"]}") : Json(false);
                case 2:
                    return (Session["memberkey"] != null) ? Json($"?plan={Session["PlanType"]}&memberkey={Session["memberkey"]}&newsletter={Session["UserNewsletter"]}") : Json(false);
                default:
                    return Json("");
            }
        }

        [HttpPost]
        public ActionResult SavePaymentSession(int amount, string type)
        {
            Session["PlanAmount"] = amount;
            Session["PlanType"] = type;
            return Json(true);
        }


        [HttpPost]
        public ActionResult SaveUserSession(string email, string password, string phone, string username, string cvr, string landingpromo)
        {
            Session["UserEmail"] = email;
            Session["UserPassword"] = password;
            Session["UserPhone"] = phone;
            Session["LandingPromoCode"] = landingpromo;
            if (!string.IsNullOrWhiteSpace(username))
            {
                Session["UserCompany"] = username;
            }

            if (!string.IsNullOrWhiteSpace(cvr))
            {
                Session["UserCvr"] = cvr;
            }

            return Json(true);
        }
    }

}