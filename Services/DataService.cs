using BlogProject.Data;
using BlogProject.Enums;
using BlogProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;

        // Leverage Constructor Injection (the inversion of control concept and its Dependency Injections) to push an instance of one or more registered services into an instance of this class, then hand itself over to the private variable //
        public DataService(ApplicationDbContext dbContext, 
                           RoleManager<IdentityRole> roleManager, 
                           UserManager<BlogUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        // Wrapper Method //
        public async Task ManageDataAsync()
        {
            // Programatically create the DB from Migrations
            await _dbContext.Database.MigrateAsync();

            // Seed a few Roles into the system
            await SeedRolesAsync();

            // Seed a few Users into the system (programatically)
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            // If there are already Roles in the system, do nothing
            if (_dbContext.Roles.Any())
            {
                return;
            }

            // Otherwise, create Roles
            foreach (var role in Enum.GetNames(typeof(BlogRole)))
            {
                // Use the RoleManager to create roles 
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

        }

        private async Task SeedUsersAsync()
        {
            // If there are already any Users in the system, do nothing
            if (_dbContext.Users.Any())
            {
                return;
            }


            // Otherwise, create a few Users:

            // Admin Role User
            //Step 1.1: Create a new instance of BlogUser
            var adminUser = new BlogUser()
            {
                Email = "claralopezuribe.developer@gmail.com",
                UserName = "claralopezuribe.developer@gmail.com",
                FirstName = "Clara",
                LastName = "Lopez-Uribe",
                PhoneNumber = "(123) 456-7890",
                EmailConfirmed = true
            };

            //Step 2.1: Use the UserManager to create a new user that is defined by adminUser variable
            await _userManager.CreateAsync(adminUser, "AdminTempP@ssw0rd");

            //Step 3.1: Add this new user to the Aministrator role
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());



            // Moderator Role User
            //Step 1.2: Create a new instance of BlogUser
            var modUser = new BlogUser()
            {
                Email = "cluribelopez@gmail.com",
                UserName = "cluribelopez@gmail.com",
                FirstName = "Clarita",
                LastName = "Lopez",
                PhoneNumber = "(123) 456-7891",
                EmailConfirmed = true
            };

            //Step 2.2: Use the UserManager to create a new user that is defined by modUser variable
            await _userManager.CreateAsync(modUser, "ModTempP@ssw0rd");

            //Step 3.2: Add this new user to the Moderator role
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());





            // TODO : Seed Guest Author Role User
            //Step 1.3: Create a new instance of BlogUser


            //Step 2.3: Use the UserManager to create a new user that is defined by GuestAuthor variable


            //Step 3.3: Add this new user to the GuestAuthor role

        }

    }
}
