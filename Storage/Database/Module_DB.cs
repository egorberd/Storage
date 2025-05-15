using Microsoft.Data.Sqlite;
using System.Globalization;

namespace Storage.Database
{
    public class Module_DB
    {
        // Инициализируем переменную которая будет хранить имя файла БД
        private static string dbFileName = "storage.db";

        // Объявляем строку для подключение к БД
        private static string connectionString = $"Data Source={dbFileName};";

        // Функция основного взаимодействия с БД
        public static void DataBaseInteraction()
        {
            // Настройка привязки библиотеки для последующего подключения к БД
            SQLitePCL.Batteries.Init();

            // Проверка на наличие файла базы данных в директории запуска
            if (!File.Exists(dbFileName))
            {
                Console.WriteLine($"Файл базы данных '{dbFileName}' не найден.");
            }

            // Инициализируем два списка объектов класса Box и Pallet
            List<Box> boxes = LoadBoxesFromDatabase(); // Вызываем метод для выгрузки данных коробок из БД
            List<Pallet> pallets = LoadPalletsFromDatabase(); // Вызываем метод для выгрузки данных паллет из БД

            // Проверка на наличие данных в списках паллет и коробок
            if (pallets.Any() && boxes.Any())
            {
                foreach (var box in boxes)
                {
                    pallets[0].AddBox(box); // Добавляем все коробки к первой паллете
                }
            }

            // Выводим информацию по коробкам в консоль
            Console.WriteLine("Коробки: ");
            foreach (var box in boxes)
            {
                Console.WriteLine(box);
            }

            // Выводим информацию по паллетам в консоль
            Console.WriteLine("\nПаллеты: ");
            foreach (var pallet in pallets)
            {
                Console.WriteLine(pallet);

                // Выводим коробки стоящие на определенной паллете:
                if (pallet.boxes.Any())
                {
                    Console.WriteLine("  Коробки на этой паллете: ");
                    foreach (var boxOnPallet in pallet.boxes)
                    {
                        Console.WriteLine($"    - {boxOnPallet}");
                    }
                }
            }
        }

        // Функция для загрузки данных о коробоках из БД
        private static List<Box> LoadBoxesFromDatabase()
        {
            List<Box> boxes = new List<Box>();

            // Устанавливаем соединение с БД SqLite
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                // Пишем SQL запрос для получения всех данных таблицы Boxes
                string query = "SELECT ID, Width, Height, Depth, Weight, ProductionDate, ExpirationDate FROM Boxes";
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Создаем объект класса Box и присваиваем ему данные из БД
                            Box box = new Box(
                                Width: Convert.ToDouble(reader["Width"]),
                                Height: Convert.ToDouble(reader["Height"]),
                                Depth: Convert.ToDouble(reader["Depth"]),
                                Weight: Convert.ToDouble(reader["Weight"]),

                                // т.к. SQLite хранит дату как текст мы используем DateTime.ParseExact
                                ExpirationDate: DateOnly.FromDateTime(DateTime.ParseExact(reader["ExpirationDate"].ToString(), "MM/d/yyyy", CultureInfo.InvariantCulture)),
                                ProductionDate: reader["ProductionDate"] == DBNull.Value || string.IsNullOrEmpty(reader["ProductionDate"].ToString())
                                    ? (DateOnly?)null
                                    : DateOnly.FromDateTime(DateTime.ParseExact(reader["ProductionDate"].ToString(), "MM/d/yyyy", CultureInfo.InvariantCulture))
                            );
                            box.DbId = Convert.ToInt32(reader["ID"]); // Сохраняем ID из базы данных
                            boxes.Add(box);
                        }
                    }
                }
            }
            return boxes;
        }

        // Функция для загрузки данных о паллетах из БД SqLite
        private static List<Pallet> LoadPalletsFromDatabase()
        {
            List<Pallet> pallets = new List<Pallet>();

            // Устанавливаем соединение с БД SqLite
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                // Пишем SQL запрос для получения всех данных таблицы Pallets
                string query = "SELECT ID, Width, Height, Depth FROM Pallets";
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Создаем объект класса Pallet и присваиваем ему данные из БД
                            Pallet pallet = new Pallet(
                                Width: Convert.ToDouble(reader["Width"]),
                                Height: Convert.ToDouble(reader["Height"]),
                                Depth: Convert.ToDouble(reader["Depth"])
                            );
                            pallet.DbId = Convert.ToInt32(reader["ID"]); // Сохраняем ID из базы данных
                            pallets.Add(pallet);
                        }
                    }
                }
            }
            return pallets;
        }
    }
}
