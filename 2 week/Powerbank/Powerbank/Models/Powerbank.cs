using System;

namespace PbApp.Models
{
    public class Powerbank
    {
        public int Id { get; set; } // Первичный ключ
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; }

        public Powerbank(string name, string description, double price)
        {
            Name = name;
            Description = description;
            Price = price;
            CreatedAt = DateTime.Now;
        }
    }
}
