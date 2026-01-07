using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace SPSUL.Controllers
{
    public class QuestionController : Controller
    {
        public class QuestionRow
        {
            public int Id { get; set; }
            public string Nazev { get; set; } = string.Empty;
            public string Tvurce { get; set; } = string.Empty;
            public bool Aktivni { get; set; }
            public int PocetPrirazeni { get; set; }
            public int PrumerUspech { get; set; }
        }

        private static readonly List<QuestionRow> _rows = Enumerable.Range(1, 30).Select(i => new QuestionRow
        {
            Id = i,
            Nazev = $"Testový balíček {i}",
            Tvurce = i % 2 == 0 ? "Julius" : "Eva",
            Aktivni = i % 5 != 0,
            PocetPrirazeni = 5 + (i % 20),
            PrumerUspech = (i * 3) % 100
        }).ToList();

        public IActionResult Index(string? q, bool? active, string? author, int page = 1, int pageSize = 40)
        {
            var data = _rows.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                var t = q.ToLower();
                data = data.Where(x => x.Nazev.ToLower().Contains(t));
            }
            if (!string.IsNullOrWhiteSpace(author))
            {
                var a = author.ToLower();
                data = data.Where(x => x.Tvurce.ToLower().Contains(a));
            }
            if (active.HasValue)
            {
                data = data.Where(x => x.Aktivni == active.Value);
            }
            var total = data.Count();
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 5, 50);
            var items = data.OrderByDescending(x => x.PocetPrirazeni).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = total;
            ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
            ViewBag.Query = q;
            ViewBag.Active = active;
            ViewBag.Author = author;
            return View(items);
        }
    }
}
