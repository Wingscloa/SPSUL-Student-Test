using Microsoft.AspNetCore.Mvc;
using SPSUL.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPSUL.Controllers
{
    public class DetailController : Controller
    {
        // GET: Detail/Index
        public IActionResult Index(
            string? Q1,
            bool? Active1,
            string? successRange,
            bool? completed,
            DateTime? dateFrom,
            DateTime? dateTo,
            string? sortBy,
            int Page1 = 1,
            int PageSize1 = 10)
        {
            // TODO: Implementovat naètení dat z databáze
            // Prozatím vrátíme testovací data
            
            var testData = GenerateSampleData();
            
            // Filtrování
            var filtered = testData.AsQueryable();
            
            if (!string.IsNullOrEmpty(Q1))
            {
                filtered = filtered.Where(x => 
                    x.Nazev.Contains(Q1, StringComparison.OrdinalIgnoreCase) || 
                    x.Jmeno.Contains(Q1, StringComparison.OrdinalIgnoreCase));
            }
            
            if (completed.HasValue)
            {
                filtered = filtered.Where(x => x.Absolvoval == completed.Value);
            }
            
            if (dateFrom.HasValue)
            {
                filtered = filtered.Where(x => x.ZacalV >= dateFrom.Value);
            }
            
            if (dateTo.HasValue)
            {
                filtered = filtered.Where(x => x.DokoncilV <= dateTo.Value);
            }
            
            // Øazení
            filtered = sortBy switch
            {
                "date-asc" => filtered.OrderBy(x => x.DokoncilV),
                "success-desc" => filtered.OrderByDescending(x => x.UspechPct),
                "success-asc" => filtered.OrderBy(x => x.UspechPct),
                "name-asc" => filtered.OrderBy(x => x.Nazev),
                "name-desc" => filtered.OrderByDescending(x => x.Nazev),
                _ => filtered.OrderByDescending(x => x.DokoncilV) // default: date-desc
            };
            
            var total = filtered.Count();
            var items = filtered
                .Skip((Page1 - 1) * PageSize1)
                .Take(PageSize1)
                .ToList();
            
            var model = new DetailIndexViewModel
            {
                Q1 = Q1,
                Active1 = Active1,
                Page1 = Page1,
                PageSize1 = PageSize1,
                Assigned = new PagedResult<AssignedTestVm>
                {
                    Items = items,
                    Page = Page1,
                    PageSize = PageSize1,
                    Total = total
                }
            };
            
            return View(model);
        }
        
        // GET: Detail/History?id=123
        public IActionResult History(int id)
        {
            // TODO: Implementovat naètení historie odpovìdí z databáze
            ViewBag.AssignmentId = id;
            return View();
        }
        
        // GET: Detail/View?id=123
        public IActionResult View(int id)
        {
            // TODO: Implementovat naètení detailu testu z databáze
            ViewBag.AssignmentId = id;
            return View();
        }
        
        // Pomocná metoda pro generování testovacích dat
        private List<AssignedTestVm> GenerateSampleData()
        {
            var random = new Random();
            var data = new List<AssignedTestVm>();
            
            var testNames = new[] 
            { 
                "Základy programování", 
                "Databáze - SQL", 
                "Web development",
                "Algoritmy a datové struktury",
                "OOP v C#"
            };
            
            var studentNames = new[]
            {
                "Jan Novák",
                "Petra Svobodová", 
                "Martin Dvoøák",
                "Kateøina Procházková",
                "Tomáš Nìmec"
            };
            
            var loginIds = new[] { "jnovak", "psvobodova", "mdvorak", "kprochazka", "tnemec" };
            
            for (int i = 1; i <= 50; i++)
            {
                var started = DateTime.Now.AddDays(-random.Next(1, 30));
                var duration = TimeSpan.FromMinutes(random.Next(15, 60));
                var success = random.Next(0, 101);
                
                data.Add(new AssignedTestVm
                {
                    Id = i,
                    Nazev = testNames[random.Next(testNames.Length)],
                    LoginId = loginIds[random.Next(loginIds.Length)],
                    Jmeno = studentNames[random.Next(studentNames.Length)],
                    ZacalV = started,
                    DokoncilV = started.Add(duration),
                    UspechPct = success,
                    Absolvoval = success >= 50,
                    Aktivni = random.Next(0, 10) > 2
                });
            }
            
            return data;
        }
    }
}
