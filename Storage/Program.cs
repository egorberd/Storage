using Storage.Database;

// Создаем родительский класс "Склад"
public abstract class Warehouse
{
    // Инициализируем переменные общего набора свойств (ID, ширина, высота, глубина, вес)
    // public Guid id { get; } = Guid.NewGuid();
    public int DbId { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Depth { get; set; }
    public double Weight { get; set; }

    // Инициализируем конструктор родительского класса 
    protected Warehouse(double width, double height, double depth, double weight)
    {
        Width = width;
        Height = height;
        Depth = depth;
        Weight = weight;
    }

    // Переопределяем метод ToString() для последующего вывода параметров на экран
    public override string ToString()
        => $"ID: {DbId}, Ширина: {Width}, Высота: {Height}, Глубина: {Depth}, Вес: {Weight}";

}

// Создаем наследуемый класс "Коробка"
public class Box : Warehouse
{
    // Инициализируем переменные срока годности и даты производства
    public DateOnly ExpirationDate { get; private set; }
    public DateOnly? ProductionDate { get; private set; }

    // Инициализируем конструктор наследуемого класса 
    public Box(double width, double height, double depth, double weight,
        DateOnly expirationDate, DateOnly? productionDate = null) : base(width, height, depth, weight)
    {
        ProductionDate = productionDate;
        ExpirationDate = expirationDate;
    }

    // Создаем функцию для рассчёта объема коробки
    public double CalculateVolume()
    {
        return Width * Height * Depth;
    }

    // Переопределяем метод ToString() для последующего вывода параметров на экран
    public override string ToString()
    {
        var dateInfo = ProductionDate.HasValue ? $", Дата производства: {ProductionDate.Value:dd.MM.yyyy}, Срок годности: {ExpirationDate:dd.MM.yyyy}" : $", Срок годности: {ExpirationDate:dd.MM.yyyy}";

        return $"{base.ToString()}, Объем коробки: {CalculateVolume()}{dateInfo}";
    }
}

// Создаем наследуемый класс "Паллета"
public class Pallet : Warehouse
{
    // Инициализируем список объектов класса "Коробка" т.к. паллета может содержать в себе коробки
    public List<Box> Boxes = new List<Box>();
    public const double PalletWeight = 30.0;

    // Инициализируем конструктор наследуемого класса меняя базовое значение веса на константу 30.0
    public Pallet(double width, double height, double depth) : base(width, height, depth, PalletWeight)
    {
    }

    // Вычисляем срок годности паллеты
    public DateOnly? ExpirationDate
    {
        get
        {
            if (!Boxes.Any())
            {
                return DateOnly.MaxValue;
            }
            return Boxes.Min(box => box.ExpirationDate);
        }
    }
    // Создаем метод для добавления коробки с проверкой по размерам (по ширине и глубине)
    public void AddBox(Box box)
    {
        if (box.Width > Width && box.Depth > Depth)
        {
            Console.WriteLine($"Предупреждение: Ширина и глубина коробки {box.DbId} ({box.Width},{box.Depth}) превышает ширину и глубину паллеты {DbId} ({Width},{Depth})\n");
        }
        else if (box.Width > Width)
        {
            Console.WriteLine($"Предупреждение: Ширина коробки {box.DbId} ({box.Width}) превышает ширину паллеты {DbId} ({Width})\n");
        }
        else if (box.Depth > Depth)
        {
            Console.WriteLine($"Предупреждение: Глубина коробки {box.DbId} ({box.Depth}) превышает глубину паллеты {DbId} ({Depth})\n");
        }
        else
        {
            Boxes.Add(box);
        }
        CalculateWeight();
    }

    // Создаем функцию для рассчета веса паллеты 
    public void CalculateWeight()
    {
        Weight = Boxes.Sum(box => box.Weight) + PalletWeight;
    }

    // Создаем функцию для рассчета объема паллеты 
    public double CalculateVolume()
    {
        var palletDepth = Width * Height * Depth;
        return Boxes.Sum(box => box.CalculateVolume()) + palletDepth;
    }

    // Переопределяем метод ToString() для последующего вывода параметров на экран
    public override string ToString()
        => base.ToString() + $", Объем паллеты: {CalculateVolume()}, Срок годности: {ExpirationDate}, Кол-во коробок: {Boxes.Count()}";

}

public class Program
{
    public static void Main()
    {
        Module_DB.OpeningDataBase();

        // Инициализируем два списка объектов класса Box и Pallet
        List<Box> boxes = Module_DB.LoadBoxesFromDatabase(); // Вызываем метод для выгрузки данных коробок из БД
        List<Pallet> pallets = Module_DB.LoadPalletsFromDatabase(); // Вызываем метод для выгрузки данных паллет из БД

        // Добавляем коробки на паллеты
        if (pallets.Any() && boxes.Any())
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
        var palletByExpiration = pallets.GroupBy(p => p.ExpirationDate) // Группировка по сроку годности
        .OrderBy(g => g.Key) // Сортировка по сроку годности
        .Select(g => new // Вложенный запрос для сортировки по весу внутри группы
        {
            ExpirationDate = g.Key,
            Pallets = g.OrderBy(p => p.Weight).ToList() // Сортировка по весу
        }
        );

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
        var palletWithBoxes = pallets.Where(p => p.Boxes.Any()).ToList(); // Получаем паллеты с коробками и записываем их в переменную palletWithBoxes
        if (palletWithBoxes.Any())
        {
            var palTopBoxExp = palletWithBoxes
                .Select(p => new
                {
                    Pallet = p,
                    TopBoxExp = p.Boxes.Max(t => t.ExpirationDate) // Выбираем коробку с наибольшим сроков годности
                })
                .OrderByDescending(palInfo => palInfo.TopBoxExp) // Сортируем в порядке убывания 
                .Take(3) // Берем первые три записи
                .OrderBy(palInfo => palInfo.Pallet.CalculateVolume()) // Сортируем по возрастанию объема
                .Select(palInfo => palInfo.Pallet)
                .ToList();

            // Вывод результата запроса
            if (palletWithBoxes.Any())
            {
                foreach (var pallet in palTopBoxExp)
                {
                    DateOnly latestBoxExp = pallet.Boxes.Any() ? pallet.Boxes.Max(b => b.ExpirationDate) : DateOnly.MinValue;
                    Console.WriteLine($"\t-{pallet} (Наибольший срок годности коробки: {latestBoxExp})");
                }
            }
        }
    }
}