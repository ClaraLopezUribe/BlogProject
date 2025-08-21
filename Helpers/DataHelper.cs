// LEARN : I had Commented out this file and left it for reference only. I believe this functionality is handled in the DataService however, after deploying on Railway and comming across errors with Identity pages, I am not sure if this is the case. I will need to test this further.


using BlogProject.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Helpers
{
    public static class DataHelper
    {
        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {
            // Service: Get an instance of db context
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();

            // Migration: This is the programatic equivalent to Update-Database
            await dbContextSvc.Database.MigrateAsync();

        }

    }
}


