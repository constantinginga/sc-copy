using Hangfire;
using Microsoft.Owin;
using Owin;
using StartupCentral.Hangfire;
using System;
using Umbraco.Web;

[assembly: OwinStartup("UmbracoStandardOwinStartup", typeof(UmbracoStandardOwinStartup))]
namespace StartupCentral.Hangfire
{
    public class UmbracoStandardOwinStartup : UmbracoDefaultOwinStartup
    {

        public override void Configuration(IAppBuilder app)
        {
            //ensure the default options are configured
            base.Configuration(app);
            try
            {
                var dashboardOptions = new DashboardOptions { Authorization = new[] { new UmbracoAuthorizationFilter() } };

                app.UseHangfireDashboard("/hangfire", dashboardOptions);
                app.UseHangfireServer();


                //to stop hangfire from auto retry 
                GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

                RecurringJob.AddOrUpdate<InvitationJobs>("RemoveDuplicates", x => x.CleanUpDuplicateBasicMembers(), "00 04 * * *");
                RecurringJob.AddOrUpdate<InvitationJobs>("Update unsubscribed users", x => x.UpdateUnsubscribedUsers(), "00 01 * * *");
                RecurringJob.AddOrUpdate<InvitationJobs>("Daily Logins", x => x.UpdateLoginList(), "00 00 * * *", TimeZoneInfo.Local);
                RecurringJob.AddOrUpdate<InvitationJobs>("CleanUp new un-approved duplicate members", x => x.CleanUpNewUnApprovedDuplicateMembers(), "00 02 * * *");
                RecurringJob.AddOrUpdate<InvitationJobs>("Messaging all students", x => x.EmailingAllStudentsAsync(), "0 0 29 2 WED");
                RecurringJob.AddOrUpdate<InvitationJobs>("Messaging all coaches", x => x.EmailingAllCochesAsync(), "0 0 29 2 WED");

                //RecurringJob.AddOrUpdate<InvitationJobs>("Message Bad Payers", x => x.MessageBadPayers(), "0 0 29 2 WED");

            }
            catch (Exception)
            {

            }
        }
    }
}