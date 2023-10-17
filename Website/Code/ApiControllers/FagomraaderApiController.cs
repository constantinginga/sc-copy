using StartupCentral.Code.Class;
using System;
using System.Web;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;

namespace StartupCentral.Code.ApiControllers
{
    public class FagomraaderApiController : UmbracoApiController
    {
        [HttpPost]
        public Model.ApiResponse Create()
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (member != null)
            {
                string name = HttpContext.Current.Request.Form["name"];
                var isAdmin = System.Web.Security.Roles.IsUserInRole(member.Username, "Admin");

                if (isAdmin)
                {
                    int fagomraaderRootFolderId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["StartupCentral.FagOmraader.RootFolder.Id"]);
                    IContent fagomraaderRoot = Services.ContentService.GetById(fagomraaderRootFolderId);

                    if (fagomraaderRoot != null)
                    {
                        // https://github.com/umbraco/Umbraco-CMS/issues/4452#issuecomment-505664486
                        IContent content = Services.ContentService.CreateAndSave(name, fagomraaderRoot, "containeritem");
                        Services.ContentService.SaveAndPublish(content);

                        return new Model.ApiResponse() { Success = true, Message = "Fagområde er blevet oprettet." };
                    }
                }
                else
                {
                    return new Model.ApiResponse() { Success = false, Message = "You are not authorized to create Fagområder, since you need to be admin." };
                }
            }

            return new Model.ApiResponse() { Success = false, Message = "Token not authorized." };
        }

        [HttpGet]
        public Model.ApiResponse Delete(int id)
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (member != null)
            {
                string name = HttpContext.Current.Request.Form["name"];
                var isAdmin = System.Web.Security.Roles.IsUserInRole(member.Username, "Admin");

                if (isAdmin)
                {
                    IContent content = Services.ContentService.GetById(id);
                    if (content != null)
                    {
                        Services.ContentService.Unpublish(content);
                        return new Model.ApiResponse() { Success = true, Message = "Fagområde er blevet slettet." };
                    }
                }
                else
                {
                    return new Model.ApiResponse() { Success = false, Message = "You are not authorized to create Fagområder, since you need to be admin." };
                }
            }

            return new Model.ApiResponse() { Success = false, Message = "Token not authorized." };
        }

        [HttpGet]
        public Model.ApiResponse Update(int id, string name)
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (member != null)
            {
                var isAdmin = System.Web.Security.Roles.IsUserInRole(member.Username, "Admin");

                if (isAdmin)
                {
                    IContent content = Services.ContentService.GetById(id);
                    if (content != null)
                    {
                        content.Name = name;
                        Services.ContentService.SaveAndPublish(content);
                        return new Model.ApiResponse() { Success = true, Message = "Fagområde er blevet opdateret." };
                    }
                }
                else
                {
                    return new Model.ApiResponse() { Success = false, Message = "You are not authorized to create Fagområder, since you need to be admin." };
                }
            }

            return new Model.ApiResponse() { Success = false, Message = "Token not authorized." };
        }

        [HttpGet]
        public Model.Fagomraade GetById(int Id)
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (member != null)
            {
                if (Id > 0)
                {
                    IContent content = Services.ContentService.GetById(Id);
                    if (content != null)
                    {
                        return new Model.Fagomraade(content);
                    }
                }
            }

            return null;
        }
    }
}