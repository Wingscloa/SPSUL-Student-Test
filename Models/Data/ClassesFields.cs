namespace SPSUL.Models.Data
{
    public class ClassesFields
    {
        public int ClassesId { get; set; }
        public int StudentFieldId { get; set; }
        public virtual Classes Classes { get; set; }
        public virtual StudentField StudentField { get; set; }

    }
}
