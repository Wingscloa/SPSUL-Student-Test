using SPSUL.Models.Data;

namespace SPSUL.Models
{
    /// <summary>
    /// Služba pro zápis auditního logu (kdo, co, kdy udělal).
    ///
    /// Proč existuje:
    ///   Administrátor potřebuje vědět, kdo smazal učitele nebo změnil roli.
    ///   Každá CREATE / UPDATE / DELETE operace zavolá LogAsync() a záznam se uloží do tabulky AuditLogs.
    ///
    /// Informace, které se logují:
    ///   - Jméno a ID přihlášeného učitele (ze session)
    ///   - Název akce (např. "Vytvořen", "Upraven", "Smazán")
    ///   - Entita (např. "Učitel", "Student", "Třída")
    ///   - ID entity a doplnkový detail
    ///   - Čas akce (UTC)
    /// </summary>
    public class AuditService
    {
        private readonly SpsulContext _ctx;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(SpsulContext ctx, IHttpContextAccessor httpContextAccessor)
        {
            _ctx = ctx;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string action, string entity, string? entityId = null, string? detail = null)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var teacherId = session?.GetInt32("TeacherId");
            var teacherName = session?.GetString("Name") ?? "Systém";

            _ctx.AuditLogs.Add(new AuditLog
            {
                TeacherId = teacherId,
                TeacherName = teacherName,
                Action = action,
                Entity = entity,
                EntityId = entityId,
                Detail = detail,
                CreatedAt = DateTime.UtcNow
            });

            await _ctx.SaveChangesAsync();
        }
    }
}
