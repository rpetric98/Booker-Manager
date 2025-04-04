namespace WebAPP.Models
{
    public class SearchVM
    {
        public string X { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string OrderBy { get; set; }
        public string Submit { get; set; }
        public int FromPager { get; set; }
        public int ToPager { get; set; }
        public int LastPage { get; set; }
        public IEnumerable<PropertyVM> Properties { get; set; }
    }
}
