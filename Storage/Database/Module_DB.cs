using Microsoft.Data.Sqlite;
using System.Data;
using System.Globalization;

namespace Storage.Database
{
    public class Module_DB
    {
        // Инициализируем переменную которая будет хранить имя файла БД
        private static string dbFileName = "storageDB.db";

        // Объявляем строку для подключение к БД
        private static string connectionString = $"Data Source={dbFileName};";

        // Функция основного взаимодействия с БД
        public static void OpeningDataBase()
        {
            // Настройка привязки библиотеки для последующего подключения к БД
            SQLitePCL.Batteries.Init();

            // Проверка на наличие файла базы данных в директории запуска
            if (!File.Exists(dbFileName))
            {
                Console.WriteLine($"Файл базы данных '{dbFileName}' не найден.");
            }

        }

        // Функция для загрузки данных о коробоках из БД
        public static List<Box> LoadBoxesFromDatabase()
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
                            DateOnly? parseProductionDate = null;
                            if (reader["ProductionDate"] != DBNull.Value)
                            {
                                string prodDateStr = reader["ProductionDate"].ToString();
                                if (DateTime.TryParse(prodDateStr, out var prodData))
                                {
                                    parseProductionDate = DateOnly.FromDateTime(prodData);
                                }
                                else if (!string.IsNullOrEmpty(prodDateStr)) Console.WriteLine("Ошибочка!");
                            }

                            DateOnly parseExpDate = DateOnly.MinValue;
                            if (reader["ExpirationDate"] != DBNull.Value)
                            {
                                string expDateStr = reader["ExpirationDate"].ToString();
                                if (DateTime.TryParse(expDateStr, out var expData))
                                {
                                    parseExpDate = DateOnly.FromDateTime(expData);
                                }
                                else if (!string.IsNullOrEmpty(expDateStr)) Console.WriteLine("Ошибочка!");
                            }

                            // Создаем объект класса Box и присваиваем ему данные из БД
                            Box box = new Box(
                                Width: Convert.ToDouble(reader["Width"]),
                                Height: Convert.ToDouble(reader["Height"]),
                                Depth: Convert.ToDouble(reader["Depth"]),
                                Weight: Convert.ToDouble(reader["Weight"]),
                                ProductionDate: parseProductionDate,
                                ExpirationDate: parseExpDate

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
        public static List<Pallet> LoadPalletsFromDatabase()
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
