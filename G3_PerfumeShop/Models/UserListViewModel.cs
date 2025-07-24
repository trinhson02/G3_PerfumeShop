namespace G3_PerfumeShop.Models
{
    using System.Collections.Generic;

    public class UserListViewModel
    {
        public List<User> Users { get; set; }
        public int? StatusId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }

}
