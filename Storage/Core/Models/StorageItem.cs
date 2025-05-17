// Создаем родительский класс "Склад"
public abstract class StorageItem
{
    // Инициализируем переменные общего набора свойств (ID, ширина, высота, глубина, вес)
    // public Guid id { get; } = Guid.NewGuid();
    public int DbId { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Depth { get; set; }
    public double Weight { get; set; }
    public abstract double Volume { get; }

    // Инициализируем конструктор родительского класса 
    protected StorageItem(double width, double height, double depth, double weight)
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
