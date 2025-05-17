namespace Storage.Database
{
    public static class DatabaseInitializer
    {
        // Инициализируем переменную которая будет хранить имя файла БД
        private static string dbFileName = "storageDB.db";

        // Объявляем строку для подключение к БД
        public static string ConnectionString = $"Data Source={dbFileName};";

        // Функция основного взаимодействия с БД
        public static void InitializeDatabase()
        {
            // Настройка привязки библиотеки для последующего подключения к БД
            SQLitePCL.Batteries.Init();

            // Проверка на наличие файла базы данных в директории запуска
            if (!File.Exists(dbFileName))
            {
                Console.WriteLine($"Файл базы данных '{dbFileName}' не найден.");
            }
        }

        public static void SetDatabasePath(string customPath)
        {
            dbFileName = customPath;
        }
    }
}
