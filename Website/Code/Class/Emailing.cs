using StartupCentral.Code.Controllers;
using StartupCentral.Code.Model;
using StartupCentral.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Composing.CompositionExtensions;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.Security;

namespace StartupCentral.Code.Class
{
    public static class Emailing
    {
        public static void SendCreationEmail(IMember member, string password)
        {
            string memberEmail = string.Empty;
            if (!string.IsNullOrEmpty(member.Email) && member.Email.IsEmailAddress())
            {
                memberEmail = member.Email;
            }
            else if (member.GetValue("wwemail") != null)
            {
                memberEmail = member.GetValue<string>("wwemail");
            }

            var mailTemplate = Helpers.Nodes.GetFirstNodeByType(-1, "mail");
            string subject = "";
            string bodyHtml = "";

            if (mailTemplate != null)
            {
                switch (member.ContentType.Alias)
                {
                    case "coach1":
                    case "investor":
                    case "teacher":
                        if (mailTemplate.GetProperty("wwcreationSubjectci") != null)
                        {
                            subject = mailTemplate.GetProperty("wwcreationSubjectci").GetValue().ToString();
                        }

                        if (mailTemplate.GetProperty("wwcreationHtmlci") != null)
                        {
                            bodyHtml = mailTemplate.GetProperty("wwcreationHtmlci").GetValue().ToString();
                        }

                        break;

                    case "student":
                        if (mailTemplate.GetProperty("wwcreationSubject") != null)
                        {
                            subject = mailTemplate.GetProperty("wwcreationSubject").GetValue().ToString();
                        }

                        if (mailTemplate.GetProperty("wwcreationHtml") != null)
                        {
                            bodyHtml = mailTemplate.GetProperty("wwcreationHtml").GetValue().ToString();
                        }

                        break;

                    default:

                        break;
                }
            }

            bodyHtml = bodyHtml.Replace("{{fornavn}}", member.Name.Split(' ').First());
            bodyHtml = bodyHtml.Replace("{{brugernavn}}", member.Username);
            bodyHtml = bodyHtml.Replace("{{adgangskode}}", password);

            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient();
            mail.From = new MailAddress("kontakt@startupcentral.dk");
            mail.To.Add(memberEmail);
            mail.Subject = subject;
            mail.Body = "<head><base href=\"https://www.startupcentral.dk/\"></head><body>" + bodyHtml + "</body>";
            mail.IsBodyHtml = true;
            try
            {
                smtpServer.Send(mail);
            }
            catch (System.Exception) { }

            try
            {
                string to = "";
                if (mail.To != null)
                {
                    to = string.Join(",", mail.To.Select(itm => itm.Address));
                }
                string cc = "";
                if (mail.CC != null)
                {
                    cc = string.Join(",", mail.CC.Select(itm => itm.Address));
                }
                string bcc = "";
                if (mail.Bcc != null)
                {
                    bcc = string.Join(",", mail.Bcc.Select(itm => itm.Address));
                }

                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Email sent ({mail.Subject}). (To: {to} | CC: {cc} | BCC: {bcc}) -> {mail.Body}");
            }
            catch (System.Exception)
            {
                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Email sent ({mail.Subject}). Error ocured while extracting more info.");
            }

        }

        public static void SendApprovvalEmail(IMember member, string password, bool hasMobilePaySubscription = true)
        {
            string memberEmail = GetMemberEmail(member);

            var mailTemplate = Helpers.Nodes.GetFirstNodeByType(-1, "mail");
            string subject = "";
            string bodyHtml = "";

            if (mailTemplate != null)
            {
                switch (member.ContentType.Alias)
                {
                    case "coach1":
                    case "investor":
                    case "teacher":
                        break;
                    case "partner":
                        if (hasMobilePaySubscription)
                        {
                            subject = GetPropertyValue(mailTemplate, "wwapprovedSubjectPartner");
                            bodyHtml = GetPropertyValue(mailTemplate, "wwapprovedHTMLpartner");
                        }
                        else
                        {
                            subject = GetPropertyValue(mailTemplate, "wwapprovedSubjectPartner");
                            bodyHtml = GetPropertyValue(mailTemplate, "wwapprovedWithoutPaySubscriptionHTMLpartner");
                        }
                        break;
                    case "student":
                        if (hasMobilePaySubscription)
                        {
                            subject = GetPropertyValue(mailTemplate, "wwapprovedSubject");
                            bodyHtml = GetPropertyValue(mailTemplate, "wwapprovedHTML");
                        }
                        else
                        {
                            subject = GetPropertyValue(mailTemplate, "wwapprovedSubject");
                            bodyHtml = GetPropertyValue(mailTemplate, "wwapprovedWithoutPaySubscriptionHTML");
                        }
                        break;
                    default:
                        break;
                }
            }

            bodyHtml = bodyHtml.Replace("{{fornavn}}", member.Name.Split(' ').First());
            bodyHtml = bodyHtml.Replace("{{brugernavn}}", member.Username);
            bodyHtml = bodyHtml.Replace("{{adgangskode}}", password);

            MailMessage mail = SendMail(memberEmail, subject, bodyHtml);

            try
            {
                string to = "";
                if (mail.To != null)
                {
                    to = string.Join(",", mail.To.Select(itm => itm.Address));
                }
                string cc = "";
                if (mail.CC != null)
                {
                    cc = string.Join(",", mail.CC.Select(itm => itm.Address));
                }
                string bcc = "";
                if (mail.Bcc != null)
                {
                    bcc = string.Join(",", mail.Bcc.Select(itm => itm.Address));
                }

                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Email sent ({mail.Subject}). (To: {to} | CC: {cc} | BCC: {bcc}) -> {mail.Body}");
            }
            catch (System.Exception)
            {
                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Email sent ({mail.Subject}). Error ocured while extracting more info.");
            }

        }

        public static void SendContactEmail(string email, string firstName, string lastName, string subject, string message)
        {
            try
            {
                string mailBody = @"<p>First name:{firstName}</p>
                                    <p>Last name:{lastName}</p>
                                    <p>Email:{email}</p>
                                    <p>Subject:{subject}</p>
                                    <p>Message:{message}</p>";

                mailBody = mailBody.Replace("{email}", email)
                    .Replace("{firstName}", string.IsNullOrEmpty(firstName) ? string.Empty : firstName)
                    .Replace("{lastName}", string.IsNullOrEmpty(lastName) ? string.Empty : lastName)
                    .Replace("{subject}", string.IsNullOrEmpty(subject) ? string.Empty : subject)
                    .Replace("{message}", string.IsNullOrEmpty(message) ? string.Empty : message);



                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient();
                mail.From = new MailAddress("kontakt@startupcentral.dk");
                mail.To.Add("kontakt@startupcentral.dk");
                mail.Subject = "Contact mail";
                mail.Body = "<head><base href=\"https://www.startupcentral.dk/\"></head><body>" + mailBody + "</body>";
                mail.IsBodyHtml = true;
                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Email sent to {email} error {ex.InnerException}");
            }
        }

        public async static Task SendNewPasswordEmail(IMember member, string password)
        {
            string memberEmail = string.Empty;
            if (!string.IsNullOrEmpty(member.Email) && member.Email.IsEmailAddress())
            {
                memberEmail = member.Email;
            }
            else if (member.GetValue("wwemail") != null)
            {
                memberEmail = member.GetValue<string>("wwemail");
            }

            //var mailTemplate = Helpers.Nodes.GetFirstNodeByType(-1, "mail");
            string subject = "Startup Central: Ny adgangskode";
            //string bodyHtml = "";

            //if (mailTemplate != null)
            //{
            //    if (mailTemplate.GetProperty("wwnewPasswordHTML") != null)
            //    {
            //        var umbracoContextAccessor = DependencyResolver.Current.GetService<IUmbracoContextAccessor>();
            //        var context = umbracoContextAccessor.UmbracoContext;
            //        bodyHtml = mailTemplate.GetProperty("wwnewPasswordHTML").GetValue().ToString();
            //    }

            //    if (mailTemplate.GetProperty("wwnewPasswordSubject") != null)
            //    {
            //        subject = mailTemplate.GetProperty("wwnewPasswordSubject").GetValue().ToString();
            //    }
            //}

            string bodyHtml = $"Hej {member.Name.Split(' ').First()},<br />Du har bestilt en ny adgangskode til Startup Central.<br />Din nye adgangskode er: <b>{password}</b> <br /> Benyt denne adresse for at logge ind: <a href=\"https://www.startupcentral.dk/log-in\">https://www.startupcentral.dk/log-in</a> <br /> Med venlig hilsen<br/>Startup Central";

            //bodyHtml = bodyHtml.Replace("{{fornavn}}", member.Name.Split(' ').First());
            //bodyHtml = bodyHtml.Replace("{{brugernavn}}", member.Username);
            //bodyHtml = bodyHtml.Replace("{{adgangskode}}", password);

            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient("email-smtp.eu-north-1.amazonaws.com", 587);
            smtpServer.EnableSsl = true;
            smtpServer.Credentials = new NetworkCredential("AKIA5IOW3KCDOBL4EEX5", "BJtTrtHv0lCASKyPg9UyELuul1b913zLJlUrNMpw+zDB");
            mail.From = new MailAddress("kontakt@startupcentral.dk");
            mail.To.Add(memberEmail);
            mail.Subject = subject;
            mail.Body = "<head><base href=\"https://www.startupcentral.dk/\"></head><body>" + bodyHtml + "</body>";
            mail.IsBodyHtml = true;

            try
            {
                await smtpServer.SendMailAsync(mail);
                // if mail succeeds, then the password is changed on member.
                ApplicationContext.Current.Services.MemberService.SavePassword(member, password);
            }
            catch (System.Exception)
            {
            }
        }

        public static void CancelSubscriptionEmail(IMember member, DateTime date)
        {
            string memberEmail = string.Empty;
            if (!string.IsNullOrEmpty(member.Email) && member.Email.IsEmailAddress())
            {
                memberEmail = member.Email;
            }
            else if (member.GetValue("wwemail") != null)
            {
                memberEmail = member.GetValue<string>("wwemail");
            }

            var mailTemplate = Helpers.Nodes.GetFirstNodeByType(-1, "mail");
            string subject = "Startup Central Opsigelse";
            string bodyHtml = "";

            if (mailTemplate != null)
            {
                bodyHtml = @"<p>Kære {name}.<p />
                             <p>Du har valgt at opsige dit abonnement hos Startup Central fra dags dato. Dit abonnement udløber {date}. 
                             Vi sender endnu en påmindelse 7 dage inden udløbsdato. <p />
                             <p>Ved fortrydelse bedes du kontakte os direkte via mail eller telefonisk. <p />
                             <br /><br /><p>Med Venlig Hilsen Startup Central<p />";

                /*if (mailTemplate.GetProperty("wwcreationHtml") != null)
                {
                    bodyHtml = mailTemplate.GetProperty("wwcreationHtml").Value;
                }*/

                bodyHtml = bodyHtml.Replace("{name}", member.Name.Split(' ').First())
                    .Replace("{date}", date.ToString("dd-MM-yyyy"));

                SendMail(memberEmail, subject, bodyHtml);
            }
        }

        public static void SendNewsLetterSubscriptions(string name, string email)
        {
            try
            {
                string mailBody = @"<p>Name:{Name}</p>
                                <p>Email:{email}</p>";
                mailBody = mailBody.Replace("{Name}", name).Replace("{email}", email);
                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient();
                mail.From = new MailAddress("kontakt@startupcentral.dk");
                mail.To.Add("dp@startupcentral.dk");
                mail.Subject = "newsletter";
                mail.Body = "<head><base href=\"https://www.startupcentral.dk/\"></head><body>" + mailBody + "</body>";
                mail.IsBodyHtml = true;
                smtpServer.Send(mail);
            }
            catch (System.Exception e)
            {
                //LogHelper.Info<string>(()=>$"Email sent error {e.InnerException.Message}");
            }
        }

        /// <summary>
        /// Notify an invester that he/she have reciveved a new business plan.
        /// </summary>
        public async static Task SendNewPlanNotification(IMember fromMember, IMember toMember)
        {
            try
            {
                var toEmail = GetMemberEmail(toMember);
                var mailTemplate = Helpers.Nodes.GetFirstNodeByType(-1, "mail");

                var subject = GetPropertyValue(mailTemplate, "wwnewplanSubject");
                var bodyHtml = GetPropertyValue(mailTemplate, "wwnewplanHTML");

                bodyHtml = bodyHtml.Replace("{{fornavn}}", toMember.Name.Split(' ').First());
                bodyHtml = bodyHtml.Replace("{{brugernavn}}", toMember.Username);

                //SendMail(toEmail, subject, bodyHtml);
                SmtpClient smtpServer = new SmtpClient("email-smtp.eu-north-1.amazonaws.com", 587);
                smtpServer.EnableSsl = true;
                smtpServer.Credentials = new NetworkCredential("AKIA5IOW3KCDOBL4EEX5", "BJtTrtHv0lCASKyPg9UyELuul1b913zLJlUrNMpw+zDB");
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("noreply@startupcentral.dk");
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = bodyHtml;
                mail.IsBodyHtml = true;

                await smtpServer.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"SendNewPlanNotification error {ex.InnerException.Message}");
                else
                    LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"SendNewPlanNotification error {ex.Message}");
            }
        }

        public async static Task SendNewMessageNotification(IMember fromMember, IMember toMember)
        {
            try
            {
                var toEmail = GetMemberEmail(toMember);
                var notification = GetNewMessageNotification(fromMember, toMember);
                //SendMail(toEmail, notification.subject, notification.bodyHtml);


                SmtpClient smtpServer = new SmtpClient("email-smtp.eu-north-1.amazonaws.com", 587);
                smtpServer.EnableSsl = true;
                smtpServer.Credentials = new NetworkCredential("AKIA5IOW3KCDOBL4EEX5", "BJtTrtHv0lCASKyPg9UyELuul1b913zLJlUrNMpw+zDB");
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("noreply@startupcentral.dk");
                mail.To.Add(toEmail);
                mail.Subject = notification.subject;
                mail.Body = notification.bodyHtml;
                mail.IsBodyHtml = true;

                await smtpServer.SendMailAsync(mail);

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"SendNewMessageNotification error {ex.InnerException.Message}");
                else
                    LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"SendNewMessageNotification error {ex.Message}");
            }
        }

        public static ValidationResponse ValidateEmail(string emailAddress)
        {
            if (!string.IsNullOrEmpty(emailAddress))
            {
                if (emailAddress.Length < 4) return new ValidationResponse(false, "E-mail adressen er for kort");

                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(emailAddress);
                if (!match.Success) return new ValidationResponse(false, "E-mail adressen er ikke gyldig.");

                try
                {
                    var addr = new System.Net.Mail.MailAddress(emailAddress);
                    if (addr.Address != emailAddress) return new ValidationResponse(false, "E-mail adressen kan ikke benyttes.");
                }
                catch
                {
                    return new ValidationResponse(false, "Email address is not valid."); ;
                }

                // Compare to see that e-mail address is not already taken.
                long totalRecords = 0;
                foreach (IMember user in ApplicationContext.Current.Services.MemberService.GetAll(0, int.MaxValue, out totalRecords))
                {
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        if (user.Email.ToLower() == emailAddress.ToLower())
                        {
                            return new ValidationResponse(false, "E-mail adressen er allerede i brug.");
                        }
                    }
                }
            }
            else
            {
                return new ValidationResponse(false, "E-mail adressen skal udfyldes.");
            }

            return new ValidationResponse(true, string.Empty);
        }

        #region Methods

        public static MailMessage SendMail(string toEmail, string subject, string body, bool isBodyHtml = true)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient();
            mail.From = new MailAddress("kontakt@startupcentral.dk");
            mail.To.Add(toEmail);

            if (subject == "Ekstra tilbud efterspurgt | Startup Central")
            {
                mail.Bcc.Add("kontakt@startupcentral.dk");
            }

            mail.Subject = subject;
            if (isBodyHtml)
            {
                mail.Body = "<head><base href=\"https://www.startupcentral.dk/\"></head><body>" + body + "</body>";
                mail.IsBodyHtml = true;
            } else
            {
                mail.Body = body;
                mail.IsBodyHtml = false;
            }

            try
            {
                smtpServer.Send(mail);
            }
            catch (Exception e)
            {
                Console.WriteLine($"SendMail: {e.Message}, Stack trace: {e.StackTrace}");
                //LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Email sent error {e.InnerException.Message}");
            }

            return mail;
        }

        private static MailMessage SendMailWithAttachment(string toEmail, string subject, string bodyHtml, List<HttpPostedFile> data)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient();
            mail.From = new MailAddress("kontakt@startupcentral.dk");
            mail.To.Add(toEmail);

            mail.Subject = subject;
            mail.Body = "<head><base href=\"https://www.startupcentral.dk/\"></head><body>" + bodyHtml + "</body>";
            mail.IsBodyHtml = true;

            foreach (var f in data)
            {
                mail.Attachments.Add(new Attachment(f.InputStream, f.FileName));
            }

            try
            {
                smtpServer.Send(mail);
            }
            catch (System.Exception e)
            {
                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Email sent error {e.InnerException.Message}");
            }

            return mail;
        }

        private static string GetMemberEmail(IMember member)
        {
            var result = string.Empty;

            if (!string.IsNullOrEmpty(member.Email) && member.Email.IsEmailAddress())
            {
                result = member.Email;
            }
            else if (member.GetValue("wwemail") != null)
            {
                result = member.GetValue<string>("wwemail");
            }

            return result;
        }

        private static (string subject, string bodyHtml) GetNewMessageNotification(IMember fromMember, IMember toMember)
        {
            var result = (subject: "", bodyHtml: "");

            var mailTemplate = Helpers.Nodes.GetFirstNodeByType(-1, "mail");

            if (toMember.ContentType.Alias == "student")
            {
                if (fromMember.ContentType.Alias == "investor")
                {
                    result.subject = GetPropertyValue(mailTemplate, "wwbaskedFromInvestorSubject");
                    result.bodyHtml = GetPropertyValue(mailTemplate, "wwbaskedFromInvestorHTML");
                }
                else if (fromMember.Id == 30823)
                {
                    result.subject = GetPropertyValue(mailTemplate, "wwBeskedFraStartupCentralSubject");
                    result.bodyHtml = GetPropertyValue(mailTemplate, "wwBeskedFraStartupCentralHtml");
                }
                else
                {
                    result.subject = GetPropertyValue(mailTemplate, "wwbaskedFromCoachSubject");
                    result.bodyHtml = GetPropertyValue(mailTemplate, "wwbaskedFromCoachHTML");
                }
            }
            else
            {
                result.subject = GetPropertyValue(mailTemplate, "wwbaskedFromStudentSubject");
                result.bodyHtml = GetPropertyValue(mailTemplate, "wwbaskedFromStudentHTML");
            }

            result.bodyHtml = result.bodyHtml.Replace("{{fornavn}}", toMember.Name.Split(' ').First());

            return result;
        }

        private static string GetPropertyValue(IPublishedContent node, string property)
        {
            return node != null && node.GetProperty(property) != null ? string.Empty + node.GetProperty(property).GetValue() : string.Empty;
        }

        #endregion

        public async static Task SendExtraOffer(int userId, int nodeId)
        {
            IMember member = ApplicationContext.Current.Services.MemberService.GetById(userId);

            var node = Umbraco.Web.Composing.Current.UmbracoHelper.Content(nodeId);



            string bodyHtml = "";
            var offerName = node.Name;

            var promocode = node.GetProperty("offerPromoCode")?.GetValue()?.ToString();
            var offerLink = node.GetProperty("offerLink")?.GetValue()?.ToString();
            var phoneNumber = node.GetProperty("offerPhoneNumber").GetValue()?.ToString();

            //var partnerName = node.GetProperty("titleOfTheOffer").GetValue()?.ToString();
            //var offerPartnerEmail = node.GetProperty("offerPartnerEmail").GetValue()?.ToString();

            string memberName = member.Name.Split(' ').First();

            string subject = "Ekstra tilbud efterspurgt | Startup Central";

            if (!string.IsNullOrEmpty(promocode) && !string.IsNullOrEmpty(offerLink))
            {
                bodyHtml = @"<p>Kære {username},<p/>
                            <p>Gennem dit medlemskab har du vist interesse for vores ekstra tilbud <b>{offerName}</b>.<p/>
                            <p>Hermed fremsender vi hvad du skal bruge for at kunne gøre brug af tilbuddet.<p/>
                            <p>Promokode: {promocode}<p/>
                            <p>Brug denne promokode ved køb for at gøre brug af vores unikke tilbud.<p/>
                            <p>Link: {offerLink}<p/>
                            <p>Følg dette link for at gøre krav på dit tilbud.<p/>
                            <br /><br />
                            <p>Med venlig hilsen<p />
                            <p>Startup Central<p />";


                bodyHtml = bodyHtml.Replace("{username}", memberName)
                        .Replace("{offerName}", offerName.ToString())
                        .Replace("{promocode}", promocode)
                        .Replace("{offerLink}", offerLink);
            }
            else if (phoneNumber != null)
            {
                bodyHtml = @"<p>Kære {username} ({memberEmail}),<p/>
                            <p>Gennem dit medlemskab har du vist interesse for vores ekstra tilbud <b>{offerName}</b>.<p/>
                            <p>Hermed fremsender vi hvad du skal bruge for at kunne gøre brug af tilbuddet.<p/>
                            <p>Kontakt: {phoneNumber}<p/>
                            <p>Kontakt dem, og fortæl at du kommer fra Startup Central - så du kan gøre brug af tilbuddet.<p/>
                            <br /><br />
                            <p>Med venlig hilsen<p />
                            <p>Startup Central<p />";


                bodyHtml = bodyHtml.Replace("{username}", memberName)
                        .Replace("{offerName}", offerName.ToString())
                        .Replace("{phoneNumber}", phoneNumber)
                        .Replace("{memberEmail}", member.Email);
            }



            SmtpClient smtpServer = new SmtpClient("email-smtp.eu-north-1.amazonaws.com", 587);
            smtpServer.EnableSsl = true;
            smtpServer.Credentials = new NetworkCredential("AKIA5IOW3KCDOBL4EEX5", "BJtTrtHv0lCASKyPg9UyELuul1b913zLJlUrNMpw+zDB");
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("noreply@startupcentral.dk");
            mail.To.Add("kontakt@startupcentral.dk");
            mail.To.Add(member.Email);
            //mail.To.Add(phoneNumber);
            mail.Subject = subject;
            mail.Body = "<head><base href=\"https://www.startupcentral.dk/\"></head><body>" + bodyHtml + "</body>";
            mail.IsBodyHtml = true;
            await smtpServer.SendMailAsync(mail);
            //await SendMail(member.Email, subject, bodyHtml);
            //await smtpServer.SendMailAsync(mail);
        }

        public async static Task SendExtraOfferToPartner(int userId, int nodeId)
        {
            IMember member = ApplicationContext.Current.Services.MemberService.GetById(userId);

            var node = Umbraco.Web.Composing.Current.UmbracoHelper.Content(nodeId);

            string bodyHtml = "";
            
            var partnerName = node.GetProperty("titleOfTheOffer").GetValue()?.ToString();
            var offerPartnerEmail = node.GetProperty("offerPartnerEmail").GetValue()?.ToString();

            string subject = "Ekstra tilbud efterspurgt | Startup Central";


            if (offerPartnerEmail != null)
            {


                bodyHtml = @"<p>Kære {partnername},<p/>
                        <p>Et medlem har vist interesse i din virksomhed.<p/>
                        <p>Du kan kontakte medlemmet via denne mail: {memberEmail} <p/>
                        <br /><br />
                        <p>Med venlig hilsen<p />
                        <p>Startup Central<p />";



                bodyHtml = bodyHtml.Replace("{partnername}", partnerName)
                        .Replace("{memberEmail}", member.Email);
            }

            SmtpClient smtpServer = new SmtpClient("email-smtp.eu-north-1.amazonaws.com", 587);
            smtpServer.EnableSsl = true;
            smtpServer.Credentials = new NetworkCredential("AKIA5IOW3KCDOBL4EEX5", "BJtTrtHv0lCASKyPg9UyELuul1b913zLJlUrNMpw+zDB");
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("noreply@startupcentral.dk");
            mail.To.Add(offerPartnerEmail);
            mail.Subject = subject;
            mail.Body = "<head><base href=\"https://www.startupcentral.dk/\"></head><body>" + bodyHtml + "</body>";
            mail.IsBodyHtml = true;
            await smtpServer.SendMailAsync(mail);
            //await SendMail(member.Email, subject, bodyHtml);
            //await smtpServer.SendMailAsync(mail);
        }

        public async static Task SendInterestNotificationPartner(int userId, int partnerId)
        {
            string receiver = "kontakt@startupcentral.dk";

            //IMember memberP = ApplicationContext.Current.Services.MemberService.GetById(partnerId);
            //IMember memberU = ApplicationContext.Current.Services.MemberService.GetById(userId);
            //string partnerName = memberP.Name.ToString();
            //string memberName = memberU.Name.ToString();
            //string memberEmail = memberU.Email.ToString();
            //string memeberKey = memberU.Key.ToString();

            string time = DateTime.UtcNow.ToString();

            string subject = "Partner button click | Log";

            //string bodyHtml = $@"<p><b>{memberName}</b> with email = {memberEmail} and key = {memeberKey} <p/>
            //        <p>is interested in <b>{partnerName}</b> and clicked the partner link button at {time} UTC. <p/>
            //        <br/><br/>
            //        <p>Umbraco Backoffice<p/>";

            string bodyHtml = $@"<p>Member with id = {userId} <p/>
                    <p>is interested in partner with id = <b>{partnerId}</b> and clicked the partner link button at {time} UTC. <p/>
                    <br/><br/>
                    <p>To find the partner and the member, search the IDs in the Umbraco Backoffice.<p/>";

            //SendMail(reciever, subject, bodyHtml);

            SmtpClient smtpServer = new SmtpClient("email-smtp.eu-north-1.amazonaws.com", 587);
            smtpServer.EnableSsl = true;
            smtpServer.Credentials = new NetworkCredential("AKIA5IOW3KCDOBL4EEX5", "BJtTrtHv0lCASKyPg9UyELuul1b913zLJlUrNMpw+zDB");
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("noreply@startupcentral.dk");
            mail.To.Add(receiver);
            mail.Subject = subject;
            mail.Body = "<head><base href=\"https://www.startupcentral.dk/\"></head><body>" + bodyHtml + "</body>";
            mail.IsBodyHtml = true;
            await smtpServer.SendMailAsync(mail);
        }

        public static void SendSubmitBlog(IMember member, System.Collections.Specialized.NameValueCollection fields, HttpPostedFile p, HttpPostedFile b)
        {
            string reciever = "vh@startupcentral.dk";
            //string reciever = "it@startupcentral.dk";
            string bodyHtml = "";

            string subject = "New Blog Submittion";
            bodyHtml = @"<p>{memberName} (<b>{memberEmail}</b>) just sent a blog for approval:<p/>
                        <p>Title of the blog: <b>{title}</b> <p/>
                        <p>Category of the blog: <b>{category}</b> <p/>
                        <p>Requested publish date: <b>{publishdate}</b> <p/>
                        <p>Website reference: <b>{website}</b> <p/>
                        <p>Author's name: <b>{authorsname}</b> <p/>
                        <p>Author's position: <b>{authorsposition}</b> <p/>
                        <p>Restricted content: <b>{restricted}</b> <p/>
                        <br/><br/>
                        <p>Umbraco Backoffice <p/>";

            bodyHtml = bodyHtml.Replace("{memberName}", member.Name)
                    .Replace("{memberEmail}", member.Email)
                    .Replace("{title}", fields[2])
                    .Replace("{category}", fields[6])
                    .Replace("{publishdate}", fields[3])
                    .Replace("{website}", fields[1])
                    .Replace("{authorsname}", fields[4])
                    .Replace("{authorsposition}", fields[5]);

            try
            {
                bodyHtml = bodyHtml.Replace("{restricted}", fields[7]);
            }
            catch
            {
                bodyHtml = bodyHtml.Replace("{restricted}", "off");
            }
             

            var posted = new List<HttpPostedFile>()
            {
                p,
                b 
            };

            SendMailWithAttachment(reciever, subject, bodyHtml, posted);
        }

        public static async Task SendFreeUserEmail(IMember member)
        {
            var name = member.GetValue<string>("wwname");
            var email = member.GetValue<string>("wwemail");
            var phoneNumber = member.GetValue<string>("wwmobile");

            try
            {

                var cvr = member.GetValue<string>("cVR");
                string mailBody = "<h3>Hi,</h3> <h3>A new sign up as free user on the main website</h3>" +
                "<h4>Info:</h4>" +
                $" <h3>Name: <b>{name}</b>,</h3>" +
                $" <h3>Email: <b>{email}</b>,</h3>" +
                $" <h3>Phone: <b>{phoneNumber}</b>.</h3>";

                if (!string.IsNullOrWhiteSpace(cvr))
                {
                    mailBody += $"<h3>CVR: <b>{cvr}</b>,</h3>";
                }

                SmtpClient smtpServer = new SmtpClient("email-smtp.eu-north-1.amazonaws.com", 587);
                smtpServer.EnableSsl = true;
                smtpServer.Credentials = new NetworkCredential("AKIA5IOW3KCDOBL4EEX5", "BJtTrtHv0lCASKyPg9UyELuul1b913zLJlUrNMpw+zDB");
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("noreply@startupcentral.dk");
                mail.To.Add("kontakt@startupcentral.dk");
                mail.Subject = $"New Free User Sign up (Main website) {DateTime.Now.ToString("yyyy-MM-dd HH:mm")}";
                mail.Body = "<head><base href=\"https://www.startupcentral.dk/\"></head><body>" + mailBody + "</body>";
                mail.IsBodyHtml = true;
                //smtpServer.Host = "smtp.office365.com";
                //smtpServer.Send(mail);
                await smtpServer.SendMailAsync(mail);
                Console.WriteLine("EMAIL SENT?");
            }
            catch (Exception ex)
            {
                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Email sent to {email} error {ex.InnerException}");
            }
        }

        public async static Task SendFriendInvite(string friendEmail, int userId)
        {
            IMember member = ApplicationContext.Current.Services.MemberService.GetById(userId);

            string bodyHtml = "";
                   

            string subject = "Friend Invitation";


            bodyHtml = @"<p>Hej,<p/>
                        <p>{username} vil gerne invitere dig til at være en del af det online iværksætternetværket Startup Lounge.</b>.<p/>
                        <p>Det er gratis at downloade og blive en del af:<p/>
                        <p>Apple - https://apps.apple.com/dk/app/startup-central-lounge/id1638888731?l=da<p/>
                        <p>Android - https://play.google.com/store/apps/details?id=com.companyname.scsome.mobileapp<p/> 
                        <p>Læs mere om Startup Lounge her. - https://www.startupcentral.dk/lounge/login<p/>                        
                        <br /><br />
                        <p>Med venlig hilsen<p />
                        <p>Startup Central<p />";


            //bodyHtml = bodyHtml.Replace("{username}", friendEmail);
            bodyHtml = bodyHtml.Replace("{username}", member.Email);


            SmtpClient smtpServer = new SmtpClient("email-smtp.eu-north-1.amazonaws.com", 587);
            smtpServer.EnableSsl = true;
            smtpServer.Credentials = new NetworkCredential("AKIA5IOW3KCDOBL4EEX5", "BJtTrtHv0lCASKyPg9UyELuul1b913zLJlUrNMpw+zDB");
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("noreply@startupcentral.dk");
            mail.To.Add(friendEmail); //member from the form
            mail.To.Add("kontakt@startupcentral.dk");
            mail.Subject = subject;
            mail.Body = "<head><base href=\"https://www.startupcentral.dk/\"></head><body>" + bodyHtml + "</body>";
            mail.IsBodyHtml = true;
            await smtpServer.SendMailAsync(mail);
        }

    }
}