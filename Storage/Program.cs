using Storage.Database;

// Создаем родительский класс "Склад"
public abstract class Warehouse
{
    // Инициализируем переменные общего набора свойств (ID, ширина, высота, глубина, вес)
    // public Guid id { get; } = Guid.NewGuid();
    public int DbId { get; set; }
    public double width { get; set; }
    public double height { get; set; }
    public double depth { get; set; }
    public double weight { get; set; }

    // Инициализируем конструктор родительского класса 
    protected Warehouse(double Width, double Height, double Depth, double Weight)
    {
        width = Width;
        height = Height;
        depth = Depth;
        weight = Weight;
    }

    // Переопределяем метод ToString() для последующего вывода параметров на экран
    public override string ToString()
    {
        return $"ID: {DbId}, Ширина: {width}, Высота: {height}, Глубина: {depth}, Вес: {weight}";
    }
}

// Создаем наследуемый класс "Коробка"
public class Box : Warehouse
{
    // Инициализируем переменные срока годности и даты производства
    public DateOnly expirationDate { get; private set; }
    public DateOnly? productionDate { get; private set; }

    // Инициализируем конструктор наследуемого класса 
    public Box(double Width, double Height, double Depth, double Weight,
        DateOnly ExpirationDate, DateOnly? ProductionDate = null) : base(Width, Height, Depth, Weight)
    {
        productionDate = ProductionDate;
        expirationDate = ExpirationDate;
    }

    // Создаем функцию для рассчёта объема коробки
    public double CalculateVolume()
    {
        return width * height * depth;
    }

    // Переопределяем метод ToString() для последующего вывода параметров на экран
    public override string ToString()
    {
        string dateInfo = productionDate.HasValue ? $", Дата производства: {productionDate.Value:dd.MM.yyyy}, Срок годности: {expirationDate:dd.MM.yyyy}" : $", Срок годности: {expirationDate:dd.MM.yyyy}";

        return base.ToString() + $", Объем коробки: {CalculateVolume()}" + dateInfo;
    }
}

// Создаем наследуемый класс "Паллета"
public class Pallet : Warehouse
{
    // Инициализируем список объектов класса "Коробка" т.к. паллета может содержать в себе коробки
    public List<Box> boxes = new List<Box>();
    public const double palletWeight = 30.0;

    // Инициализируем конструктор наследуемого класса меняя базовое значение веса на константу 30.0
    public Pallet(double Width, double Height, double Depth) : base(Width, Height, Depth, palletWeight)
    {
    }

    // Вычисляем срок годности паллеты
    public DateOnly? ExpirationDate
    {
        get
        {
            if (!boxes.Any())
            {
                return DateOnly.MaxValue;
            }
            return boxes.Min(box => box.expirationDate);
        }
    }
    // Создаем метод для добавления коробки с проверкой по размерам (по ширине и глубине)
    public void AddBox(Box box)
    {
        if (box.width > width && box.depth > depth)
        {
            Console.WriteLine($"Предупреждение: Ширина и глубина коробки {box.DbId} ({box.width},{box.depth}) превышает ширину и глубину паллеты {DbId} ({width},{depth})\n");
        }
        else if (box.width > width)
        {
            Console.WriteLine($"Предупреждение: Ширина коробки {box.DbId} ({box.width}) превышает ширину паллеты {DbId} ({width})\n");
        }
        else if (box.depth > depth)
        {
            Console.WriteLine($"Предупреждение: Глубина коробки {box.DbId} ({box.depth}) превышает глубину паллеты {DbId} ({depth})\n");
        }
        else
        {
            boxes.Add(box);
            CalculateWeight();
        }

    }

    // Создаем функцию для рассчета веса паллеты 
    public void CalculateWeight()
    {
        weight = boxes.Sum(box => box.weight) + palletWeight;
    }

    // Создаем функцию для рассчета объема паллеты 
    public double CalculateVolume()
    {
        double palletDepth = width * height * depth;
        return boxes.Sum(box => box.CalculateVolume()) + palletDepth;
    }

    // Переопределяем метод ToString() для последующего вывода параметров на экран
    public override string ToString()
    {
        return base.ToString() + $", Объем паллеты: {CalculateVolume()}, Срок годности: {ExpirationDate}, Кол-во коробок: {boxes.Count()}";
    }
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

            // Выводим коробки стоящие на определенной паллете:
            if (pallet.boxes.Any())
            {
                Console.WriteLine("  Коробки на этой паллете: ");
                foreach (var boxOnPallet in pallet.boxes)
                {
                    Console.WriteLine($"\t- {boxOnPallet}");
                }
            }
        }

        //---------------------LINQ-----------------------
        Console.WriteLine("\nГруппировка паллет по сроку годности, сортировка по сроку годности и весу:");
        var palletByExpiration = pallets.GroupBy(p => p.ExpirationDate)
        .OrderBy(g => g.Key)
        .Select(g => new
        {
            ExpirationDate = g.Key,
            Pallets = g.OrderBy(p => p.weight).ToList()
        }
        );

        foreach (var palGroup in palletByExpiration)
        {
            Console.WriteLine($"\tСрок годности: {palGroup.ExpirationDate}");
            foreach (var pallet in palGroup.Pallets)
            {
                Console.WriteLine($"\t\t-{pallet}");
            }
        }

        Console.WriteLine("\nТри паллеты, которые содержат коробки с наибольшим сроком годности, отсортированные по возрастанию объема:");
        var palletWithBoxes = pallets.Where(p => p.boxes.Any()).ToList();

        if (palletWithBoxes.Any())
        {
            var palTopBoxExp = palletWithBoxes
                .Select(p => new
                {
                    Pallet = p,
                    TopBoxExp = p.boxes.Max(t => t.expirationDate)
                })
                .OrderByDescending(palInfo => palInfo.TopBoxExp)
                .Take(3)
                .OrderBy(palInfo => palInfo.Pallet.CalculateVolume())
                .Select(palInfo => palInfo.Pallet)
                .ToList();

            if (palletWithBoxes.Any())
            {
                foreach (var pallet in palTopBoxExp)
                {
                    DateOnly latestBoxExp = pallet.boxes.Any() ? pallet.boxes.Max(b => b.expirationDate) : DateOnly.MinValue;
                    Console.WriteLine($"\t-{pallet} (Наибольший срок годности коробки: {latestBoxExp})");
                }
            }
        }
    }
}