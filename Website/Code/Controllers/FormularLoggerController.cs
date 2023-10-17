using StartupCentral.Code.Class;
using StartupCentral.Code.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Security;
using Umbraco.Web.WebApi;

namespace StartupCentral.Code.Controllers
{
    public class FormularLoggerController : UmbracoApiController
    {
        readonly FormularLoggerService formularLoggerService;
        readonly ActiveCampaignClient activeCampaignClient = new ActiveCampaignClient();
        //private IMember GetMember() => TokenValidator.GetCurrentMember(HttpContext.Current.Request);

        public FormularLoggerController()
        {
            formularLoggerService = new FormularLoggerService();
        }

        [HttpPost]
        public async Task<ApiResponse> Add(FormularLogger model)
        {
            try
            {
                var formid = await formularLoggerService.Save(model);

                return new ApiResponse(true, $"New formular added.", formid);
            }
            catch (Exception e)
            {
                return new ApiResponse(false, $"Adding new formular send an error message: {e.Message}");
            }
        }

        [HttpPost]
        public async Task<ApiResponse> OverriteCheckbox(string propertyName, string recordKey, bool state)
        {
            try
            {
                if (string.IsNullOrEmpty(recordKey))
                {
                    return new ApiResponse(false, $"Record key empty.");
                }
                 
                if (propertyName.Equals("Contacted"))
                {
                    return new ApiResponse(true, $"Contacted property overriten.", await formularLoggerService.UpdateContactedProperty(new Guid(recordKey), state));
                }
                else if (propertyName.Equals("MailFlow"))
                {
                    var res = await formularLoggerService.UpdateMailFlowProperty(new Guid(recordKey), state);

                    if (state && !string.IsNullOrEmpty(res))
                    {
                        await activeCampaignClient.ApiAsync("contact_sync", new Dictionary<string, string>
                        {
                            { "email", res},
                            { "tags", "Hjemmeside pop-up"}
                        });

                        return new ApiResponse(true, $"ActiveCampaign is triggered for the user with email:" + res);
                    }
                    else if (string.IsNullOrEmpty(res))
                    {
                        return new ApiResponse(false, $"Record cannot be found.");
                    }
                    else
                    {
                        return new ApiResponse(true, $"ActiveCampaign property overriten without adding tag.");
                    }              
                }
                else
                {
                    return new ApiResponse(false, $"Unknown property.");
                }
            }
            catch (Exception e)
            {
                return new ApiResponse(false, $"OverriteCheckbox send an error message: {e.Message}");
            }

        }

        [HttpGet]
        public async Task<ApiResponse> GetFormulars()
        {
            //IMember member = GetMember();
            //if (member != null)
            //{
            //var isAdmin = System.Web.Security.Roles.IsUserInRole(member.Username, "Admin");

            //if (isAdmin)
            //{
            var res = await formularLoggerService.GetListOfAllFormulars();

            return new ApiResponse(true, "Formular logs", res.OrderByDescending( x => x.CreateDate));
                //}
            //}

            return new ApiResponse(false, "Token not authorized.");
        }

        [HttpPost]
        public async Task<ApiResponse> MarkRead(string formularKey)
        {
            try
            {
                await formularLoggerService.MarkRead(new Guid(formularKey));

                return new ApiResponse(true, "Message marked as opened.");
            }
            catch (Exception e)
            {
                return new ApiResponse(false, $"MarkRead send an error message: {e.Message}");
            }
            
            
        }

        [HttpGet]
        public async Task<ApiResponse> GetNumberNewFormulars()
        {
            try
            {
                var res = await formularLoggerService.GetListOfUnreadFormulars();

                return new ApiResponse(true, "Message marked as opened.", res.Count());
            }
            catch (Exception e)
            {
                return new ApiResponse(false, $"MarkRead send an error message: {e.Message}");
            }
        }
    }
}