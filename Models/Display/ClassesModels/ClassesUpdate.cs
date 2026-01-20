namespace SPSUL.Models.Display.ClassesModels
{
    public class ClassesUpdate
    {
        public int ClassesId { get; set; }
        public required string Name { get; set; }
        public int StartFrom { get; set; }
        public int EndTo { get; set; }
        public required List<int> StudentFieldIds { get; set; }
        public bool IsActive { get; set; }
    }
}
