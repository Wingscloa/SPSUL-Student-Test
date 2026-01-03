namespace SPSUL.Models.Data
{
    public class TestName
    {
        public int TestNameId { get; set; }
        public required string Name { get; set; }
        public ICollection<Test> Tests { get; set; }
    }
}
