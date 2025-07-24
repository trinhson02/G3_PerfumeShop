namespace G3_PerfumeShop.Models
{
    using System.Collections.Generic;

    public class CustomerListViewModel
    {
        public List<User> Customers { get; set; }
        public string SearchTerm { get; set; }
        public int? StatusId { get; set; }
        public string SortOrder { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public List<Status> Statuses { get; set; } // Để hiển thị dropdown chọn trạng thái
        public List<string> DynamicColumns { get; set; }
    }

}
