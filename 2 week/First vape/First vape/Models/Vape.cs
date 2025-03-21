using System;

namespace VapeApp.Models
{
    public class Vape
    {
        public int Id { get; set; } // Первичный ключ
        public string Name { get; set; }
        public string Laste { get; set; }
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; }

        public Vape(string name, string laste, double price)
        {
            Name = name;
            Laste = laste;
            Price = price;
            CreatedAt = DateTime.Now;
        }
    }
}
