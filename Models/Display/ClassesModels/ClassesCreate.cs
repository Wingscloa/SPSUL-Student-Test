namespace SPSUL.Models.Display.ClassesModels
{
    public class ClassesCreate
    {
        public required string Name { get; set; }
        public int StartFrom { get; set; }
        public int EndTo { get; set; }
        public required List<int> StudentFieldIds { get; set; }
    }
}
