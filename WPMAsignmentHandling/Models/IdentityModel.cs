using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WPMAsignmentHandling.Models
{

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser:IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        [Required(ErrorMessage = "Vorname nicht eingetragen!")]
        [StringLength(32, ErrorMessage = "Maxiaml sind 32 Zeichen möglich")]
        public string Vorname { get; set; }
        [Required(ErrorMessage = "Nachname nicht eingetragen!")]
        [StringLength(32, ErrorMessage = "Maxiaml sind 32 Zeichen möglich")]
        public string Nachname { get; set; }
        [StringLength(32, ErrorMessage = "Maxiaml sind 32 Zeichen möglich")]
        public string Telefonnummer { get; set; }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DMS_Winkhardt_DB", throwIfV1Schema: false)
        {

        }

        public static ApplicationDbContext Create()
        {
            

            return new ApplicationDbContext();
        }
    }
}