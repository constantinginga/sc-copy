using StartupCentral.Code.Class;
using StartupCentral.Extensions;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace StartupCentral.Code.Controllers
{
    public static class UserController
    {

        public static string IssueTokenIfEmpty(IMember member)
        {
            string token = string.Empty;

            if (member != null)
            {
                if (member.HasProperty("userToken"))
                {
                    if (member.GetValue("userToken") != null)
                    {
                        token = member.GetValue<string>("userToken");
                        if (Class.TokenValidator.ValidateToken(token) != null)
                        {
                            return token;
                        }
                    }

                    token = new JwtSecurityTokenHandler().WriteToken(Class.TokenValidator.GetJwtSecurityToken(member.Key));
                    member.SetValue("userToken", token);
                    ApplicationContext.Current.Services.MemberService.Save(member, true);
                    return token;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns upload folder for media due to member-type.
        /// </summary>
        /// <param name="currentMember"></param>
        /// <returns></returns>
        public static IMedia GetProfileImageFolder(IMember currentMember)
        {
            int studenterRootFolderId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["StartupCentral.Student.RootFolder.Id"]); // Media/Studenter (M.I.S.)
            int coachesRootFolderId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["StartupCentral.Coach.Media.RootFolder.Id"]);  // Media/Coaches
            int businessAngelsRootFolderId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["StartupCentral.Investor.Media.RootFolder.Id"]); // Media/Offenlig side/Business Angels

            switch (currentMember.ContentTypeAlias)
            {
                case "student":
                    string memberEmail = string.Empty;
                    if (!string.IsNullOrEmpty(currentMember.Email))
                    {
                        memberEmail = currentMember.Email;
                    }
                    else if (currentMember.GetValue("wwemail") != null)
                    {
                        memberEmail = currentMember.GetValue<string>("wwemail");
                    }

                    IMedia studentFolder = null;
                    IMedia mediaRoot = ApplicationContext.Current.Services.MediaService.GetById(studenterRootFolderId);
                    if (mediaRoot != null)
                    {
                        studentFolder = // mediaRoot.Children().Where(itm => itm.Name == memberEmail).FirstOrDefault();
                            ApplicationContext.Current.Services.MediaService.GetPagedChildren(mediaRoot.Id, 0, int.MaxValue, out long total)
                            .Where(itm => itm.Name == memberEmail).FirstOrDefault();
                    }

                    if (studentFolder == null)
                    {
                        studentFolder = ApplicationContext.Current.Services.MediaService.CreateMedia(memberEmail, studenterRootFolderId, "Folder");
                        studentFolder.SetValue("memberId", currentMember.Id);
                        ApplicationContext.Current.Services.MediaService.Save(studentFolder);
                    }

                    return studentFolder;

                case "coach1":
                    return ApplicationContext.Current.Services.MediaService.GetById(coachesRootFolderId);

                case "investor":
                    return ApplicationContext.Current.Services.MediaService.GetById(businessAngelsRootFolderId);
            }

            return null;
        }

        public static string GetMemberFullname(int memberId)
        {
            if (memberId > 0)
            {
                return ApplicationContext.Current.Services.MemberService.GetById(memberId)?.Name;
            }

            return "Ukendt";
        }

        public static IMember GetByEmail(string email)
        {
            if (!string.IsNullOrEmpty(email) && email.IsEmailAddress())
                return ApplicationContext.Current.Services.MemberService.GetByEmail(email);
            return null;
        }

        public async static Task<bool> SendNewPassword(string email)
        {
            return await SendNewPassword(GetByEmail(email));
        }

        public async static Task<bool> SendNewPassword(IMember member)
        {
            if (member != null)
            {
                PasswordController passwordController = new PasswordController();
                string newPassword = System.Web.Security.Membership.GeneratePassword(passwordController.GetMinRequiredPasswordLength(), 0);
                Random rnd = new Random();
                newPassword = System.Text.RegularExpressions.Regex.Replace(newPassword, @"[^0-9]", m => rnd.Next(0, 9).ToString());

                await Emailing.SendNewPasswordEmail(member, newPassword);
                return true;
            }
            return false;
        }
    }
}