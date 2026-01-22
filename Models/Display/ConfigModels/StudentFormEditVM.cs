using SPSUL.Models.Data;

namespace SPSUL.Models.Display.ConfigModels
{
    public class StudentFormEditVM
    {
        public required Student Student { get; set; }
        public required List<Classes> Classes { get; set; }
    }
}
