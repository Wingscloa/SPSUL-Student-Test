using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// Oznaèí akci jako pøístupnou bez pøihlášení uèitele.
///
/// Proè existuje:
///   Studenti se pøihlašují na test pomocí LoginId (ne jako uèitelé).
///   Akce jako Test/Take nebo Test/Example proto nepotøebují session uèitele.
///   LoginRequiredAttribute kontroluje pøítomnost tohoto atributu a kontrolu pøeskoèí.
///
/// Použití:
///   [AllowAnonymousTest]
///   public IActionResult Take() { ... }
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AllowAnonymousTestAttribute : Attribute, IFilterMetadata
{
}
