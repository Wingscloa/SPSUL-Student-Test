namespace SPSUL.Models.Data
{
    /// <summary>
    /// Reprezentuje test vytvořený učitelem.
    /// Test obsahuje snapshot otázek (JSON) v momentě vytvoření, aby změna otázky později
    /// neovlivnila již probíhající nebo dokončené testy.
    /// </summary>
    public class Test
    {
        /// <summary>Primární klíč testu.</summary>
        public int TestId { get; set; }

        /// <summary>Název testu zobrazovaný učiteli i studentovi.</summary>
        public required string Name { get; set; }

        /// <summary>ID učitele, který test vytvořil.</summary>
        public int CreatorId { get; set; }

        /// <summary>ID předmětu (oboru), ke kterému test patří.</summary>
        public int StudentFieldId { get; set; }

        /// <summary>
        /// JSON snapshot otázek v momentě vytvoření testu.
        /// Uloží otázky tak, jak vypadaly při tvorbě – pozdější změny otázek test neovlivní.
        /// Formát: List&lt;QuestionSnapshotItem&gt; serializovaný do JSON.
        /// </summary>
        public required string QuestionSnapshot { get; set; }

        /// <summary>Volitelný časový limit testu v minutách. Null = bez limitu.</summary>
        public int? TimeLimitMinutes { get; set; }

        /// <summary>Pokud true, otázky se studentovi zobrazí v náhodném pořadí.</summary>
        public bool ShuffleQuestions { get; set; }

        /// <summary>Zda je test aktivní a dostupný pro přiřazení studentům.</summary>
        public bool IsActive { get; set; }

        /// <summary>Předmět (obor) ke kterému test patří.</summary>
        public virtual StudentField StudentField { get; set; }

        /// <summary>Učitel, který test vytvořil.</summary>
        public virtual Teacher Creator { get; set; }

        /// <summary>Seznam přiřazení tohoto testu studentům.</summary>
        public virtual ICollection<StudentTest> StudentTests { get; set; }
    }
}