using Storage.Database;
using Storage.Database.Repositories;

namespace Storage
{
    public class Program
    {
        public static void Main()
        {
            DatabaseConfig.Configure("storageDB.db");

            // Инициализируем два списка объектов класса Box и Pallet
            var boxes = new SqliteRepository().LoadBoxesFromDatabase(); // Вызываем метод для выгрузки данных коробок из БД
            var pallets = new SqliteRepository().LoadPalletsFromDatabase(); // Вызываем метод для выгрузки данных паллет из БД

            // Добавляем коробки на паллеты
            if (pallets.Count != 0 && boxes.Count != 0)
            {
                pallets[0].AddBox(boxes[0]);
                pallets[0].AddBox(boxes[1]);
                pallets[1].AddBox(boxes[2]);
                pallets[1].AddBox(boxes[3]);
                pallets[2].AddBox(boxes[4]);
                pallets[2].AddBox(boxes[5]);
                pallets[2].AddBox(boxes[6]);
                pallets[3].AddBox(boxes[7]);
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
                Console.WriteLine($"\n{pallet}");

                if (pallet.Boxes.Count == 0)
                {
                    continue;
                }

                // Выводим коробки стоящие на определенной паллете:
                Console.WriteLine("  Коробки на этой паллете: ");
                foreach (var boxOnPallet in pallet.Boxes)
                {
                    Console.WriteLine($"\t- {boxOnPallet}");
                }
            }

            //---------------------LINQ-----------------------
            Console.WriteLine("\nГруппировка паллет по сроку годности, сортировка по сроку годности и весу: ");
            var palletByExpiration = pallets
                .GroupBy(p => p.ExpirationDate) // Группировка по сроку годности
                .OrderBy(g => g.Key) // Сортировка по сроку годности
                .Select(g => new // Вложенный запрос для сортировки по весу внутри группы
                {
                    ExpirationDate = g.Key,
                    Pallets = g.OrderBy(p => p.Weight).ToList() // Сортировка по весу
                });

            // Цикл для вывода результатов группировки и сортировки
            foreach (var palGroup in palletByExpiration)
            {
                Console.WriteLine($"\tСрок годности: {palGroup.ExpirationDate}");
                foreach (var pallet in palGroup.Pallets)
                {
                    Console.WriteLine($"\t\t-{pallet}");
                }
            }

            Console.WriteLine("\nТри паллеты, которые содержат коробки с наибольшим сроком годности, отсортированные по возрастанию объема: ");
            var palletWithBoxes = pallets
                .Where(p => p.Boxes.Count != 0).ToList(); // Получаем паллеты с коробками и записываем их в переменную palletWithBoxes
            if (palletWithBoxes.Count != 0)
            {
                var palTopBoxExp = palletWithBoxes
                    .Select(p => new
                    {
                        Pallet = p,
                        TopBoxExp = p.Boxes.Max(t => t.ExpirationDate) // Выбираем коробку с наибольшим сроков годности
                    })
                    .OrderByDescending(palInfo => palInfo.TopBoxExp) // Сортируем в порядке убывания 
                    .Take(3) // Берем первые три записи
                    .OrderBy(palInfo => palInfo.Pallet.Volume) // Сортируем по возрастанию объема
                    .Select(palInfo => palInfo.Pallet)
                    .ToList();

                // Вывод результата запроса
                foreach (var pallet in palTopBoxExp)
                {
                    DateOnly latestBoxExp = pallet.Boxes.Count != 0 ? pallet.Boxes.Max(b => b.ExpirationDate) : DateOnly.MinValue;
                    Console.WriteLine($"\t-{pallet} (Наибольший срок годности коробки: {latestBoxExp})");
                }

            }
            Console.ReadLine();
        }
    }
}