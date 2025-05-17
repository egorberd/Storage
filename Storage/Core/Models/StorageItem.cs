namespace Storage.Core.Models
{// Создаем родительский класс "Склад"
    public abstract class StorageItem(double width, double height, double depth, double weight)
    {
        // Инициализируем переменные общего набора свойств (ID, ширина, высота, глубина, вес)
        // public Guid id { get; } = Guid.NewGuid();
        public int DbId { get; set; }
        public double Width { get; } = width;
        protected double Height { get; } = height;
        public double Depth { get; } = depth;
        public virtual double Weight { get; } = weight;
        public abstract double Volume { get; }


        // Переопределяем метод ToString() для последующего вывода параметров на экран
        public override string ToString()
        {
            return $"ID: {DbId}, Ширина: {Width}, Высота: {Height}, Глубина: {Depth}, Вес: {Weight}";
        }
    }
}