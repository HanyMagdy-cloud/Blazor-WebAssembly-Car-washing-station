using System;
using System.Collections.Generic;

namespace CarWashStation.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalBookingsCount { get; set; }
        public int NewThisWeekCount { get; set; }
        public int TodayCount { get; set; }
        public int RemainingTodayCount { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int TotalCustomersCount { get; set; }
        public int CorporateCustomersCount { get; set; }
        public IEnumerable<Booking> RecentBookings { get; set; } = new List<Booking>();
        
        // Search filters to pass back to the view
        public string? CurrentFilter { get; set; }
        public string? CurrentDateFilter { get; set; }
    }
}
