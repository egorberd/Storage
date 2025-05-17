// Создаем наследуемый класс "Паллета"
public class Pallet : StorageItem
{
    // Инициализируем список объектов класса "Коробка" т.к. паллета может содержать в себе коробки
    public readonly List<Box> boxes = new List<Box>();
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
            if (!boxes.Any())
            {
                return DateOnly.MaxValue;
            }
            return boxes.Min(box => box.ExpirationDate);
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
            boxes.Add(box);
        }
        CalculateWeight();
    }

    // Переопределяем значение объема для класса Pallet
    public override double Volume
    {
        get
        {
            var palletDepth = Width * Height * Depth;
            return boxes.Sum(box => box.Volume) + palletDepth;
        }
    }

    public void CalculateWeight()
    {
        Weight = boxes.Sum(box => box.Weight) + PalletWeight;
    }

    // Переопределяем метод ToString() для последующего вывода параметров на экран
    public override string ToString()
        => base.ToString() + $", Объем паллеты: {Volume}, Срок годности: {ExpirationDate}, Кол-во коробок: {boxes.Count()}";

}

