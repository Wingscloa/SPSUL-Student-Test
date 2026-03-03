namespace SPSUL.Models.Data
{
    /// <summary>
    /// Reprezentuje třídu (skupinu studentů).
    /// Třída má ročník začátku a konce studia a je spřažena s obory (StudentField).
    /// </summary>
    public class Classes
    {
        public int ClassesId { get; set; }

        /// <summary>Název třídy, např. "4A", "3IT".</summary>
        public required string Name { get; set; }

        /// <summary>Rok zahájení studia (např. 2022).</summary>
        public int StartFrom { get; set; }

        /// <summary>Rok ukončení studia (např. 2026).</summary>
        public int EndTo { get; set; }

        /// <summary>Zda je třída aktivní (aktuálně studuje).</summary>
        public bool IsActive { get; set; }

        /// <summary>Studenti v této třídě (M:N vazba).</summary>
        public ICollection<ClassesStudent> ClassesStudents { get; set; }

        /// <summary>Obory přiřazené této třídě (M:N vazba).</summary>
        public ICollection<ClassesFields> ClassesFields { get; set; }
    }
}
