using StartupCentral.Code.Class;
using System.Web;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;

namespace StartupCentral.Code.ApiControllers
{
    public class SamarbejdspartnerApiController : UmbracoApiController
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
                    IContent partner = Services.ContentService.GetById(nodeId);
                    if (partner != null)
                    {
                        Services.ContentService.Save(partner);

                        Logger.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Samarbejdspartner: {member.Name} ({member.Id}) has deactivated samarbejdspartner: {partner.Name} ({partner.Id}) ");

                        return new Model.ApiResponse(true, "OK");
                    }

                    return new Model.ApiResponse(false, "Member was not found.");
                }
                else
                {
                    Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Member has tried to deactivated samarbejdspartner ID: {nodeId}. The guilty member that has tried this is {member.Name} ({member.Id})");
                    return new Model.ApiResponse(false, "Your identity is not allowed to change this property! For safety purposes, this is added to the log.");
                }
            }
            else
            {
                Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Unknown member tried to deactivated samarbejdspartner ID: {nodeId} ");
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
                    IContent partner = Services.ContentService.GetById(nodeId);
                    if (partner != null)
                    {
                        Services.ContentService.SaveAndPublish(partner);

                        Logger.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Samarbejdspartner: {member.Name} ({member.Id}) has activated samarbejdspartner: {partner.Name} ({partner.Id}) ");

                        return new Model.ApiResponse(true, "OK");
                    }

                    return new Model.ApiResponse(false, "Member was not found.");
                }
                else
                {
                    Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Member has tried to deactivated samarbejdspartner ID: {nodeId}. The guilty member that has tried this is {member.Name} ({member.Id})");
                    return new Model.ApiResponse(false, "Your identity is not allowed to change this property! For safety purposes, this is added to the log.");
                }
            }
            else
            {
                Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Unknown member tried to activate samarbejdspartner ID: {nodeId} ");
                return new Model.ApiResponse(false, "Your identity is unknown! For safety purposes, this is added to the log.");
            }
        }

        [HttpGet]
        public Model.Samarbejdspartner GetById(int nodeId)
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);
            if (member != null)
            {
                if (nodeId > 0)
                {
                    IContent partner = Services.ContentService.GetById(nodeId);
                    if (partner != null)
                    {
                        return new StartupCentral.Code.Model.Samarbejdspartner()
                        {
                            Id = partner.Id,
                            Name = partner.Name,
                            Key = partner.Key,
                            ContentTypeAlias = partner.ContentType.Alias
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
                    IContent partner = Services.ContentService.GetById(nodeId);
                    if (partner != null)
                    {
                        Services.ContentService.Delete(partner);

                        Logger.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Member: {member.Name} ({member.Id}) has deleted partner: {partner.Name} ({partner.Id}) ");

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