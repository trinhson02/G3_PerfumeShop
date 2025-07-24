namespace G3_PerfumeShop.Models
{
    public class FeedbackListViewModel
    {
        public List<Feedback> Feedbacks { get; set; }
        public string SearchTerm { get; set; }
        public int? Rating { get; set; }
        public bool? Status { get; set; }
        public string SortOrder { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public List<string> DynamicColumns { get; set; }
    }

}
