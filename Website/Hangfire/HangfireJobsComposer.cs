using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace StartupCentral.Hangfire
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class HangfireJobsComposer : IComposer
    {

        public void Compose(Composition composition)
        {

            // Configure hangfire
            var options = new SqlServerStorageOptions { PrepareSchemaIfNecessary = true };
            var connectionString = System.Configuration
                .ConfigurationManager
                .ConnectionStrings["umbracoDbDSN"]
                .ConnectionString;

            var container = composition.Concrete as LightInject.ServiceContainer;

            GlobalConfiguration.Configuration
                .UseSqlServerStorage(connectionString, options)
                .UseConsole()
                .UseLightInjectActivator(container);

            composition.Register<InvitationJobs>(Lifetime.Singleton);

        }
    }
}