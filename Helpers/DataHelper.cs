// LEARN : Isnt this file redundant??? It looks to me that the implementation is handled in the DataService class.


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


