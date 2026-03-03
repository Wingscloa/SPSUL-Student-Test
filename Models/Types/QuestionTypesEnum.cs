namespace SPSUL.Models.Types
{
    /// <summary>
    /// Konstanty pro názvy typů otázek tak, jak jsou uloženy v databázi (tabulka QuestionTypes).
    ///
    /// Proč:
    ///   Na několika místech v kódu se rozlišuje chování podle typu otázky.
    ///   Místo psání "Uzavřená otázka" jako string na více místech se používají tyto konstanty.
    /// </summary>
    public static class QuestionTypesEnum
    {
        /// <summary>Uzavřená otázka s textovými možnostmi (A, B, C, D).</summary>
        public const string SelectText = "Uzavřená otázka";

        /// <summary>Uzavřená otázka kde možnosti obsahují obrázky z Azure Blob Storage.</summary>
        public const string SelectImage = "Uzavřená otázka s obrázky";
    }
}
