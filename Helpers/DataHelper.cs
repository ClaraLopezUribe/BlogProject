using BlogProject.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Helpers
{
    public class DataHelper
    {
        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {
            // Service: an instance of db context
            var dbContext = svcProvider.GetRequiredService<ApplicationDbContext>();

            // Migration: This is the programatic equivalent of running the command "Update-Database" in Package Manager Console
            await dbContext.Database.MigrateAsync();
        }
    }
}
