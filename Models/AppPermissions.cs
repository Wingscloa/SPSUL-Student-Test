namespace SPSUL.Models
{
    /// <summary>
    /// Statické konstanty pro všechna opravdná oprávnìní v aplikaci.
    ///
    /// Proè konstanty místo magic strings:
    ///   Pokud by se opravdé název zmìnil, staèí zmìnit zde – ne na 20 místech.
    ///   Kompilder také upozorní na pøeklep (na rozdíl od stringù).
    ///
    /// Mapování konstant na role je definováno v AuthorizationService.RolePermissionMap.
    /// Detailní pøehled co která role mùže je v souboru PERMISSIONS.txt.
    /// </summary>
    public static class AppPermissions
    {
        // Administrátor — full control
        public const string All = "All";

        // Tvùrce — CRUD on everything except teachers
        public const string ManageTests = "ManageTests";
        public const string ManageStudents = "ManageStudents";
        public const string ManageClasses = "ManageClasses";
        public const string ManageQuestions = "ManageQuestions";

        // Testátor — CRUD tests only
        public const string CrudTests = "CrudTests";

        // Uèitelátor — CRUD teachers only
        public const string CrudTeachers = "CrudTeachers";

        // Studentátor — CRUD students only
        public const string CrudStudents = "CrudStudents";

        // Hlediè / Anonymous — view only
        public const string ViewOnly = "ViewOnly";
    }
}
