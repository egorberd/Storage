using System;
using Microsoft.Data.Sqlite;
using System.Data.SQLite;
using Storage.Database;

// Создаем родительский класс "Склад"
public abstract class Warehouse
{
    // Инициализируем переменные общего набора свойств (ID, ширина, высота, глубина, вес)
    public Guid id { get; } = Guid.NewGuid();
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
        return $"App_ID: {id}, DB_ID: {DbId} Ширина: {width}, Высота: {height}, Глубина: {depth}, Вес: {weight}";
    }
}

// Создаем наследуемый класс "Коробка"
public class Box : Warehouse
{
    // Инициализируем переменные срока годности и даты производства
    public DateOnly expirationDate { get; private set; }
    public DateOnly? productionDate { get; private set; }

    // нициализируем конструктор наследуемого класса 
    public Box(double Width, double Height, double Depth, double Weight,
        DateOnly ExpirationDate, DateOnly? ProductionDate = null) : base(Width, Height, Depth, Weight)
    {

        productionDate = ProductionDate;

        //Прописываем условие: "Если указана дата производства, то срок годности вычисляется из даты производства плюс 100 дней"
        expirationDate = productionDate.HasValue ? productionDate.Value.AddDays(100) : expirationDate;
    }

    // Создаем функцию для рассчёта объема коробки
    public double CalculateVolume()
    {
        return width * height * depth;
    }

    // Переопределяем метод ToString() для последующего вывода параметров на экран
    public override string ToString()
    {
        string dateInfo = productionDate.HasValue ? $", Дата производства: {productionDate.Value:dd.MM.yyyy}, Срок годности: {expirationDate:dd.MM.yyyy}" : $"Срок годности: {expirationDate:dd.MM.yyyy}";

        return base.ToString() + dateInfo;
    }
}

// оздаем наследуемый класс "Паллета"
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
    public DateOnly ExpirationDate
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
        foreach (var currentBox in boxes)
        {
            if (currentBox.width > width)
            {
                throw new ArgumentException(
                    $"Ширина коробки({currentBox.width}) превышает ширину паллеты({width})");
            }
            else if (currentBox.depth > depth)
            {
                throw new ArgumentException(
                    $"Глубина коробки({currentBox.depth}) превышает глубину паллеты({depth})");
            }

        }

        boxes.Add(box);
        CalculateWeight();
    }

    // Создаем метод для удаления коробки
    /*   public void RemoveBox(Box box)
       {
           if (boxes.Remove(box)) CalculateWeight();
       }*/

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
        return base.ToString() + " Кол-во коробок: " + boxes.Count();
    }
}

public class Program
{
    public static void Main()
    {
        Module_DB.DataBaseInteraction();
    }
}