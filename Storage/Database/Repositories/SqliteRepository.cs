using Microsoft.Data.Sqlite;
using Storage.Core.Interfaces;
using Storage.Core.Models;


namespace Storage.Database.Repositories
{
    public class SqliteRepository : IStorageRepository
    {
        // Функция для загрузки данных о коробоках из БД
        public List<Box> LoadBoxesFromDatabase()
        {
            var boxes = new List<Box>();

            // Устанавливаем соединение с БД SqLite
            using var connection = new SqliteConnection(DatabaseInitializer.ConnectionString);
            {
                connection.Open();

                // Пишем SQL запрос для получения всех данных таблицы Boxes
                var query = "SELECT ID, Width, Height, Depth, Weight, ProductionDate, ExpirationDate FROM Boxes";
                using var command = new SqliteCommand(query, connection);
                {
                    using var reader = command.ExecuteReader();
                    {
                        while (reader.Read())
                        {
                            //Т.к. SqLite хранит дату в текстовом формате мы парсим её чтоб преобразовать в DateOnly
                            DateOnly? parsedProductionDate = null;
                            var prodDate = reader["ProductionDate"];
                            if (prodDate != DBNull.Value && !string.IsNullOrEmpty(prodDate.ToString()))
                            {
                                if (DateTime.TryParse(prodDate.ToString(), out var prodData))
                                {
                                    parsedProductionDate = DateOnly.FromDateTime(prodData);
                                }
                                else
                                {
                                    Console.WriteLine($"Ошибка: Не удалось преобразовать дату производства '{prodDate}' в нужный формат!");
                                }

                            }

                            var parsedExpDate = DateOnly.MinValue;
                            var expDate = reader["ExpirationDate"];
                            if (!string.IsNullOrEmpty(expDate.ToString()))
                            {
                                if (DateTime.TryParse(expDate.ToString(), out var expData))
                                {
                                    parsedExpDate = DateOnly.FromDateTime(expData);
                                }
                                else
                                {
                                    Console.WriteLine($"Ошибка: Не удалось преобразовать срок годности '{expDate}' в нужный формат!");
                                }
                            }

                            // Проверяем заданы ли дата производства и срок годности
                            if (parsedProductionDate == null && parsedExpDate == default)
                            {
                                Console.WriteLine("Предупреждение: У коробки не указана дата производства и срок годности!");
                                // В таком случае введем срок годности +100 дней от текущей даты
                                parsedExpDate = DateOnly.FromDateTime(DateTime.Now.AddDays(100));
                            }

                            // Создаем объект класса Box и присваиваем ему данные из БД
                            var box = new Box(
                                width: Convert.ToDouble(reader["Width"]),
                                height: Convert.ToDouble(reader["Height"]),
                                depth: Convert.ToDouble(reader["Depth"]),
                                weight: Convert.ToDouble(reader["Weight"]),
                                productionDate: parsedProductionDate,
                                expirationDate: parsedExpDate
                             )
                            {
                                DbId = Convert.ToInt32(reader["ID"]) // Сохраняем ID из базы данных
                            };
                            boxes.Add(box);
                        }
                    }
                }
            }
            return boxes;
        }

        // Функция для загрузки данных о паллетах из БД SqLite
        public List<Pallet> LoadPalletsFromDatabase()
        {
            var pallets = new List<Pallet>();

            // Устанавливаем соединение с БД SqLite
            using var connection = new SqliteConnection(DatabaseInitializer.ConnectionString);
            {
                connection.Open();

                // Пишем SQL запрос для получения всех данных таблицы Pallets
                var query = "SELECT ID, Width, Height, Depth FROM Pallets";
                using var command = new SqliteCommand(query, connection);
                {
                    using var reader = command.ExecuteReader();
                    {
                        while (reader.Read())
                        {
                            // Создаем объект класса Pallet и присваиваем ему данные из БД
                            var pallet = new Pallet(
                                width: Convert.ToDouble(reader["Width"]),
                                height: Convert.ToDouble(reader["Height"]),
                                depth: Convert.ToDouble(reader["Depth"])
                            )
                            {
                                DbId = Convert.ToInt32(reader["ID"]) // Сохраняем ID из базы данных
                            };
                            pallets.Add(pallet);
                        }
                    }
                }
            }
            return pallets;
        }
    }
}
