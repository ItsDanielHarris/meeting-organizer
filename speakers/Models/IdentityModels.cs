using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Security.Principal;

namespace speakers.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        private string firstname;
        public String FirstName
        {
            get {
                return firstname;
            }
            set
            {
                firstname = value;
            }
        }

        private string lastname;
        public String LastName
        {
            get
            {
                return lastname;
            }
            set
            {
                lastname = value;
            }
        }

        private Int16 ListOrder;
        public Int16 List_Order
        {
            get
            {
                return ListOrder;
            }
            set
            {
                ListOrder = value;
            }
        }

        private Int16 Logged_In;
        public Int16 LoggedIn
        {
            get
            {
                return Logged_In;
            }
            set
            {
                Logged_In = value;
            }
        }

        private Int32 Time_Left;
        public Int32 TimeLeft
        {
            get
            {
                return Time_Left;
            }
            set
            {
                Time_Left = value;
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}