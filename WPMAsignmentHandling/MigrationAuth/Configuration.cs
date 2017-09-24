namespace WPMAsignmentHandling.MigrationAuth
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WPMAsignmentHandling.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WPMAsignmentHandling.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"MigrationAuth";
        }

        protected override void Seed(WPMAsignmentHandling.Models.ApplicationDbContext context)
        {
            UserManager<ApplicationUser> usermanger = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            RoleManager<IdentityRole> rolemanger = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));


            if (!rolemanger.RoleExists("Administrator"))
            {
                rolemanger.Create(new IdentityRole("Administrator"));
            }
            if (!rolemanger.RoleExists("MA-Winkdardt"))
            {
                rolemanger.Create(new IdentityRole("Winkhardt-MA"));
            }
            if (!rolemanger.RoleExists("Messe-MA"))
            {
                rolemanger.Create(new IdentityRole("Messe-MA"));
            }

            if (usermanger.Users != null)
            {
                if (!usermanger.Users.Any(n => n.Vorname == "Uwe" && n.Nachname == "Lackner" && n.Email == "uvre@gmx.de"))
                {
                    var user = new ApplicationUser { UserName = "uvre@gmx.de", Vorname = "Uwe", Nachname = "Lackner", Email = "uvre@gmx.de" };
                    usermanger.Create(user, "UweAdmin2015#");
                    context.SaveChanges();
                    usermanger.AddToRole(user.Id, "Administrator");
                }
                else
                {
                    var user = usermanger.Users.Single(n => n.Vorname == "Uwe" && n.Nachname == "Lackner" && n.Email == "uvre@gmx.de");
                    if (!usermanger.IsInRole(user.Id, "Administrator"))
                    {
                        usermanger.AddToRole(user.Id, "Administrator");
                    }
                }
            }
            else
            {
                var user = new ApplicationUser { UserName = "uvre@gmx.de", Vorname = "Uwe", Nachname = "Lackner", Email = "uvre@gmx.de" };
                usermanger.Create(user, "UweAdmin2015#");
                usermanger.AddToRole(user.Id, "Administrator");
            }
        }
    }
}
