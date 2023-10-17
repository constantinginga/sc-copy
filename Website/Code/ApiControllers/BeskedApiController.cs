
using StartupCentral.Code.Class;
using StartupCentral.Code.Model;
using StartupCentral.DAL.EntityModels;
using StartupCentral.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;
using System;
using System.Data.Entity.Validation;
using Umbraco.Web;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace StartupCentral.Code.ApiControllers
{
    public class BeskedApiController : UmbracoApiController
    {
        readonly UserMessageService userMessagService;
       // private const string allowedMembers = "31957, 26886, 26502";

        public BeskedApiController()
        {
            userMessagService = new UserMessageService();
        }

        [HttpGet]
        public InvestorSvar GetInvestorSvar(int id, int modtagerId = 0)
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (member != null)
            {
                if (member.ContentTypeAlias == "investor" || member.ContentTypeAlias == "coach1")
                {
                    IContent content = Services.ContentService.GetById(id);
                    if (content != null)
                    {
                        InvestorSvar investorSvar = new InvestorSvar();
                        investorSvar.Overskrift = content.GetValue<string>("wwoverskrift");
                        investorSvar.StandardSvar = content.GetValue<string>("standardsvar");
                        investorSvar.Overskrift = investorSvar.Overskrift.Replace("{{investor-navn}}", member.GetValue<string>("wwname"));
                        investorSvar.StandardSvar = investorSvar.StandardSvar.Replace("{{investor-navn}}", member.GetValue<string>("wwname"));

                        investorSvar.Overskrift = investorSvar.Overskrift.Replace("{{coach-navn}}", member.GetValue<string>("wwname"));
                        investorSvar.StandardSvar = investorSvar.StandardSvar.Replace("{{coach-navn}}", member.GetValue<string>("wwname"));
                        investorSvar.AfsenderNavn = member.GetValue<string>("wwname");

                        if (modtagerId > 0)
                        {
                            IMember modtager = Services.MemberService.GetById(modtagerId);
                            if (modtager != null)
                            {
                                investorSvar.Overskrift = investorSvar.Overskrift.Replace("{{modtager-navn}}", modtager.GetValue<string>("wwname"));
                                investorSvar.StandardSvar = investorSvar.StandardSvar.Replace("{{modtager-navn}}", modtager.GetValue<string>("wwname"));
                                investorSvar.ModtagerNavn = modtager.GetValue<string>("wwname");
                            }
                        }

                        return investorSvar;
                    }
                }
            }

            return null;
        }


        private IMember GetMember() => TokenValidator.GetCurrentMember(HttpContext.Current.Request);


        [HttpGet]
        public async Task<ApiResponse> EmailingAllStudents()
        {
            try
            {
                Besked model = new Besked();
                IMember fromMember = Services.MemberService.GetById(30823);
                //IMember member = Services.MemberService.GetById(31957);
                var toMembers = Services.MemberService.GetMembersInRole("student").ToList();

                string memberName = "";
                 int count = 0;
                 var watch = System.Diagnostics.Stopwatch.StartNew();
                if (toMembers.Count <= 0)
                {
                    return new ApiResponse(false, $"Error!");
                }
                
                foreach (var member in toMembers)
                {
                    if (member != null)
                    {
                        var messageBody = Umbraco.Content(33623).Value<string>("messageToStudents");
                        memberName = member.Name.Split(' ').First();
                        messageBody = messageBody.Replace("{{fornavn}}", memberName);
                        //check msg does not exist
                        var msgs = await userMessagService.GeMessages(member.Key, fromMember.Key);
                        var exists = msgs.Any(x => x.Body.Equals(messageBody));
                        if (!exists)
                        {

                            model.besked = messageBody; 
                            count++;
                            model.toMemberId = member.Id;
                            await userMessagService.Save(fromMember.Key, member.Key, model);
                            Emailing.SendNewMessageNotification(fromMember, member);

                            var elapsedMs = watch.ElapsedMilliseconds;

                            while (count >= 55)
                            {
                                if (elapsedMs < 60000)
                                    Task.Delay(2000).Wait();

                                count = 0;
                                elapsedMs = 0;
                            }
                        }

                                    
                                    
                    }
                }                   


                     watch.Stop();             
                return new ApiResponse(true, $"Beskeder er sent til alle");

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
                return new ApiResponse(false, $"SendBesked sent error {err}");
            }
            catch (Exception e)
            {
                return new ApiResponse(false, $"SendBesked sent error {e.Message}");
            }
        }
        [HttpGet]
        public async Task<ApiResponse> EmailingAllCoaches()
        {
            try
            {
                Besked model = new Besked();
                IMember fromMember = Services.MemberService.GetById(30823);
                //IMember member = Services.MemberService.GetById(31957);
                var toMembers = Services.MemberService.GetMembersInRole("coach1").ToList();

                string memberName = "";
                int count = 0;
                var watch = System.Diagnostics.Stopwatch.StartNew();
                if (toMembers.Count <= 0)
                {
                    return new ApiResponse(false, $"Error!");
                }

                foreach (var member in toMembers)
                {
                    
                    if (member != null)
                    {
                        var messageBody = Umbraco.Content(33623).Value<string>("messageToCoaches");
                        memberName = member.Name.Split(' ').First();
                        messageBody = messageBody.Replace("{{fornavn}}", memberName);
                        //check msg does not exist
                        var msgs = await userMessagService.GeMessages(member.Key, fromMember.Key);
                        var exists = msgs.Any(x => x.Body.Equals(messageBody));

                        if (!exists)
                        {
                            
                                model.besked = messageBody;
                                count++;
                                model.toMemberId = member.Id;
                                await userMessagService.Save(fromMember.Key, member.Key, model);
                                Emailing.SendNewMessageNotification(fromMember, member);

                                var elapsedMs = watch.ElapsedMilliseconds;

                                while (count >= 55)
                                {
                                    if (elapsedMs < 60000)
                                        Task.Delay(2000).Wait();

                                    count = 0;
                                    elapsedMs = 0;
                                }
                        }



                    }
                }


                watch.Stop();
                return new ApiResponse(true, $"Beskeder er sent til alle");

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
                return new ApiResponse(false, $"SendBesked sent error {err}");
            }
            catch (Exception e)
            {
                return new ApiResponse(false, $"SendBesked sent error {e.Message}");
            }
        }
        [HttpPost]
        public async Task<ApiResponse> SendBesked(Besked model)
        {
            try
            {
                IMember fromMember = GetMember();
                IMember toMember = Services.MemberService.GetById(model.toMemberId);
                
                if (fromMember != null && toMember != null)
                {
                    var msgid = await userMessagService.Save(fromMember.Key, toMember.Key, model);
                    await Emailing.SendNewMessageNotification(fromMember, toMember);
                    LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"SendBesked ok");

                    return new ApiResponse(true, "Beskeden er sendt.", msgid);
                }
                else
                {
                    return new ApiResponse(false, $"SendBesked member is empty.");
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
                return new ApiResponse(false, $"SendBesked sent error {err}");
            }
            catch (Exception e)
            {
                return new ApiResponse(false, $"SendBesked sent error {e.Message}");
            }
                 //return new ApiResponse(false, "Beskeden kunne ikke sendes.");
        }

        [HttpPost]
        public async Task<ApiResponse> SendAttachmentsToInvestor(BeskedWithPlane model)
        {
            IMember fromMember = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (fromMember != null)
            {
                IMember toMember = Services.MemberService.GetById(model.investorId);
                //IContent plan = Services.ContentService.GetById(model.planId);

                if (toMember != null) //&& plan!=null)
                {
                    string planValue = model.planId.ToString();
                    string pitchValue = model.pitchId.ToString();

                    if (planValue.Equals("00000000-0000-0000-0000-000000000000"))
                    {
                        await userMessagService.SaveMsgWithPitch(fromMember.Key, toMember.Key, model,  model.pitchId);

                    }
                    else if (pitchValue.Equals("00000000-0000-0000-0000-000000000000"))
                    {
                        await userMessagService.SaveMsgWithPlan(fromMember.Key, toMember.Key, model, model.planId);

                    }
                    else
                    {
                        await userMessagService.Save(fromMember.Key, toMember.Key, model, model.planId, model.pitchId);

                    }
                    await Emailing.SendNewPlanNotification(fromMember, toMember);
                    return new ApiResponse(true, "Beskeden er sendt.");
                }
            }
            return new ApiResponse(false, "Beskeden kunne ikke sendes."); 
        }


        [HttpGet]
        public async Task<ApiResponse> GetRecipients()
        {
            IMember member = GetMember();
            if (member != null)
            {
                return new ApiResponse(true, "recipients", (await userMessagService.GetRecipientKeys(member.Key))
                                                                                    .Select(x => Services.MemberService.GetByKey(x))
                                                                                    .Where(x => x != null).Select(x => new RecipientsDTO(x)));
            }
            return new ApiResponse(false, "recipients");
        }

        [HttpGet]
        public async Task<ApiResponse> GeMessages(int fromMemberId)
        {
            IMember member = GetMember();
            IMember fromMember = Services.MemberService.GetById(fromMemberId);

            if (member != null && fromMember !=null)
            {
                RecipientsDTO memberDto = new RecipientsDTO(member);
                RecipientsDTO memberFromToDto = new RecipientsDTO(fromMember);
                var res = await userMessagService.GeMessages(member.Key, fromMember.Key);
                var unreadMessages = res.Where(x => x.ToUser == member.Key && x.FromUser == fromMember.Key && !x.Read).ToList();

                if(unreadMessages.Count > 0)
                {
                    await userMessagService.MarkRead(unreadMessages);
                }

                foreach (var msg in res)
                {
                    if(fromMemberId!= 30823)
                    {
                        msg.Body = Regex.Replace(msg.Body, @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)",
                        "<a target='_blank' href='$1'>Link</a>");
                    }
                }

                return new ApiResponse(true, "messages fetched and marked as seen", res.Select(x=>new MessagDTO(x, memberDto, memberFromToDto)));
            }
            return new ApiResponse(false, "messages");

        }

        [HttpGet]
        public async Task<ApiResponse> GetLatestMsgFromConversation(int fromMemberId)
        {
            IMember member = GetMember();
            IMember fromMember = Services.MemberService.GetById(fromMemberId);

            if(member != null && fromMember != null)
            {
                var res = await userMessagService.GeMessages(member.Key, fromMember.Key);
                return new ApiResponse(true, "messages", res.Last());
     
            }
            return new ApiResponse(false, "messages");
        }

        [HttpGet]
        public async Task<ApiResponse> AnyUnreadGroupMessages()
        {
            IMember member = GetMember();
            HttpClient client = new HttpClient();
            HttpResponseMessage response = 
                await client.GetAsync("https://api.startupcentral.dk/Notifications/GetMemberNotificationsCount?memberId="
                + member.Id.ToString("D"));

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<string>();
                if (result == "0") 
                    return new ApiResponse(true, "Unread messages", "");
                return new ApiResponse(true, "Unread messages", result.ToString());
            }
            return new ApiResponse(true, "Unread messages", "");
        }

        [HttpGet]
        public async Task<ApiResponse> AnyUnreadMessages()
        {
            IMember member = GetMember();
            if (member != null)
            {
                var unreadList = await userMessagService.GetUnreadMessages(member.Key);
                return new ApiResponse(true, "Unread messages", unreadList);
            }
            return new ApiResponse(false, "Member not found");
        }

        [HttpGet]
        public async Task<ApiResponse> MarkRead(int fromMemberId)
        {

            IMember member = GetMember();
            IMember fromMember = Services.MemberService.GetById(fromMemberId);

            if (member != null)
            {
                if (fromMember != null)
                {
                    await userMessagService.MarkRead(member.Key, fromMember.Key);
                }
                else
                {
                    return new ApiResponse(false, $"FromMember with Id {fromMember.Id} could not be loaded.");
                }
            }
            return new ApiResponse(true, "No work to be done.");
        }
        
        [HttpPost]
        public async Task<ApiResponse> Migrate()
        {
            return new ApiResponse(true, "");
            int beskedRootFolderId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["StartupCentral.Beskeder.RootFolder.Id"]);
            const int pageSize = 8000;
            IEnumerable<IContent> beskeder = ApplicationContext.Current.Services.ContentService.GetPagedChildren(beskedRootFolderId, 0, pageSize, out long totalRecords);
            if(totalRecords == pageSize)
                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"{pageSize} pageSize output reached - risk of unhandled beskeder");

            int i = 0;
            var beskederDb = beskeder.ToList().Select(besked => 
            {
                try
                {

                var userMessagesDb = new userMessages();
                userMessagesDb.Id = besked.Key;
                IMember wwfrom = null;
                if (besked.GetValue("wwfrom").ToString().Contains("umb://"))
                {
                    wwfrom = ApplicationContext.Current.Services.MemberService.GetById(besked.GetValue("wwfrom").ToString().GetIdByUdi());
                }
                else if (besked.GetValue("wwfrom").ToString().IsNumeric())
                {
                    wwfrom = ApplicationContext.Current.Services.MemberService.GetById(Convert.ToInt32(besked.GetValue("wwfrom").ToString()));
                }
                if (wwfrom == null)
                    //throw new Exception("wwfrom == null");
                    i++;
                else
                    userMessagesDb.FromUser = wwfrom.Key;

                IMember wwTo = null;
                if (besked.GetValue("wwto").ToString().Contains("umb://"))
                {
                    wwTo = ApplicationContext.Current.Services.MemberService.GetById(besked.GetValue("wwto").ToString().GetIdByUdi());
                }
                else if (besked.GetValue("wwto").ToString().IsNumeric())
                {
                    wwTo = ApplicationContext.Current.Services.MemberService.GetById(Convert.ToInt32(besked.GetValue("wwto").ToString()));
                }
                if (wwTo == null)
                    i++;
                //throw new Exception("wwTo == null");
                else
                    userMessagesDb.ToUser = wwTo.Key;

                userMessagesDb.CreateDate = besked.CreateDate;
                var Read = true;                
                if (!string.IsNullOrEmpty(besked.GetValue<string>("wwread")))
                    Read = besked.GetValue<string>("wwread").ToString().ParseBoolean();
                userMessagesDb.Read = Read;
                if (!string.IsNullOrEmpty(besked.GetValue<string>("wwbody")))
                    userMessagesDb.Body = besked.GetValue<string>("wwbody").ToString();
                else
                    userMessagesDb.Body = string.Empty;

                string forretningsplan = string.Empty;
                if (besked.GetValue<string>("wwattachments") != null)
                {
                    if (besked.GetValue<string>("wwattachments").IsNumeric())
                    {
                            // https://our.umbraco.com/forum/umbraco-8/98319-umbraco-library-niceurlwithdomain-and-niceurl-in-umbraco-81
                            //forretningsplan = Umbraco.NiceUrl(besked.GetValue<int>("wwattachments"));
                            forretningsplan = Umbraco.Content(besked.GetValue<int>("wwattachments"))?.Url();
        }
                    else
                    {
                        forretningsplan = besked.GetValue<string>("wwattachments");
                    }
                }
                userMessagesDb.Attachment = forretningsplan;

                    //wwpitchAttachment
                    string pitch = string.Empty;
                    if (besked.GetValue<string>("wwpitchAttachment") != null)
                    {
                        if (besked.GetValue<string>("wwpitchAttachment").IsNumeric())
                        {
                            pitch = Umbraco.Content(besked.GetValue<int>("wwpitchAttachment"))?.Url();
                        }
                        else
                        {
                            pitch = besked.GetValue<string>("wwpitchAttachment");
                        }
                    }
                    userMessagesDb.PitchAttatchment = pitch;

                    return userMessagesDb;

                }
                catch (Exception)
                {

                    return null;
                }

            });


            //await userMessagService.migrate(beskeder);



            //.Where(itm => itm.GetValue<string>("wwto") == string.Concat("umb://member/", currentUser.GetKey().ToString().Replace("-", string.Empty)) || itm.GetValue<string>("wwto") == currentUser.Id.ToString() ||
            //itm.GetValue<string>("wwfrom") == string.Concat("umb://member/", currentUser.GetKey().ToString().Replace("-", string.Empty)) || itm.GetValue<string>("wwfrom") == currentUser.Id.ToString());
            var res = beskederDb.ToList();
            await userMessagService.migrate(res.Where(x=>x != null && x.FromUser != Guid.Empty && x.ToUser != Guid.Empty).ToList());
            return new ApiResponse(true, res.Count.ToString());
        }

        //[HttpGet]
        //public async Task<ApiResponse> BadPayersMessage()
        //{
        //    try
        //    {
        //        Besked model = new Besked();
        //        IMember fromMember = Services.MemberService.GetById(30823);
        //        //int toMemberId = 26502;
        //        //IMember member= Services.MemberService.GetById(toMemberId);
        //        List<IMember> toMembers = new List<IMember>(); //list of bad payers ids

        //        //List<string> emails = new List<string> { "mathiaspedersen1418@hotmail.com", "a.m.a.service@hotmail.com", "cn@global-gts.com", "info@cocktailchamps.com", "xtrrush@gmail.com", "vidriv@gmail.com", "Dpodennispoulsen@gmail.com", "erik_hohwu@hotmail.com",
        //        //"bedhubgrid@gmail.com", "ogogmontage@gmail.com", "joachim@ja-branding.dk", "alexander@lgaming.dk", "freja_bierregaard@live.dk", "marcusjohannsen2@icloud.com", "Mads@socialkey.dk",
        //        //"markblumeschmidt@gmail.com", "janningsmultiservice@gmail.com", "mjeentreprenor@gmail.com", "denniskildevej19@gmail.com", "Kontakt@ortmanmontage.dk", "sebastianaltmannjohansen@gmail.com",
        //        //"miken@sodyum.dk", "amir1966@outlook.dk", "contact@textmunchy.com", "kuus1999@hotmail.com", "rensvtmidt@gmail.com", "dennishoeg63@gmail.com", "adm.nostro.design@gmail.com", "bkochsinding@gmail.com", "cecilie.krogager@live.dk",
        //        //"R.O.Maskinstation@outlook.dk", "theistpa@hotmail.com", "mikejepsen1712@gmail.com", "dilankatrine@gmail.com", "jobnikolaj1@gmail.com", "BKBYG@outlook.dk", "webshop@dingnaver.dk", "info@clothingbyloos.dk", "rise@hofho.eu", "Kristiantingager@gmail.com",
        //        //"dani078d@gmail.com", "Jakob95gundersen@hotmail.com", "pm@copenhagencarebrands.com", "admin@we-lab.dk", "dahmsentreprise@gmail.com", "poulsencaro@gmail.com", "caveman.hairstyle@gmail.com", "amir.nagi01@gmail.com",
        //        //"rebal9797@gmail.com", "rmikkelsen6@gmail.com", "Info@ilovewallstickers.dk", "Nwsahl@gmail.com", "andrea.salskov.nielsen@gmail.com", "justabrick@hotmail.dk", "nikolaj_t@icloud.com", "saadiawhite@gmail.com",
        //        //"emilodi1994@gmail.com", "louis.lyager.poulsen@gmail.com" ,"nb@toemrerfirmaet-madsen.dk", "Bossbabesuits@gmail.com", "oha@cooplii.com", "nicolaj@danskdigital.com", "danielbennetsen69@gmail.com", "filipdantoft@gmail.com",
        //        //"jianmfn@icloud.com", "info@mikkelbarker.dk", "angelinvest@outlook.dk", "nbrservicedk@gmail.com", "oliverstorm117@protonmail.com", "osama@sailnow.dk", "sebastianknudsen@reusetomorrow.dk",
        //        //"mymessenger01@hotmail.com", "tomskov@gentlefaire.dk", "fb@adcase.dk", "kontakt@aleksander-rengoering.dk", "zb@startupcentral.dk"};
                
                
        //        List<string> emails = new List<string>(); //remove the line and uncomment the above line, with adding the emailes you want the message to be sent to 


        //        List<string> unem = new List<string>();
        //        foreach(string email in emails)
        //        {
        //            var mem = Services.MemberService.GetByEmail(email.ToLower());
        //            if(mem != null)
        //            {
        //                toMembers.Add(mem);
        //            }
        //            else
        //            {
        //                unem.Add(email);
        //            }
        //        }
        //        string memberName = "";
        //        int count = 0;
        //        var watch = System.Diagnostics.Stopwatch.StartNew();
        //        foreach (var member in toMembers)
        //        {

        //            if (member != null)
        //            {

        //                    memberName = member.Name;
        //                string message =
        //                    $" Hej {memberName} " +
        //                    "<p>Der er problemer med dit betalingskort.</p>" +
        //                    "<p> Du er tilmeldt automatisk kortbetaling, men denne gang kunne vi ikke trække den månedlige/kvartalsvise/årlige betaling" +
        //                    " på det betalingskort, som du har registeret hos os. Vi skal derfor bede dig om at betale din regning. Du finder din regning således: ”Min Profil” --> ”Indstillinger” --> ”Faktura”.</p>" +
        //                    "<p> Har du fået nyt betalingskort, bedes du opdatere dette under dine brugerindstillinger, så dine fremtidige regninger vil blive trukket automatisk. Sådan finder du dine brugerindstillinger: ”Min Profil” --> ”Indstillinger” -->  ”Brugerindstillinger”.</p>" +
        //                    " <p> Der kan gå et par dage, før vi kan registrere din betaling i vores systemer, så hvis du allerede har betalt, bedes du venligst se bort fra denne henvendelse.</p>" +
        //                    " <p><b>Brug for hjælp?</b></p>" +
        //                    " <p> Hvis du har spørgsmål eller brug for hjælp, kan du altid række ud til os. Vi sidder klar ved telefonen: <a href =\"tel:29290113\"> 29290113 </a> eller mailen: <a href =\"mailto:fo@startupcentral.dk\"> fo@startupcentral.dk </a></p>" +
        //                    "<p>Bemærk, at du ikke kan besvare denne besked.</p>" +
        //                    "<p>Mange rakethilsner</p>" +
        //                    "<p> Startup Central</p>";


        //                    model.besked = message; 
        //                    count++;
        //                    model.toMemberId = member.Id;
        //                    await userMessagService.Save(fromMember.Key, member.Key, model);
        //                    Emailing.SendNewMessageNotification(fromMember, member);

        //                var elapsedMs = watch.ElapsedMilliseconds;

        //                while (count >= 55)
        //                {
        //                    if (elapsedMs < 60000)
        //                        Task.Delay(2000).Wait();

        //                    count = 0;
        //                    elapsedMs = 0;
        //                }


        //            }
        //        }



        //        watch.Stop();

        //        return new ApiResponse(true, $"Message was sent to all bad payers");

        //    }
        //    catch (DbEntityValidationException ex)
        //    {
        //        string err = string.Empty;
        //        foreach (var eve in ex.EntityValidationErrors)
        //        {
        //            err = err + string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
        //            foreach (var ve in eve.ValidationErrors)
        //            {
        //                err = err + string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
        //            }
        //        }
        //        LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"SendBesked error {err}");
        //        return new ApiResponse(false, $"SendBesked sent error {err}");
        //    }
        //    catch (Exception e)
        //    {
        //        return new ApiResponse(false, $"SendBesked sent error {e.Message}");
        //    }
        //}

    }

    public class Dialog
    {

    }

    public class MessagDTO
    {
        public MessagDTO(userMessages DbModel, RecipientsDTO member1, RecipientsDTO member2)
        {
            this.Id = DbModel.Id;
            this.FromUser = DbModel.FromUser == member1.Key ? member1: member2;
            this.ToUser = DbModel.FromUser == member1.Key ? member1 : member2;
            this.Body = DbModel.Body;
            //02. Sep 2019 14:24:57 
            this.CreateDate = DbModel.CreateDate.ToString("dd. MMM yyyy") + " " + DbModel.CreateDate.ToString("HH:mm:ss");
            this.Attachment = "";
            this.PitchAttatchment = "";
            try
            {
                if (!string.IsNullOrEmpty(DbModel.Attachment))
                {
                    this.Attachment = DbModel.Attachment.Substring(15);
                }
                else
                { 
                    this.Attachment = ""; 
                }
                if (!string.IsNullOrEmpty(DbModel.PitchAttatchment))
                {
                    this.PitchAttatchment = DbModel.PitchAttatchment.Substring(15);
                }
                else
                {
                    this.PitchAttatchment = "";
                }


            }
            catch (Exception)
            {

            }
            this.Subject = DbModel.Subject;
            this.Read = DbModel.Read;
        }
        public System.Guid Id { get; private set; }
        public RecipientsDTO FromUser { get; private set; }
        public RecipientsDTO ToUser { get; private set; }
        public string Body { get; private set; }
        public string CreateDate { get; private set; }
        public string Attachment { get; private set; }
        public string PitchAttatchment { get; private set; }

        public string Subject { get; private set; }
        public bool Read { get; private set; }
    }

    public class RecipientsDTO
    {
        public int Id { get; set; }

        public Guid Key { get; set; }
        public string Name { get; set; }
        public int iAvatar { get; set; }   
        public string Avatar { get; set; }
        public string UserType { get; set; }

        public RecipientsDTO(IMember member)
        {
            Id = member.Id;
            Key = member.Key;
            Name = member.Name;
            iAvatar = 0;
            Avatar = "/media/1195/startup-central-bruger.jpg";
            UserType = member.ContentType.Name;
            // https://our.umbraco.com/forum/umbraco-8/96270-using-umbracohelper-in-a-custom-class-in-v8
            //UmbracoHelper helper = new UmbracoHelper(Umbraco.Web.UmbracoContext.Current);
            UmbracoHelper helper = Umbraco.Web.Composing.Current.UmbracoHelper; 

            try
            {
                if(member.GetValue<string>("wwavatar") != null)
                {
                    if(member.GetValue<string>("wwavatar").IsNumeric())
                    {
                        iAvatar = Convert.ToInt32(member.GetValue<string>("wwavatar"));
                    }
                    else if(!string.IsNullOrEmpty(member.GetValue<string>("wwavatar")))
                    {
                        iAvatar = member.GetValue<string>("wwavatar").GetIdByUdi();
                    }
                }

                if(iAvatar>0)
                {
                    Avatar = helper.Media(iAvatar).Url;
                }
            }
            catch (System.Exception)
            {

            }
        }
    }
}


/*
public class BeskedApiController : UmbracoApiController
{
    [HttpGet]
    public InvestorSvar GetInvestorSvar(int id, int modtagerId = 0)
    {
        IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
        if (member != null)
        {
            if (member.ContentTypeAlias == "investor" || member.ContentTypeAlias == "coach1")
            {
                IContent content = Services.ContentService.GetById(id);
                if (content != null)
                {
                    InvestorSvar investorSvar = new InvestorSvar();
                    investorSvar.Overskrift = content.GetValue<string>("wwoverskrift");
                    investorSvar.StandardSvar = content.GetValue<string>("standardsvar");
                    investorSvar.Overskrift = investorSvar.Overskrift.Replace("{{investor-navn}}", member.GetValue<string>("wwname"));
                    investorSvar.StandardSvar = investorSvar.StandardSvar.Replace("{{investor-navn}}", member.GetValue<string>("wwname"));

                    investorSvar.Overskrift = investorSvar.Overskrift.Replace("{{coach-navn}}", member.GetValue<string>("wwname"));
                    investorSvar.StandardSvar = investorSvar.StandardSvar.Replace("{{coach-navn}}", member.GetValue<string>("wwname"));
                    investorSvar.AfsenderNavn = member.GetValue<string>("wwname");

                    if (modtagerId > 0)
                    {
                        IMember modtager = Services.MemberService.GetById(modtagerId);
                        if (modtager != null)
                        {
                            investorSvar.Overskrift = investorSvar.Overskrift.Replace("{{modtager-navn}}", modtager.GetValue<string>("wwname"));
                            investorSvar.StandardSvar = investorSvar.StandardSvar.Replace("{{modtager-navn}}", modtager.GetValue<string>("wwname"));
                            investorSvar.ModtagerNavn = modtager.GetValue<string>("wwname");
                        }
                    }

                    return investorSvar;
                }
            }
        }

        return null;
    }

    [HttpPost]
    public ApiResponse SendForretningsPlanToInvestor(BeskedWithPlane model)
    {
        //int planId, int investorId, string besked
        IMember fromMember = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
        if (fromMember != null)
        {
            int beskedRootFolderId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["StartupCentral.Beskeder.RootFolder.Id"]);
            IContent beskedRootFolder = Services.ContentService.GetById(beskedRootFolderId);
            IContent plan = Services.ContentService.GetById(model.planId);
            if (beskedRootFolder != null && plan != null)
            {
                IContent messi = Services.ContentService.CreateContentWithIdentity($"Ny forretningsplan: {plan.GetValue<string>("wwprojektnavn")}", beskedRootFolder, "besked");

                if (messi != null)
                {
                    IMember toMember = Services.MemberService.GetById(model.investorId);
                    if (toMember != null)
                    {
                        messi.SetValue("wwto", $"umb://member/{toMember.Key.ToString().Replace("-", string.Empty)}");
                        messi.SetValue("wwfrom", $"umb://member/{fromMember.Key.ToString().Replace("-", string.Empty)}");
                        messi.SetValue("wwsubject", $"Ny forretningsplan: {plan.GetValue<string>("wwprojektnavn")}");
                        messi.SetValue("wwbody", model.besked);
                        messi.SetValue("wwattachments", $"umb://document/{plan.Key.ToString().Replace("-", string.Empty)}");
                        messi.SetValue("wwread", false);
                        messi.SetValue("wwanswered", false);

                        Services.ContentService.SaveAndPublishWithStatus(messi);
                        Emailing.SendNewPlanNotification(fromMember, toMember);
                    }

                    return new ApiResponse(true, "Beskeden er sendt.");
                }
            }
        }

        return new ApiResponse(false, "Beskeden kunne ikke sendes.");
    }

    [HttpPost]
    public ApiResponse SendBesked(Besked model)
    {
        IMember fromMember = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
        if (fromMember != null)
        {
            int beskedRootFolderId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["StartupCentral.Beskeder.RootFolder.Id"]);
            IContent beskedRootFolder = Services.ContentService.GetById(beskedRootFolderId);

            if (beskedRootFolder != null)
            {
                string emne = string.IsNullOrEmpty(model.emne) ? $"Besked" : $"Besked: {model.emne}";
                IContent messi = Services.ContentService.CreateContentWithIdentity(emne, beskedRootFolder, "besked");

                if (messi != null)
                {
                    IMember toMember = Services.MemberService.GetById(model.toMemberId);

                    if (toMember != null)
                    {
                        messi.SetValue("wwto", $"umb://member/{toMember.Key.ToString().Replace("-", string.Empty)}");
                        messi.SetValue("wwfrom", $"umb://member/{fromMember.Key.ToString().Replace("-", string.Empty)}");
                        messi.SetValue("wwsubject", emne);
                        messi.SetValue("wwbody", model.besked);
                        messi.SetValue("wwread", false);
                        messi.SetValue("wwanswered", false);

                        Services.ContentService.SaveAndPublishWithStatus(messi);

                        Emailing.SendNewMessageNotification(fromMember, toMember);
                    }

                    return new ApiResponse(true, "Beskeden er sendt.");
                }
            }
        }

        return new ApiResponse(false, "Beskeden kunne ikke sendes.");
    }


    [HttpGet]
    public ApiResponse AnyUnreadMessages()
    {
        List<int> unreadList = new List<int>();

        IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
        if (member != null)
        {
            int beskedRootFolderId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["StartupCentral.Beskeder.RootFolder.Id"]);
            Node rootNode = new Node(beskedRootFolderId);
            if (rootNode != null)
            {
                foreach (Node besked in rootNode.Children)
                {
                    if (besked.GetProperty("wwto") != null && (besked.GetProperty("wwto").Value.ToString() == member.Id.ToString() || besked.GetProperty("wwto").Value == $"umb://member/{member.Key.ToString().Replace("-", string.Empty)}"))
                    {
                        if (besked.GetProperty("wwread") != null && !besked.GetProperty("wwread").ToString().ParseBoolean())
                        {
                            unreadList.Add(besked.Id);
                        }
                        else if (besked.GetProperty("wwread") == null)
                        {
                            unreadList.Add(besked.Id);
                        }
                        System.Diagnostics.Debug.WriteLine(besked.GetProperty("wwread"));
                    }
                }
            }

            return new ApiResponse(true, "Unread messages", unreadList);
        }

        return new ApiResponse(false, "Member not found");
    }

    //[HttpGet]
    //public ApiResponse MarkAllAsRead()
    //{
    //    ApiResponse unreadApi = AnyUnreadMessages();
    //    if (unreadApi != null)
    //    {
    //        if (unreadApi.Data != null)
    //        {
    //            List<int> unreadList = (List<int>)unreadApi.Data;
    //            if (unreadList != null && unreadList.Count > 0)
    //            {
    //                foreach(int Id in unreadList)
    //                {
    //                    IContent content = Services.ContentService.GetById(Id);
    //                    if (content != null)
    //                    {
    //                        if (content.HasProperty("wwread"))
    //                        {
    //                            content.SetValue("wwread", true);
    //                            Services.ContentService.SaveAndPublishWithStatus(content);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    return new ApiResponse(true, "All messages marked as read");
    //}

    [HttpGet]
    public ApiResponse MarkRead(int fromMemberId)
    {
        bool isDirty = false;
        ApiResponse unreadApi = AnyUnreadMessages();
        if (unreadApi != null)
        {
            IMember fromMember = Services.MemberService.GetById(fromMemberId);
            if (fromMember != null)
            {
                if (unreadApi.Data != null)
                {
                    List<int> unreadList = (List<int>)unreadApi.Data;
                    if (unreadList != null && unreadList.Count > 0)
                    {
                        foreach (int Id in unreadList)
                        {
                            IContent content = Services.ContentService.GetById(Id);
                            if (content != null)
                            {
                                if (content.HasProperty("wwread") && content.HasProperty("wwfrom"))
                                {
                                    if (content.HasProperty("wwfrom") && content.GetValue<string>("wwfrom") == fromMemberId.ToString() || content.GetValue<string>("wwfrom") == $"umb://member/{fromMember.Key.ToString().Replace("-", string.Empty)}")
                                    {
                                        content.SetValue("wwread", true);
                                        Services.ContentService.SaveAndPublishWithStatus(content);
                                        isDirty = true;
                                    }
                                }
                            }
                        }
                        if (isDirty)
                        {
                            return new ApiResponse(true, "All messages marked as read.");
                        }
                    }
                }
            }
            else
            {
                return new ApiResponse(false, $"FromMember with Id {fromMemberId} could not be loaded.");
            }
        }

        return new ApiResponse(true, "No work to be done.");
    }
}
}*/
