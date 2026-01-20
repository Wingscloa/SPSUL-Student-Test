using SPSUL.Models.Data;

namespace SPSUL.Models.Display.ConfigModels
{
    public class ClassesFormEditVM
    {
        public required Classes Classes { get; set; }
        public required List<StudentField> StudentFields { get; set; }
    }
}
