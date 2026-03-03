namespace SPSUL.Models.Data
{
    /// <summary>
    /// Reprezentuje studenta v systému.
    /// Student nemá uživatelské jméno ani heslo – přihlašuje se pouze jednorázovým LoginId k testu.
    /// </summary>
    public class Student
    {
        public int StudentId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        /// <summary>Zda je student aktivní. Neaktivní student nemůže být přiřazen k testu.</summary>
        public bool IsActive { get; set; }

        /// <summary>Všechna přiřazení testů tomuto studentovi.</summary>
        public virtual ICollection<StudentTest> StudentTests { get; set; }

        /// <summary>Třídy, do kterých student patří (M:N vazba).</summary>
        public virtual ICollection<ClassesStudent> ClassesStudents { get; set; }
    }
}
