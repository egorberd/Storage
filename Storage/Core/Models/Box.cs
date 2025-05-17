namespace Storage.Core.Models
{
    // Создаем наследуемый класс "Коробка"
    public class Box(double width, double height, double depth, double weight, DateOnly? productionDate, DateOnly expirationDate)
        : StorageItem(width, height, depth, weight)
    {
        // Инициализируем переменные срока годности и даты производства
        private DateOnly _expirationDate = expirationDate; // Поле для зранения срока годности 
        public DateOnly? ProductionDate { get; set; } = productionDate;

        public DateOnly ExpirationDate
        {
            get => ProductionDate?.AddDays(100) ?? _expirationDate;
            set => _expirationDate = value;
        }

        // Переопределяем значение объема для класса Box
        public override double Volume
        {
            get
            {
                return Width * Height * Depth;
            }
        }

        // Переопределяем метод ToString() для последующего вывода параметров на экран
        public override string ToString()
        {
            var dateInfo = ProductionDate.HasValue
                ? $", Дата производства: {ProductionDate.Value:dd.MM.yyyy}, Срок годности: {ExpirationDate:dd.MM.yyyy}"
                : $", Срок годности: {ExpirationDate:dd.MM.yyyy}";

            return $"{base.ToString()}, Объем коробки: {Volume}{dateInfo}";
        }
    }
}