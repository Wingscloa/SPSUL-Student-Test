namespace SPSUL.Models.Data
{
    /// <summary>
    /// Jedna možnost (odpověď) ke konkrétní otázce.
    /// Otázka může mít více možností, přičemž právě jedna (nebo více) je správná.
    /// </summary>
    public class QuestionOption
    {
        /// <summary>Primární klíč možnosti.</summary>
        public int QuestionOptionId { get; set; }

        /// <summary>ID otázky, ke které tato možnost patří.</summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// Klíč obrázku v Azure Blob Storage (bez přípony).
        /// Null pokud možnost nemá obrázek (textová možnost).
        /// </summary>
        public string? ImageKey { get; set; }

        /// <summary>Text možnosti zobrazený studentovi.</summary>
        public required string Text { get; set; }

        /// <summary>Zda je tato možnost správná odpovědí.</summary>
        public bool IsCorrect { get; set; }

        public virtual Question Question { get; set; }
    }
}
