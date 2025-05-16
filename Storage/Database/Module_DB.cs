using Microsoft.Data.Sqlite;
using System.Data;

namespace Storage.Database
{
    public class Module_DB
    {
        // Инициализируем переменную которая будет хранить имя файла БД
        private static string dbFileName = "storageDB.db";

        // Объявляем строку для подключение к БД
        private static string connectionString = $"Data Source={dbFileName};";

        private const int daysExp = 100;

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
            using SqliteConnection connection = new SqliteConnection(connectionString);
            {
                connection.Open();

                // Пишем SQL запрос для получения всех данных таблицы Boxes
                var query = "SELECT ID, Width, Height, Depth, Weight, ProductionDate, ExpirationDate FROM Boxes";
                using SqliteCommand command = new SqliteCommand(query, connection);
                {
                    using SqliteDataReader reader = command.ExecuteReader();
                    {
                        while (reader.Read())
                        {
                            //Т.к. SqLite хранит дату в текстовом формате мы парсим её чтоб преобразовать в DateOnly
                            DateOnly? parseProductionDate = null;
                            if (reader["ProductionDate"] != DBNull.Value)
                            {
                                var prodDateStr = reader["ProductionDate"].ToString();
                                if (DateTime.TryParse(prodDateStr, out var prodData))
                                {
                                    parseProductionDate = DateOnly.FromDateTime(prodData);
                                }
                                else if (!string.IsNullOrEmpty(prodDateStr)) // Если строка не пустая, но не распарсилась
                                    Console.WriteLine("Ошибка: Не удалось преобразовать в нужный формат!");
                            }

                            var parseExpDate = DateOnly.MinValue;
                            if (!string.IsNullOrEmpty(reader["ExpirationDate"].ToString()))
                            {
                                var expDateStr = reader["ExpirationDate"].ToString();
                                if (DateTime.TryParse(expDateStr, out var expData))
                                {
                                    parseExpDate = DateOnly.FromDateTime(expData);
                                }
                            }
                            else parseExpDate = parseProductionDate.Value.AddDays(daysExp);

                            // Создаем объект класса Box и присваиваем ему данные из БД
                            Box box = new Box(
                                width: Convert.ToDouble(reader["Width"]),
                                height: Convert.ToDouble(reader["Height"]),
                                depth: Convert.ToDouble(reader["Depth"]),
                                weight: Convert.ToDouble(reader["Weight"]),
                                productionDate: parseProductionDate,
                                expirationDate: parseExpDate

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
                var query = "SELECT ID, Width, Height, Depth FROM Pallets";
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Создаем объект класса Pallet и присваиваем ему данные из БД
                            Pallet pallet = new Pallet(
                                width: Convert.ToDouble(reader["Width"]),
                                height: Convert.ToDouble(reader["Height"]),
                                depth: Convert.ToDouble(reader["Depth"])
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
