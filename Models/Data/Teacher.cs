namespace SPSUL.Models.Data
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string NickName { get; set; }
        public required string PasswordHash { get; set; }
        public ICollection<TeacherTitle> Titles {get; set;}
        public ICollection<TeacherRole> TeacherRoles { get; set; }
    }
}
