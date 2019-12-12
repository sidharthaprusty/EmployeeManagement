using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace EmployeeManagement.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("FirstName", this.FirstName));
            return userIdentity;
        }
    }

    //Identity Roles
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base(){}
        public ApplicationRole(string roleName) : base(roleName) { }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<State> States { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        // public System.Data.Entity.DbSet<EmployeeManagement.Models.RoleViewModel> RoleViewModels { get; set; }

        //public System.Data.Entity.DbSet<EmployeeManagement.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
    
}