using SPSUL.Models.Data;

namespace SPSUL.Models.Display
{
    public class HomeViewModel
    {
        public string TeacherName { get; set; } = "Učiteli";
        public int ActiveTests { get; set; }
        public int TotalStudents { get; set; }
        public int TotalQuestions { get; set; }
        public double AvgSuccess { get; set; }
        public int PendingCount { get; set; }
        public List<StudentTest> RecentFinished { get; set; } = [];
        public List<Test> UpcomingTests { get; set; } = [];
        public string MonthlyLabels { get; set; } = "[]";
        public string MonthlyValues { get; set; } = "[]";
    }
}
