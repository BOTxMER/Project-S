using System;

namespace CarApp.Models
{
    public class Car
    {
        public int Id { get; set; } 
        public string Model { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; }

        public Car(string model, string description, double price)
        {
            Model = model;
            Description = description;
            Price = price;
            CreatedAt = DateTime.Now;
        }
    }
}
