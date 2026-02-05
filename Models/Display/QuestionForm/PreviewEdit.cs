using SPSUL.Models.Data;

namespace SPSUL.Models.Display.QuestionForm
{
    public class PreviewEdit
    {
        public required string Header { get; set; } 
        public required string Description { get; set; }
        public required List<QuestionOption> Options { get; set; }
    }
}
