using Npgsql;

namespace BlogProject.Helpers
{
    public static class ConnectionHelper
    {
        public static string ConnectionString(IConfiguration configuration)
        {
            // Get the connection string from the configuration
            var connectionString = configuration.GetSection("pgSettings")["pgConnection"];

            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);

        }

        // Build the connection string from the environment variable (i.e.  Heroku or Railway)
        private static string BuildConnectionString(string databaseUrl)
        {
            // Parse the database URL
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require
            };

            return builder.ToString();
        }
    }
}
