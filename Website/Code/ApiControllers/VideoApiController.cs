using StartupCentral.Code.Class;
using System.Web;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;

namespace StartupCentral.Code.ApiControllers
{
    public class VideoApiController : UmbracoApiController
    {
        [HttpGet]
        public Model.ApiResponse Deactivate(int nodeId)
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (member != null)
            {
                var isAdmin = System.Web.Security.Roles.IsUserInRole(member.Username, "Admin");

                if (isAdmin)
                {
                    IContent video = Services.ContentService.GetById(nodeId);
                    if (video != null)
                    {
                        video.SetValue("wwaktiveret", false);
                        Services.ContentService.SaveAndPublish(video);

                        Logger.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Template Video: {member.Name} ({member.Id}) has deactivated Template Video: {video.Name} ({video.Id}) ");

                        return new Model.ApiResponse(true, "OK");
                    }

                    return new Model.ApiResponse(false, "Member was not found.");
                }
                else
                {
                    Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Member has tried to deactivated Template Video ID: {nodeId}. The guilty member that has tried this is {member.Name} ({member.Id})");
                    return new Model.ApiResponse(false, "Your identity is not allowed to change this property! For safety purposes, this is added to the log.");
                }
            }
            else
            {
                Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Unknown member tried to deactivated Template Video ID: {nodeId} ");
                return new Model.ApiResponse(false, "Your identity is unknown! For safety purposes, this is added to the log.");
            }
        }

        [HttpGet]
        public Model.ApiResponse Activate(int nodeId)
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (member != null)
            {
                var isAdmin = System.Web.Security.Roles.IsUserInRole(member.Username, "Admin");

                if (isAdmin)
                {
                    IContent video = Services.ContentService.GetById(nodeId);
                    if (video != null)
                    {
                        video.SetValue("wwaktiveret", true);
                        Services.ContentService.SaveAndPublish(video);

                        Logger.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Template Video: {member.Name} ({member.Id}) has activated Template Video: {video.Name} ({video.Id}) ");

                        return new Model.ApiResponse(true, "OK");
                    }

                    return new Model.ApiResponse(false, "Member was not found.");
                }
                else
                {
                    Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Member has tried to deactivated Template Video ID: {nodeId}. The guilty member that has tried this is {member.Name} ({member.Id})");
                    return new Model.ApiResponse(false, "Your identity is not allowed to change this property! For safety purposes, this is added to the log.");
                }
            }
            else
            {
                Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Unknown member tried to activate Template Video ID: {nodeId} ");
                return new Model.ApiResponse(false, "Your identity is unknown! For safety purposes, this is added to the log.");
            }
        }

        [HttpGet]
        public Model.TemplateVideo GetById(int nodeId)
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (member != null)
            {
                if (nodeId > 0)
                {
                    IContent video = Services.ContentService.GetById(nodeId);
                    if (video != null)
                    {
                        return new StartupCentral.Code.Model.TemplateVideo()
                        {
                            Id = video.Id,
                            Name = video.Name,
                            Key = video.Key,
                            ContentTypeAlias = video.ContentType.Alias
                        };
                    }
                }
            }
            else
            {
                Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Unknown member tried to GetById node ID: {nodeId} ");
            }

            return null;
        }

        [HttpGet]
        public Model.ApiResponse Delete(int nodeId)
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (member != null)
            {
                var isAdmin = System.Web.Security.Roles.IsUserInRole(member.Username, "Admin");

                if (isAdmin)
                {
                    IContent video = Services.ContentService.GetById(nodeId);
                    if (video != null)
                    {
                        Services.ContentService.Delete(video);

                        Logger.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Member: {member.Name} ({member.Id}) has deleted partner: {video.Name} ({video.Id}) ");

                        return new Model.ApiResponse(true, "OK");
                    }

                    return new Model.ApiResponse(false, "Member was not found.");
                }
                else
                {
                    Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Member has tried to delete node ID: {nodeId}. The guilty member that has tried this is {member.Name} ({member.Id})");
                    return new Model.ApiResponse(false, "Your identity is not allowed to change this property! For safety purposes, this is added to the log.");
                }
            }
            else
            {
                Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Unknown member tried to delete node ID: {nodeId} ");
                return new Model.ApiResponse(false, "Your identity is unknown! For safety purposes, this is added to the log.");
            }
        }
    }
}