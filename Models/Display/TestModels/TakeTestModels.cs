namespace SPSUL.Models.Display.TestModels
{
    public class AssignTestDto
    {
        public int TestId { get; set; }
        public List<int> StudentIds { get; set; } = new();
    }

    public class ReassignDto
    {
        public int StudentId { get; set; }
        public int TestId { get; set; }
    }

    public class SaveProgressDto
    {
        public string LoginId { get; set; } = string.Empty;
        public List<AnswerSnapshot> Answers { get; set; } = new();
        public int CurrentQuestionIndex { get; set; }
    }

    public class AnswerSnapshot
    {
        public int QuestionId { get; set; }
        public List<string> SelectedOptions { get; set; } = new();
    }

    public class TestResultSnapshot
    {
        public List<AnswerSnapshot> Answers { get; set; } = new();
        public int CurrentQuestionIndex { get; set; }
    }

    public class QuestionSnapshotItem
    {
        public int QuestionId { get; set; }
        public string Header { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public List<OptionSnapshotItem> Options { get; set; } = new();
    }

    public class OptionSnapshotItem
    {
        public string Text { get; set; } = string.Empty;
        public string? ImageKey { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class TakeTestViewModel
    {
        public string LoginId { get; set; } = string.Empty;
        public string TestName { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public int? TimeLimitMinutes { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public List<QuestionSnapshotItem> Questions { get; set; } = new();
        public List<AnswerSnapshot> ExistingAnswers { get; set; } = new();
    }

    public class SelectedCodesDto
    {
        public int TestId { get; set; }
        public List<string> LoginIds { get; set; } = new();
    }
}
