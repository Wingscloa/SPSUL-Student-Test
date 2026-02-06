using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// Marks an action as accessible without teacher login (for student test-taking).
/// Works by skipping the LoginRequired check.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AllowAnonymousTestAttribute : Attribute, IFilterMetadata
{
}
