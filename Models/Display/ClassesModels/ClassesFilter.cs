namespace SPSUL.Models.Display.ClassesModels
{
    public class ClassesFilter
    {
        public string? SearchFilter { get; set; }
        public int? StartFromFilter { get; set; }
        public int? EndToFilter { get; set; }
        public int? FieldFilterId { get; set; }
        public bool? ActiveFilter { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 13;
    }
}
