namespace SPSUL.Models.Display
{
     /// <summary>
     /// Generální helper pro stránkování dat v tabulkách.
     ///
     /// Jak funguje:
     ///   Controller vezme všechny záznamy z DB, předají se PageIndex a PageSize,
     ///   a PaginatedList vypočítá celkový počet stránek a uloží právě tu správnou "stránku" záznamů.
     ///
     /// Použití:
     ///   var paged = new PaginatedList&lt;Teacher&gt;(rows, totalCount, pageNumber, pageSize);
     ///   // V Razor view: @Model.PageIndex / @Model.TotalPages
     /// </summary>
    public class PaginatedList<T> : List<T>
    {
        public List<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
            TotalItems = count;
        }
    }
}
