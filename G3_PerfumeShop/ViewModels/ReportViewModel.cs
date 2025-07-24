using System;
using System.Collections.Generic;
using G3_PerfumeShop.Models;


namespace G3_PerfumeShop.Models
{
    public class ReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? UserId { get; set; }
        public bool? Status { get; set; }
        public List<User> Users { get; set; }
        public List<RevenueTrend> RevenueTrend { get; set; }
        public List<OrderTrend> OrderTrend { get; set; }
    }

   

    public class RevenueTrend
    {
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class OrderTrend
    {
        public DateTime Date { get; set; }
        public int SuccessOrders { get; set; }
        public int TotalOrders { get; set; }
    }
}