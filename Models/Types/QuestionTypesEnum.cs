namespace SPSUL.Models.Types
{
    /// <summary>
    /// Konstanty pro nazvy typu otazek tak, jak jsou ulozeny v databazi (tabulka QuestionTypes).
    /// </summary>
    public static class QuestionTypesEnum
    {
        /// <summary>Uzavrena otazka s textovymi moznostmi (A, B, C, D).</summary>
        public const string SelectText = "Vyber z moznosti";

        /// <summary>Uzavrena otazka kde moznosti obsahuji obrazky z Azure Blob Storage.</summary>
        public const string SelectImage = "Uzavrena otazka s obrazky";
    }
}
