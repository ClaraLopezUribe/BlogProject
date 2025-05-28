/* LEARN : How can I back up the data in database programatically? Migrations are for creating & updating the database structure, but not for backing up data. How do I safeguard from losing critical user data? */

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


            /* TODO : Define the permissions granted to each role in the system. For example, the Administrator role can create, edit, and delete blogs, posts, and comments of all users. The Moderator role can only edit and delete comments, except for their own original comments. The Guest Author role can only create, edit, and delete their own blogs and posts, including posts under another author's blog (maybe...tbd) */


            // ADMIN ROLE USER
            //Step 1.1: Create a new instance of BlogUser
            var adminUser = new BlogUser()
            {
                Email = "claralopezuribe.developer@gmail.com",
                UserName = "claralopezuribe.developer@gmail.com",
                FirstName = "Clara",
                LastName = "Lopez-Uribe",
                PhoneNumber = "(123) 456-7890",
                EmailConfirmed = true,
                DisplayName = "Clara Lopez-Uribe"
            };

            //Step 2.1: Use the UserManager to create a new user that is defined by adminUser variable
            await _userManager.CreateAsync(adminUser, "AdminTempP@ssw0rd");

            //Step 3.1: Add this new user to the Aministrator role
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());



            // MODERATOR ROLE USER
            //Step 1.2: Create a new instance of BlogUser
            var modUser = new BlogUser()
            {
                Email = "cluribelopez@gmail.com",
                UserName = "cluribelopez@gmail.com",
                FirstName = "Clarita",
                LastName = "Lopez",
                PhoneNumber = "(123) 456-7891",
                EmailConfirmed = true,
                DisplayName = "Clarita Lopez"
            };

            //Step 2.2: Use the UserManager to create a new user that is defined by modUser variable
            await _userManager.CreateAsync(modUser, "ModTempP@ssw0rd");

            //Step 3.2: Add this new user to the Moderator role
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());





            /* FEATURE : Seed Guest Author Role User. This may be a good solution to DEMO this project by creating a user that is not an administrator or moderator, but still has access to the system. This user can be used to test the system without having to create a new user every time. */

            // GUEST AUTHOR USER : Can create their own blogs and posts, but cannot edit or delete other users' blogs or posts. They can only view the blogs and posts of other users. They can also comment on blogs and posts, but cannot moderate comments. They can also view their own profile and edit their own profile information, but cannot edit other users' profiles. They can also view the list of all blogs and posts, but cannot edit/delete the details of other users' blogs and posts. They can also view the list of all comments, but cannot view the details of the comment's users. They can also view the list of all roles, but cannot view the details of other users' roles. They can also view the list of all permissions, but cannot view the details of other users' permissions.
            //Step 1.3: Create a new instance of BlogUser


            //Step 2.3: Use the UserManager to create a new user that is defined by GuestAuthor variable


            //Step 3.3: Add this new user to the GuestAuthor role


            /* LEARN : How can a registered user apply for Guest Author Role permission? add a new view, or link to existing contact page? Where is the best location for a user to interact with this option? */


        }

    }
}
