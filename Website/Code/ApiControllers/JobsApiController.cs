using StartupCentral.Code.Class;
using StartupCentral.DAL.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;
using Upodi.SDK;

namespace StartupCentral.Code.ApiControllers
{
    public class JobsApiController : UmbracoApiController
    {

        bool isProd = Environment.MachineName.Equals("startupVM");

        [HttpGet]
        public bool test()
        {
            return true;
        }

        [HttpGet]
        public async Task<int> UnsubscribeUsers()
        {
            if (!isProd) return 0;

            int students = 0;

            var upodiWrapperService = new Class.UpodiWrapperService();
            var activeCampaignClient = new Class.ActiveCampaignClient();

            var l = Services.MemberService.GetMembersByMemberType("student").Where(x => x.GetValue<DateTime>("unsubscribeDate") != null
                                                                                     && x.GetValue<DateTime>("unsubscribeDate") != DateTime.MinValue
                                                                                     && x.GetValue<DateTime>("unsubscribeDate") < DateTime.Today
                                                                                     //&& x.GetValue<DateTime>("unsubscribeDate").Date == DateTime.Today
                                                                                     && x.IsApproved).ToList();
            students = l.Count;
            foreach (var mem in l)
            {
                var i = upodiWrapperService.GetByAccountNumber(mem.Id);
                if (i != null)
                {
                   await upodiWrapperService.DeactivateSubscription(mem);
                }

                mem.IsApproved = false;
                Services.MemberService.Save(mem, true);

                var result = activeCampaignClient.ApiAsync("contact_sync", new Dictionary<string, string>
                {
                     {"email", mem.Email.ToString()},
                     {"tags", "Opsagt"}
                });
            }

            return students;
        }

        [HttpGet]
        public async Task<int> DailyActivMembers()
        {
            if (!isProd) return 0;

            int logins = 0;
            DateTime now = DateTime.Now;

            var l = Services.MemberService.GetMembersByMemberType("student").Where(x => x.LastLoginDate > now.AddHours(-24) && x.LastLoginDate <= now).ToList();
            ActiveMembersService service = new ActiveMembersService();
            foreach (var mem in l)
            {                
               await service.Add(mem.Id, mem.LastLoginDate);   
            }
            logins = l.Count;
            return logins;
        }

        [HttpGet]
        public int CleanUpNewUnApprovedDuplicateMembers()
        {
            if (!isProd) return 0;

            DateTime now = DateTime.Now;

            var newMembersOfTheDay = Services.MemberService.GetMembersByMemberType("student").Where(x => x.CreateDate > now.AddDays(-7)).ToList();

            var toDelete = new List<IMember>();
            foreach (var mem in newMembersOfTheDay)
            {
                if (!mem.IsApproved)
                {
                    var approvedMatch = newMembersOfTheDay.FirstOrDefault(x => x.IsApproved
                        && string.Equals(x.Username, mem.Username, StringComparison.InvariantCultureIgnoreCase));
                    if (approvedMatch != null)
                    {
                        toDelete.Add(mem);
                    }
                }
            }

            foreach (var memToDelete in toDelete)
            {
                Services.MemberService.Delete(memToDelete);
            }

            return toDelete.Count;
        }

        [HttpGet]
        public int CleanUpDuplicateBasicMembers()
        {
            if (!isProd) return 0;

            DateTime now = DateTime.Now;

            var newMembersOfTheDay = Services.MemberService.GetMembersByMemberType("student").Where(x => x.CreateDate > now.AddDays(-1) && x.IsApproved == false).ToList();
            
            var cleanList = newMembersOfTheDay.GroupBy(x => x.Email).Select(x => x.First()).ToList();
            int count = 0;
            foreach (var mem in newMembersOfTheDay)
            {
                if(!cleanList.Contains(mem))
                {
                    Services.MemberService.Delete(mem);
                    count++;
                }
                
            }


            return count;
        }

        [HttpGet]
        public string ConvertUpodiAccountNumberFromGuidToId()
        {
            if (!isProd) return "0";

            // One time run - then this method can be deleted
            var accessKey = System.Configuration.ConfigurationManager.AppSettings["StartupCentral.Upodi.AccessKey"];
            UpodiClient upodi = new UpodiClient(accessKey);

            int count = 0;
            int converted = 0;
            var allMembers = Services.MemberService.GetAll(0, int.MaxValue, out long totalRecords);

            foreach (var member in allMembers)
            {
                count++;
                try
                {
                    //var customer = upodi.Customers.GetByAccountNumber(member.Key.ToString());
                    //if (customer == null) continue;

                    //customer.AccountNumber = member.Id.ToString("D");
                    //upodi.Customers.Update(customer);

                    converted++;
                }
                catch (Exception err)
                {
                    Logger.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        "Problem with member node id: " + member.Id + Environment.NewLine + err);
                }
            }
            return "Count:" + count + " - Converted:" + converted;
        }

    }
}