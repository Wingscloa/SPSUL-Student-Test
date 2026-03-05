namespace SPSUL.Models
{
    /// <summary>
    /// Statické konstanty pro všechna opravdná oprávnění v aplikaci.
    ///
    /// Proč konstanty místo magic strings:
    ///   Pokud by se opravdé název změnil, stačí změnit zde – ne na 20 místech.
    ///   Kompilder také upozorní na překlep (na rozdíl od stringů).
    ///
    /// Mapování konstant na role je definováno v AuthorizationService.RolePermissionMap.
    /// Detailní přehled co která role může je v souboru PERMISSIONS.txt.
    /// </summary>
    public static class AppPermissions
    {
        // Administrátor — full control
        public const string All = "All";

        // Tvůrce — CRUD on everything except teachers
        public const string ManageTests = "ManageTests";
        public const string ManageStudents = "ManageStudents";
        public const string ManageClasses = "ManageClasses";
        public const string ManageQuestions = "ManageQuestions";

        // Testátor — CRUD tests only
        public const string CrudTests = "CrudTests";

        // Učitelátor — CRUD teachers only
        public const string CrudTeachers = "CrudTeachers";

        // Studentátor — CRUD students only
        public const string CrudStudents = "CrudStudents";

        // Hledič / Anonymous — view only
        public const string ViewOnly = "ViewOnly";
    }
}
