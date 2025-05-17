namespace Storage.Core.Models
{
    // Создаем наследуемый класс "Паллета"
    // Инициализируем первичный конструктор наследуемого класса меняя базовое значение веса на константу 30.0
    public class Pallet(double width, double height, double depth)
        : StorageItem(width, height, depth, PalletWeight)
    {
        // Инициализируем список объектов класса "Коробка" т.к. паллета может содержать в себе коробки
        public readonly List<Box> Boxes = [];
        public const double PalletWeight = 30.0;

        // Переопределяем значение веса паллеты
        public override double Weight => Boxes.Sum(box => box.Weight) + PalletWeight;

        // Вычисляем срок годности паллеты
        public DateOnly? ExpirationDate
        {
            get
            {
                if (Boxes.Count == 0)
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
        }

        // Переопределяем значение объема для класса Pallet
        public override double Volume
        {
            get
            {
                var palletDepth = Width * Height * Depth;
                return Boxes.Sum(box => box.Volume) + palletDepth;
            }
        }

        // Переопределяем метод ToString() для последующего вывода параметров на экран
        public override string ToString()
        {
            return $"{base.ToString()}, Объем паллеты: {Volume}, Срок годности: {ExpirationDate}, Кол-во коробок: {Boxes.Count}";
        }

    }
}

