using Microsoft.Owin;
using Owin;
using Hangfire;
using WPMAsignmentHandling.Additional;


[assembly: OwinStartupAttribute(typeof(WPMAsignmentHandling.Startup))]
namespace WPMAsignmentHandling
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            GlobalConfiguration.Configuration.UseSqlServerStorage("DMS_Winkhardt_DB");
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            //RecurringJob.AddOrUpdate("Check-if-Reminder-should-be-sent", () => Extensions.CheckDateAndSendSpecificMailReminder(), "0 0 * * *");
        }
    }
}
