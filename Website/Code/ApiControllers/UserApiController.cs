using StartupCentral.Code.Class;
using System.Web;
using System.Web.Http;
using Umbraco.Core;
using Core = Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;
using StartupCentral.Code.Model;
using System.Collections.Generic;
using System.IO;

namespace StartupCentral.Code.ApiControllers
{
    public class UserApiController : UmbracoApiController
    {
        private IMember GetMember() => TokenValidator.GetCurrentMember(HttpContext.Current.Request);

        //public ApiResponse UpdateSocialProperties()
        //{
        //    var form = HttpContext.Current.Request.Form;      
        //    var propertyNameUmbracoFb = form[1];
        //    var propertyContentFb = form[0];

            
        //        //Dictionary<string, string> properties = new Dictionary<string, string>();
        //        //properties.Add(propertyNameUmbracoFb, propertyContentFb);

        //        IMember member = GetMember();

        //    if(member!=null)
        //    {
        //        if (HttpContext.Current.Request.ContentLength > 0)
        //        {
        //            string usertype = HttpContext.Current.Request.Form["user"];
        //        }

        //        if (member.HasProperty(propertyNameUmbraco))
        //        {
        //            member.SetValue(propertyNameUmbraco, propertyContent);
        //        }
        //        else
        //        {
        //            return new ApiResponse(false, $"UpdateProperty(): {member.ContentTypeAlias} property {propertyNameUmbraco} not found.");
        //        }
        //    }

        //}


        [HttpPost]
        public ApiResponse UpdateProperty()
        {
            var form = HttpContext.Current.Request.Form;
            var propertyContent = form[0];
            var propertyNameUmbraco = form[1];

            IMember member = GetMember();

            if (member != null)
            {
                if (member.HasProperty(propertyNameUmbraco))
                {
                    member.SetValue(propertyNameUmbraco, propertyContent);
                }
                else
                {
                    return new ApiResponse(false, $"UpdateProperty(): {member.ContentTypeAlias} property {propertyNameUmbraco} not found.");
                }

                if (member.IsDirty())
                {
                    Services.MemberService.Save(member);
                    return new ApiResponse(true, $"UpdateProperty(): {member.ContentTypeAlias} property {propertyNameUmbraco} updated.");
                }
                
            }
            return new ApiResponse(false, $"UpdateProperty(): Member's authorisation failed.");
        }

        [HttpPost]
        public ApiResponse UploadPhp()
        {
            if (HttpContext.Current.Request.Files != null && HttpContext.Current.Request.Files.Count > 0)
            {
                IMember member = GetMember();
                IMedia folder = Controllers.UserController.GetProfileImageFolder(member);

                if (folder != null)
                {
                    for (var i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {
                        if (HttpContext.Current.Request.Files[i].ContentLength > 0)
                        {
                            IMedia mediaFile = ApplicationContext.Current.Services.MediaService.CreateMedia(HttpContext.Current.Request.Files[i].FileName, folder, "Image");
                            System.IO.Stream s = HttpContext.Current.Request.Files[i].InputStream;
                            mediaFile.SetValue(Core.Composing.Current.Services.ContentTypeBaseServices, "umbracoFile", HttpContext.Current.Request.Files[i].FileName, s);
                            s.Close();

                            ApplicationContext.Current.Services.MediaService.Save(mediaFile);

                            var typeAliasVariable = "wwavatar";

                            if (member.ContentTypeAlias == "Partner")
                                typeAliasVariable = "wwpartnerAvatar";

                            member.SetValue(typeAliasVariable, mediaFile.GetUdi().ToString());
                            ApplicationContext.Current.Services.MemberService.Save(member);

                            return new ApiResponse(true, $"UploadPhp(): Php uploaded.");
                        }
                    }
                }
                return new ApiResponse(false, $"UploadPhp(): User folder not found.");
            }
            return new ApiResponse(false, $"UploadPhp(): Member's authorisation failed.");
        }

        [HttpPost]
        public ApiResponse UploadCompanyLogo()
        {
            if (HttpContext.Current.Request.Files != null && HttpContext.Current.Request.Files.Count > 0)
            {
                IMember member = GetMember();
                IMedia folder = Controllers.UserController.GetProfileImageFolder(member);


                if (folder != null)
                {
                    for (var i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {
                        if (HttpContext.Current.Request.Files[i].ContentLength > 0)
                        {
                            IMedia mediaFile = ApplicationContext.Current.Services.MediaService.CreateMedia(HttpContext.Current.Request.Files[i].FileName, folder, "Image");
                            System.IO.Stream s = HttpContext.Current.Request.Files[i].InputStream;
                            mediaFile.SetValue(Core.Composing.Current.Services.ContentTypeBaseServices, "umbracoFile", HttpContext.Current.Request.Files[i].FileName, s);
                            s.Close();

                            ApplicationContext.Current.Services.MediaService.Save(mediaFile);

                            var typeAliasVariable = "companyLogo";

                            member.SetValue(typeAliasVariable, mediaFile.GetUdi().ToString());
                            ApplicationContext.Current.Services.MemberService.Save(member);

                            return new ApiResponse(true, $"UploadCompanyLogo(): Company logo uploaded.", mediaFile.GetUdi().ToString());
                        }
                    }
                }

                return new ApiResponse(false, $"UploadCompanyLogo(): User folder not found.");
            }
            return new ApiResponse(false, $"UploadCompanyLogo(): Member's authorisation failed.");
        }

        [HttpPost]
        public ApiResponse ChangePassword()
        {
            var form = HttpContext.Current.Request.Form;
            var password0 = form[0];
            var password1 = form[1];

            IMember member = GetMember();

            if(member != null)
            {
                if (!string.IsNullOrEmpty(password0) && !string.IsNullOrEmpty(password1))
                {
                    Controllers.PasswordController passwordController = new Controllers.PasswordController();
                    var passwordValidation = passwordController.ValidatePasswords(password0, password1);

                    if (passwordValidation.Success)
                    {
                        ApplicationContext.Current.Services.MemberService.SavePassword(member, password0);
                        return new ApiResponse(true, $"ChangePassword(): Password updated.");
                    }

                    return passwordValidation;
                    //return new ApiResponse(false, $"ChangePassword(): Passwords does not match.");
                }
            }
            
            return new ApiResponse(false, $"ChangePassword(): Member's authorisation failed.");
        }

        [HttpPost]
        public ApiResponse SubmitBlog()
        {
            var form = HttpContext.Current.Request.Form;

            IMember member = GetMember();

            if (member != null && form[0] == member.Email)
            {
                
                if (HttpContext.Current.Request.Files != null && HttpContext.Current.Request.Files.Count > 0)
                {
                 
                    HttpPostedFile hpfP = HttpContext.Current.Request.Files[0];
                    HttpPostedFile hpfB = HttpContext.Current.Request.Files[1];

                    List<string> aFiles = new List<string>() { ".odt", ".doc", ".docx", ".pages" };

                    if(aFiles.Contains(Path.GetExtension(hpfB.FileName).ToLower()))
                    {
                        try
                        {
                           
                            Emailing.SendSubmitBlog(member, form, hpfP, hpfB);
                            return new ApiResponse(true, $"SubmitBlog(): Blog succssesfully submited");
                        }
                        catch
                        {
                            return new ApiResponse(false, $"SubmitBlog(): Email could not be send.");
                        }
                                
                    }

                    return new ApiResponse(false, $"SubmitBlog(): Attached blog file is not a .doc type.");
                }

                return new ApiResponse(false, $"SubmitBlog(): No files attached.");

            }
                
            return new ApiResponse(false, $"SubmitBlog(): Member's authorisation failed.");
        }

    }
}