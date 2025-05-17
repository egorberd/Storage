namespace Storage.Database
{
    public static class DatabaseConfig
    {
        public static void Configure(string? dbFileName = null)
        {
            if (!string.IsNullOrEmpty(dbFileName))
            {
                DatabaseInitializer.SetDatabasePath(dbFileName);
            }
            DatabaseInitializer.InitializeDatabase();
        }
    }
}
