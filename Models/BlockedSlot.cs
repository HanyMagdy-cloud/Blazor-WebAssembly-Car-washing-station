using System;
using System.ComponentModel.DataAnnotations;

namespace CarWashStation.Models
{
    public class BlockedSlot
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Datum")]
        public DateTime Date { get; set; }

        [Display(Name = "Tid (valfritt)")]
        public TimeSpan? TimeSlot { get; set; }

        [Display(Name = "Anledning")]
        public string? Reason { get; set; }

        [Display(Name = "Hela dagen")]
        public bool IsFullDay => !TimeSlot.HasValue;
    }
}
