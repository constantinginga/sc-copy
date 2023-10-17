using StartupCentral.Code.Class;
using StartupCentral.Code.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;

namespace StartupCentral.Code.ApiControllers
{
    public class ActivecampaignApiController : UmbracoApiController
    {
        readonly ActiveCampaignClient client = new ActiveCampaignClient();
        //private IMember GetMember() => TokenValidator.GetCurrentMember(HttpContext.Current.Request);
        
        [AcceptVerbs("GET", "POST")]
        public async Task<ApiResponse> Contact_sync_lead(string name, string email)
        {
            return await Execute_call_contact_sync(name, email, "Lead");
        }

        #region Default Before
        [AcceptVerbs("GET", "POST")]
        public async Task<ApiResponse> Contact_fejletsignup()
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);

            return await Execute_call_contact_sync(member.Name, member.Email, "Fejlet signup");
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<ApiResponse> Contact_fejletsignup_nomarketing()
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);

            return await Execute_call_contact_sync(member.Name, member.Email, "Fejlet signup, No-marketing");
        }
        #endregion

        #region Special
        [AcceptVerbs("GET", "POST")]
        public async Task<ApiResponse> Contact_fejletsignup_special()
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);

            return await Execute_call_contact_sync(member.Name, member.Email, "Fejlet signup, Særtilbud");
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<ApiResponse> Contact_fejletsignup_nomarketing_special()
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);

            return await Execute_call_contact_sync(member.Name, member.Email, "Fejlet signup, No-marketing, Særtilbud");
        }
        #endregion

        #region Default After
        [AcceptVerbs("GET", "POST")]
        public async Task<ApiResponse> Contact_sync()
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);

            return await Execute_call_contact_sync(member.Name, member.Email, "Nyt medlem");
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<ApiResponse> Contact_sync_basic()
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);

            return await Execute_call_contact_sync(member.Name, member.Email, "BasicMembership");
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<ApiResponse> Contact_sync_nomarketingmember()
        {
            IMember member = TokenValidator.GetCurrentMember(HttpContext.Current.Request);

            return await Execute_call_contact_sync(member.Name, member.Email, "No-marketing medlem");
        }
        #endregion

        public async Task<ApiResponse> Execute_call_contact_sync(string name, string email, string key)
        {
            var result = await client.ApiAsync("contact_sync", new Dictionary<string, string>
            {
                 {"email", email},
                 {"first_name", name},
                 {"tags", key}
            });

            return !result.Success ? new ApiResponse(false, result.Message.ToString()) : new ApiResponse(true, result.Message.ToString());
        }

    }
}