namespace SPSUL.Models.Data
{
    /// <summary>
    /// Reprezentuje přiřazení testu konkrétnímu studentovi (vazba M:N mezi Student a Test).
    /// Zároveň uchovává celý průběh testu – od zahájení po výsledky.
    /// </summary>
    public class StudentTest
    {
        /// <summary>ID studenta (součást složeného primárního klíče).</summary>
        public int StudentId { get; set; }

        /// <summary>ID testu (součást složeného primárního klíče).</summary>
        public int TestId { get; set; }

        /// <summary>
        /// Jednorázový přihlašovací kód vygenerovaný pro studenta.
        ///Student se jim přihlasí na stránku testu (bez uživatelského jména/hesla).
        /// </summary>
        public string LoginId { get; set; }

        /// <summary>
        /// Čas zahájení testu studentem.
        /// Pokud je roven DateTime.MinValue, student test ještě nezahájil.
        /// </summary>
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// Čas dokončení testu.
        /// Pokud je roven DateTime.MinValue, student test ještě nedokončil.
        /// </summary>
        public DateTime FinishedAt { get; set; }

        /// <summary>
        /// JSON snapshot odpovědí studenta.
        /// Formát: TestResultSnapshot serializovaný do JSON.
        /// Uloží se při odeslání testu.
        /// </summary>
        public string ResultSnapshot { get; set; }

        /// <summary>
        /// Volitelné JSON pole, které uchovává pořadí otázek po jejich případném namíchání.
        /// Uloží se hned při zahájení testu, aby bylo pořadí konzistentní po celou dobu.
        /// </summary>
        public string? ShuffleOrder { get; set; }

        public virtual Student Student { get; set; }
        public virtual Test Test { get; set; }
    }
}
