namespace SPSUL.Models.Display.StudentModels
{
    public class StudentFilter
    {
        public string? SearchFilter { get; set; }
        public bool? ActiveFilter { get; set; }
        public List<int>? ClassFilterIds { get; set; } 
        public List<int>? TestFilterIds { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 13;
    }
}
