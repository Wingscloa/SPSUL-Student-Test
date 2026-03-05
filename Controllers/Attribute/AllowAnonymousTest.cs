using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// Označí akci jako přístupnou bez přihlášení učitele.
///
/// Proč existuje:
///   Studenti se přihlašují na test pomocí LoginId (ne jako učitelé).
///   Akce jako Test/Take nebo Test/Example proto nepotřebují session učitele.
///   LoginRequiredAttribute kontroluje přítomnost tohoto atributu a kontrolu přeskočí.
///
/// Použití:
///   [AllowAnonymousTest]
///   public IActionResult Take() { ... }
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AllowAnonymousTestAttribute : Attribute, IFilterMetadata
{
}
