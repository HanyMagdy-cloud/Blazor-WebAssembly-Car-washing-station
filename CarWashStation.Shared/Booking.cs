using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CarWashStation.Models
{
    public class Booking
    {
        // Primary key 
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string? CustomerName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Car Plate Number")]
        public string? CarPlateNumber { get; set; }

        [Required]
        [Display(Name = "Service")]
        public int ServiceId { get; set; }
        
        public Service? Service { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Booking Date")]
        [FutureOrToday(ErrorMessage = "Bokningsdatum kan inte vara i det förflutna.")]
        [JsonConverter(typeof(BookingDateJsonConverter))]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Time Slot")]
        public TimeSpan TimeSlot { get; set; }

        [Display(Name = "Företagsnamn")]
        public string? CompanyName { get; set; }

        [Display(Name = "Anteckningar")]
        public string? Notes { get; set; }
    }

    public class FutureOrTodayAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime.Date < BookingTime.Now.Date)
                {
                    return new ValidationResult(ErrorMessage ?? "Datumet kan inte vara i det förflutna.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
