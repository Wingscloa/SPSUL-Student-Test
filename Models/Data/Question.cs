namespace SPSUL.Models.Data
{
    /// <summary>
    /// Reprezentuje jednu otázku v bance otázek.
    /// Otázky jsou samostatné a přidávají se do testů formě snapshotu.
    /// </summary>
    public class Question
    {
        /// <summary>Primární klíč otázky.</summary>
        public int QuestionId { get; set; }

        /// <summary>Nadpis / znění otázky zobrazené studentovi.</summary>
        public required string Header { get; set; }

        /// <summary>Volitelný doplnkjící popis nebo kontext otázky.</summary>
        public required string Description { get; set; }

        /// <summary>ID typu otázky (např. Uzavřená otázka, Uzavřená s obrázky).</summary>
        public int QuestionTypeId { get; set; }

        /// <summary>ID učitele, který otázku vytvořil.</summary>
        public int CreatorId { get; set; }

        /// <summary>ID předmětu (oboru), ke kterému otázka patří.</summary>
        public int FieldId { get; set; }

        /// <summary>Zda je otázka aktivní a může být přidána do testu.</summary>
        public bool IsActive { get; set; }

        public virtual StudentField Field { get; set; }
        public virtual QuestionType QuestionType { get; set; }
        public virtual Teacher Creator { get; set; }

        /// <summary>Možnosti (odpovědi) k této otázce.</summary>
        public virtual ICollection<QuestionOption> QuestionOptions { get; set; }
    }
}
