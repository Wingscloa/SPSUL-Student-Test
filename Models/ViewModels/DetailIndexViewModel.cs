using System;
using System.Collections.Generic;

namespace SPSUL.Models.ViewModels
{
    public class AssignedTestVm
    {
        public int Id { get; set; }
        public string Nazev { get; set; } = string.Empty;
        public string LoginId { get; set; } = string.Empty;
        public string Jmeno { get; set; } = string.Empty;
        public DateTime ZacalV { get; set; }
        public DateTime DokoncilV { get; set; }
        public int UspechPct { get; set; }
        public bool Absolvoval { get; set; }
        public bool Aktivni { get; set; }
    }

    public class TestVm
    {
        public int Id { get; set; }
        public string Nazev { get; set; } = string.Empty;
        public int PocetOtazek { get; set; }
        public DateTime Vytvoren { get; set; }
        public bool Aktivni { get; set; }
    }

    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)Total / Math.Max(1, PageSize));
    }

    public class DetailIndexViewModel
    {
        // Filters table 1
        public string? Q1 { get; set; }
        public bool? Active1 { get; set; }
        public int Page1 { get; set; } = 1;
        public int PageSize1 { get; set; } = 10;
        public PagedResult<AssignedTestVm> Assigned { get; set; } = new();

        // Filters table 2
        public string? Q2 { get; set; }
        public bool? Active2 { get; set; }
        public int Page2 { get; set; } = 1;
        public int PageSize2 { get; set; } = 10;
        public PagedResult<TestVm> Tests { get; set; } = new();
    }
}
