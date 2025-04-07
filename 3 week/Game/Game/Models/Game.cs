using System;

namespace GameApp.Models
{
    public class Game
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; }

        public Game(string name, string description, double price)
        {
            Name = name;
            Description = description;
            Price = price;
            CreatedAt = DateTime.Now;
        }
    }
}
