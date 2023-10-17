using StartupCentral.Code.Class;
using StartupCentral.Code.Model;
using System;
using System.Reflection;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;
using System.Configuration;
using System.Net;
using System.Text;
using System.Web;
using System.Net.Http;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using RestSharp.Extensions;
using System.Data.Entity.Validation;
using System.Threading.Tasks;

namespace StartupCentral.Code.ApiControllers
{
    [AllowCrossSite]
    public class UpodiApiController : UmbracoApiController
    {
        private static readonly string AccessKey = ConfigurationManager.AppSettings["StartupCentral.Upodi.AccessKey"];
        readonly UserMessageService userMessagService;
        readonly private UpodiWrapperService upodiWrapper;

        public UpodiApiController()
        {
            userMessagService = new UserMessageService();
            upodiWrapper = new UpodiWrapperService();
        }


        [HttpGet]
        public ApiResponse SendMail()
        {
            var email = "aksel@vaffelbaronerne.dk"; 
            IMember member = Services.MemberService.GetByEmail(email);

            /*IMember member = Services.MemberService.GetByKey(UserId);
            if (member != null && member.ContentType.Alias.ToLower() == "student")
            {
                string newPassword = Passwords.GenerateNewPassword();
                Services.MemberService.SavePassword(member, newPassword);
                Emailing.SendApprovvalEmail(member, newPassword, true);
            }*/
            return new ApiResponse(true, "");
        }

        [HttpPost]
        public async Task<ApiResponse> SavePaymentMethod(PaylikeCreditCardResult model)
        {
            Logger.Warn(MethodBase.GetCurrentMethod().DeclaringType, model.ToString());
            try
            {
                if(model.MemberNodeId == null || model.MemberNodeId == 0)
                {
                    SetMemberNodeIdFromUserId(model);
                    
                }
                if (model.UserId.Contains("000"))
                    {
                    var member1 = Services.MemberService.GetById(model.MemberNodeId);
                    model.UserId = member1.Key.ToString();
                }
                Guid ProductPlanId = upodiWrapper.АssginProductPlanId(model.SubscriptionType); //new impl ProductPlanId

                Guid guid = await upodiWrapper.SavePaymentMethod(model.MemberNodeId, model.Token, model.PromoKode, ProductPlanId); //new impl ProductPlanId
                IMember member = Services.MemberService.GetById(model.MemberNodeId);
                if (member != null && member.ContentType.Alias.ToLower() == "student" && !member.IsApproved)
                {
                    member.IsApproved = true;
                    Services.MemberService.Save(member, true);
                    //string newPassword = Passwords.GenerateNewPassword();
                    //Services.MemberService.SavePassword(member, newPassword);
                    Emailing.SendApprovvalEmail(member, "", true);
                }
                else if(member != null && member.ContentType.Alias.ToLower() == "partner" && !member.IsApproved)
                {
                    member.IsApproved = true;
                    Services.MemberService.Save(member, true);
                    Emailing.SendApprovvalEmail(member, "", true);
                }

                return new ApiResponse(true, guid.ToString());
            }
            catch (Exception ex)
            {
                Logger.Warn(MethodBase.GetCurrentMethod().DeclaringType, 
                    string.Format("UpodiApiController.SavePaymentMethod Exception {0}", (object)ex.Message));
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ApiResponse> SavePaymentLounge(PaylikeCreditCardResult model)
        {
            Logger.Warn(MethodBase.GetCurrentMethod().DeclaringType, model.ToString());
            try
            {
                if (model.MemberNodeId == null || model.MemberNodeId == 0)
                {
                    SetMemberNodeIdFromUserId(model);

                }
                if (model.UserId.Contains("000"))
                {
                    var member1 = Services.MemberService.GetById(model.MemberNodeId);
                    model.UserId = member1.Key.ToString();
                }
                Guid ProductPlanId = upodiWrapper.АssginProductPlanId(model.SubscriptionType); //new impl ProductPlanId

                IMember member = Services.MemberService.GetById(model.MemberNodeId);
                Guid guid = await upodiWrapper.SavePaymentMethodFromLounge(model.MemberNodeId, model.Token, model.PromoKode, ProductPlanId, member); //new impl ProductPlanId
                if (member != null && member.ContentType.Alias.ToLower() == "student" && !member.IsApproved)
                {
                    member.IsApproved = true;
                    member.SetValue("isFreeLoungeUser", false);
                    Services.MemberService.Save(member, true);
                    //string newPassword = Passwords.GenerateNewPassword();
                    //Services.MemberService.SavePassword(member, newPassword);
                    Emailing.SendApprovvalEmail(member, "", true);
                }
                else if (member != null && member.ContentType.Alias.ToLower() == "partner" && !member.IsApproved)
                {
                    member.IsApproved = true;
                    member.SetValue("isFreeLoungeUser", false);
                    Services.MemberService.Save(member, true);
                    Emailing.SendApprovvalEmail(member, "", true);
                }
                return new ApiResponse(true, guid.ToString());
            }
            catch (Exception ex)
            {
                Logger.Warn(MethodBase.GetCurrentMethod().DeclaringType,
                    string.Format("UpodiApiController.SavePaymentMethod Exception {0}", (object)ex.Message));
                throw ex;
            }
        }


        [HttpPost]
        public ApiResponse GetMemberKey()
        {
            try
            {
                string userId = HttpContext.Current.Request.Form["UserId"];
                int id = int.Parse(userId);
                IMember member = Services.MemberService.GetById(id);
                if (member != null)
                {
                    return new ApiResponse(true, member.Key.ToString());
                }
                else
                {
                    return new ApiResponse(false, "Member not found");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(MethodBase.GetCurrentMethod().DeclaringType,
                    string.Format("UpodiApiController.GetMemberKey Exception {0}", (object)ex.Message));
                throw ex;
            }
        }
        [HttpPost]
        public void WebhookMethod(UpodiWebhooksModel model)
        {
            Logger.Warn(MethodBase.GetCurrentMethod().DeclaringType, model.ToString());
        }

        [HttpGet]
        public CreditCard GetPaymentMethod(String memberKey)
        {
            var member = Services.MemberService.GetByKey(Guid.Parse(memberKey));

            return upodiWrapper.GetCreditCard(member.Id);
        }

        
        [HttpPost]
        public async Task<ApiResponse> ReSavePaymentMethod(PaylikeCreditCardResult model)
        {
            SetMemberNodeIdFromUserId(model);
            Logger.Warn(MethodBase.GetCurrentMethod().DeclaringType, model.ToString());
            try
            {
                Guid ProductPlanId = upodiWrapper.АssginProductPlanId(model.SubscriptionType); //new impl ProductPlanId

                Guid guid = await upodiWrapper.ReSavePaymentMethod(model.MemberNodeId, model.Token, model.PromoKode, ProductPlanId); //new impl ProductPlanId
                IMember member = Services.MemberService.GetById(model.MemberNodeId);
                //IMember member = Services.MemberService.GetById(model.MemberNodeId);
                //Guid guid = await upodiWrapper.ReSavePaymentMethod(model.MemberNodeId, model.Token, member, ProductPlanId); //new impl ProductPlanId
                return new ApiResponse(true, guid.ToString());
            }
            catch (Exception ex)
            {
                Logger.Warn(MethodBase.GetCurrentMethod().DeclaringType,
                    string.Format("UpodiApiController.ReSavePaymentMethod Exception {0}", ex.Message));
                throw ex;
            }
        }

        [HttpGet]
        public ApiResponse UnsubscribeUser(int UserId)
        {
            Logger.Warn(MethodBase.GetCurrentMethod().DeclaringType, UserId.ToString());
            try
            {
                var member = Services.MemberService.GetById(UserId);
                if (member == null && member.ContentType.Alias.ToLower() == "student")
                {
                    return new ApiResponse(false, "User not found");
                }
                else
                {
                    member.SetValue("unsubscribeDate", DateTime.Now.AddMonths(1));
                    Services.MemberService.Save(member);
                    Emailing.CancelSubscriptionEmail(member, DateTime.Now.AddMonths(1));
                    return new ApiResponse(true, "User will be unsubscribed");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(MethodBase.GetCurrentMethod().DeclaringType,
                    string.Format("UpodiApiController.UnsubscribeUser Exception {0}", ex.Message));
                throw ex;
            }
        }

        [HttpGet]
        public ApiResponse DownloadInvoiceAsync(string InvoicesId)
        {
            try
            {
                String Url = "https://api.upodi.io/v3/invoices/{" + InvoicesId + "}/getpdfwithid";
                string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(AccessKey));

                HttpWebRequest fileReq = (HttpWebRequest)HttpWebRequest.Create(Url);
                fileReq.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + base64String);

                var resp = fileReq.GetResponse();
           
                Stream input = resp.GetResponseStream();
                byte[] data = input.ReadAsBytes();

                return new ApiResponse(true, "Downloadet til skrivebordet.", data);
            }
            catch (Exception ex)
            {
                Logger.Warn(MethodBase.GetCurrentMethod().DeclaringType,
                    string.Format("UpodiApiController.DownloadInvoice Exception {0}", ex.Message));
                throw ex;

                //return new ApiResponse(false, ex.ToString()); 
            }
        }

        //4th of Sept 2020 - GGesheva
        [HttpPost]
        public async Task<ApiResponse> ActivateSubscription(PaylikeCreditCardResult model)
        {
            SetMemberNodeIdFromUserId(model);
            Logger.Warn(MethodBase.GetCurrentMethod().DeclaringType, model.ToString());
            try
            {
                IMember member = Services.MemberService.GetById(model.MemberNodeId);

                if (member != null && member.ContentType.Alias.ToLower() == "student" && !member.IsApproved)
                {
                    int res = await upodiWrapper.ReActivateSubscription(model, member);

                    switch (res)
                    {
                        case 1:
                            PurgeMember(member);
                            await FirstLoginMessage(member.Id);
                            return new ApiResponse(true, "Subscription will be renewed.");
                        case 2:
                            PurgeMember(member);

                            //string newPassword = Passwords.GenerateNewPassword();
                            //Services.MemberService.SavePassword(member, newPassword);
                            //Emailing.SendApprovvalEmail(member, "", true);

                            await FirstLoginMessage(member.Id);
                            return new ApiResponse(true, "New subscription is created.");
                        default:
                            PurgeMember(member);

                            await FirstLoginMessage(member.Id);
                            return new ApiResponse(true, "New Upodi customer was created and assigned a subscription.");
                    }
                }
                else 
                {
                    return new ApiResponse(false, "Umbraco student user not found.");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(MethodBase.GetCurrentMethod().DeclaringType,
                    string.Format("UpodiApiController.ActivateSubscription Exception {0}", ex.Message));
                throw ex;
            }

        }

        [HttpPost]
        public ApiResponse ResumeSubscription()
        {
            try
            {
                IMember memb = TokenValidator.GetCurrentMember(HttpContext.Current.Request);

                if (upodiWrapper.resumeSubscription(memb)) {

                    Services.MemberService.DissociateRole(memb.Email, "student paused");
                    Services.MemberService.AssignRole(memb.Email, "student");

                    return new ApiResponse(true, "Subscription resumed.");
                }
                else
                {
                    return new ApiResponse(false, "Couldn't resume subscription.");
                }      
            }
            catch (Exception ex)
            {
                Logger.Warn(MethodBase.GetCurrentMethod().DeclaringType,
                    string.Format("UpodiApiController.ActivateSubscription Exception {0}", ex.Message));
                throw ex;
            }        
        }

        private void PurgeMember(IMember member)
        {
            member.SetValue("unsubscribeDate", null);
            member.IsApproved = true;
            Services.MemberService.Save(member, true);
        }

        private void SetMemberNodeIdFromUserId(PaylikeCreditCardResult model)
        {
            var customer = Services.MemberService.GetByKey(Guid.Parse(model.UserId));
            model.MemberNodeId = customer.Id;
        }

        public async Task FirstLoginMessage(int toMemberId)
        {
            try
            {
                Besked model = new Besked();
                IMember fromMember = Services.MemberService.GetById(30823);
                IMember toMember = Services.MemberService.GetById(toMemberId);
                string memberName = toMember.Name;
                string link = "https://startupcentral.dk/media/3u4f50an/f%C3%A5-succes-med-dit-medlemskab-guide.pdf";
                model.besked =
                                $" Hej {memberName} " +
                                "<p>Først og fremmest velkommen til Startup Central</p>" +
                                "<p>Vi er meget glade for, at du er en del af os og vi glæder os til at følge din iværksætterrejse.</p>" +
                                "<p> For at få mest ud af dit medlemskab, så handler det om, at du opsøger hjælp.</p>" +
                                "<p>Skriv i Startup Lounge og få hjælp af andre selvstændige, de kan eksempelvis hjælpe dig med at undgå begynder fejl.</p>" +
                                " <p> Ræk ud til vores coaches, hvis du har brug for, at en specialist kigger på din konkrete sag." +
                                " De vil kunne hjælpe dig med stort og småt, så tøv ikke med at række ud til dem.</p>" +
                                " <p>Der er mange omkostninger, når man starter og driver virksomhed." +
                                " Derfor har vi selvfølgelig forhandlet priserne i bund for de programmer og services, som er en nødvendighed i dit iværksætteri.</p>" +
                                " <p>Gør brug af skabelonerne for at skabe et overblik over din virksomhed både økonomisk og strategisk." +
                                " Der er en masse viden at hente på platformen; vlogs, blogs, podcast, Startup Wiki osv.</p>" +
                                "<p>Vil du have et større overblik over, hvordan dit medlemskab kan skabe mest værdi for dig og virksomhed," +
                                $" så kan du læse succesguiden her: <a href =\"{link}\"> Medlemskab Guide </a></p>" +
                                "<p>Endnu en gang stort velkommen</p>" +
                                "<p> Denne besked kan ikke besvares.</p>" +
                                "<p>Mange rakethilsner</p>" +
                                "<p>Startup Central</p>";
                if (fromMember != null && toMember != null)
                {
                    var msgid = await userMessagService.Save(fromMember.Key, toMember.Key, model);
                    Emailing.SendNewMessageNotification(fromMember, toMember);
                    LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"SendBesked ok");
                    Console.WriteLine("Welcome Message was sent.");
                }
                else
                {
                    Console.WriteLine("Sender or receiver couldn't be found!");
                }


            }
            catch (DbEntityValidationException ex)
            {
                string err = string.Empty;
                foreach (var eve in ex.EntityValidationErrors)
                {
                    err = err + string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        err = err + string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"SendBesked error {err}");
                Console.WriteLine($"SendBesked sent error {err}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"SendBesked sent error {e.Message}");
            }
        }

    }
}