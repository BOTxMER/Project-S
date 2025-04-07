using System;

namespace MicApp.Models
{
    public class Mic
    {
        public int Id { get; set; } 
        public string Model { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; }

        public Mic(string model, string description, double price)
        {
            Model = model;
            Description = description;
            Price = price;
            CreatedAt = DateTime.Now;
        }
    }
}
