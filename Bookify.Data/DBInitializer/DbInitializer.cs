using Bookify.Models;
using Bookify.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Data.DBInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }


        public void Initialize()
        {
            //migrations if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            //create roles if they are not created
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();

                //if roles are not created, then we will create admin user as well
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "dren@gmail.com",
                    Email = "dren@gmail.com",
                    Name = "Dren Kastrati",
                    PhoneNumber = "+383 48121051",
                    StreetAddress = "Lidhja e Lezhes",
                    State = "Kosova",
                    PostalCode = "10000",
                    City = "Prishtine"
                }, "Dren123!").GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "blend@gmail.com",
                    Email = "blend@gmail.com",
                    Name = "Blend Sejdiu",
                    PhoneNumber = "+383 45860810",
                    StreetAddress = "Lidhja e Prizrenit",
                    State = "Kosova",
                    PostalCode = "10000",
                    City = "Prishtine"
                }, "Blend123!").GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "era@gmail.com",
                    Email = "era@gmail.com",
                    Name = "Era Qerimi",
                    PhoneNumber = "+383 48125725",
                    StreetAddress = "Lidhja e Shkodres",
                    State = "Kosova",
                    PostalCode = "10000",
                    City = "Gjilan"
                }, "Era123!").GetAwaiter().GetResult();


                ApplicationUser user1 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "dren@gmail.com");
                _userManager.AddToRoleAsync(user1, SD.Role_Admin).GetAwaiter().GetResult();

                ApplicationUser user2 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "blend@gmail.com");
                _userManager.AddToRoleAsync(user2, SD.Role_Customer).GetAwaiter().GetResult();

                ApplicationUser user3 = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "era@gmail.com");
                _userManager.AddToRoleAsync(user3, SD.Role_Customer).GetAwaiter().GetResult();

            }
            return;
        }
    }
}
