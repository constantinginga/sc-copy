using StartupCentral.Code.Class;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace StartupCentral.Code.EventHandlers
{
    // overrides the created event (after Save)
    public class UmbracoEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            MemberService.Created += MemberService_Created;
        }

        private void MemberService_Created(IMemberService sender, Umbraco.Core.Events.NewEventArgs<IMember> e)
        {
            if (e.Entity != null)
            {
                switch (e.Entity.ContentType.Alias)
                {
                    case "coach1":
                    case "investor":
                    case "teacher":
                        string password = Passwords.GenerateNewPassword();
                        ApplicationContext.Current.Services.MemberService.SavePassword(e.Entity, password);
                        Emailing.SendCreationEmail(e.Entity, password);
                        break;
                    case "student":
                        break;
                    default:
                        break;
                }
            }

        }
    }
}