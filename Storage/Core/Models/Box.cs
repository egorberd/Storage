// Создаем наследуемый класс "Коробка"
public class Box : StorageItem
{
    // Инициализируем переменные срока годности и даты производства

    public DateOnly? ProductionDate { get; private set; }

    private DateOnly _expirationDate;

    public DateOnly ExpirationDate
    {
        get
        {
            if (ProductionDate.HasValue)
            {
                return ProductionDate.Value.AddDays(100);
            }
            return _expirationDate;
        }
        set
        {
            _expirationDate = value;
        }
    }

    // Инициализируем конструктор наследуемого класса 
    public Box(double width, double height, double depth, double weight,
        DateOnly expirationDate, DateOnly? productionDate = null) : base(width, height, depth, weight)
    {
        ProductionDate = productionDate;
        ExpirationDate = expirationDate;
    }

    // Переопределяем значение объема для класса Box
    public override double Volume
         => Width * Height * Depth;


    // Переопределяем метод ToString() для последующего вывода параметров на экран
    public override string ToString()
    {
        var dateInfo = ProductionDate.HasValue ? $", Дата производства: {ProductionDate.Value:dd.MM.yyyy}, Срок годности: {ExpirationDate:dd.MM.yyyy}" : $", Срок годности: {ExpirationDate:dd.MM.yyyy}";

        return $"{base.ToString()}, Объем коробки: {Volume}{dateInfo}";
    }
}
