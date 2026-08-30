using System.ComponentModel.DataAnnotations;

namespace CarWashStation.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }
}
