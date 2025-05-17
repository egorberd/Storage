using Microsoft.Data.Sqlite;
using Storage.Core.Interfaces;


namespace Storage.Database.Repositories
{
    public class SqliteRepository : IStorageRepository
    {
        // Функция для загрузки данных о коробоках из БД
        public List<Box> LoadBoxesFromDatabase()
        {
            List<Box> boxes = new List<Box>();

            // Устанавливаем соединение с БД SqLite
            using SqliteConnection connection = new SqliteConnection(DatabaseInitializer.ConnectionString);
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
                            DateOnly? parsedProductionDate = null;
                            var prodDate = reader["ProductionDate"];
                            if (prodDate != DBNull.Value)
                            {
                                if (DateTime.TryParse(prodDate.ToString(), out var prodData))
                                {
                                    parsedProductionDate = DateOnly.FromDateTime(prodData);
                                }
                                else if (!string.IsNullOrEmpty(prodDate.ToString())) // Если строка не пустая, но не распарсилась
                                    Console.WriteLine("Ошибка: Не удалось преобразовать в нужный формат!");
                            }

                            var parsedExpDate = DateOnly.MinValue;
                            var expDateStr = reader["ExpirationDate"].ToString();
                            if (!string.IsNullOrEmpty(expDateStr))
                            {
                                if (DateTime.TryParse(expDateStr, out var expData))
                                {
                                    parsedExpDate = DateOnly.FromDateTime(expData);
                                }
                            }


                            // Создаем объект класса Box и присваиваем ему данные из БД
                            Box box = new Box(
                                width: Convert.ToDouble(reader["Width"]),
                                height: Convert.ToDouble(reader["Height"]),
                                depth: Convert.ToDouble(reader["Depth"]),
                                weight: Convert.ToDouble(reader["Weight"]),
                                productionDate: parsedProductionDate,
                                expirationDate: parsedExpDate

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
        public List<Pallet> LoadPalletsFromDatabase()
        {
            List<Pallet> pallets = new List<Pallet>();

            // Устанавливаем соединение с БД SqLite
            using (SqliteConnection connection = new SqliteConnection(DatabaseInitializer.ConnectionString))
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
