using System.ComponentModel.DataAnnotations;

namespace SPSUL.Models.Display.ClassesModels
{
    public class ClassesCreate : IValidatableObject
    {
        public required string Name { get; set; }
        public int StartFrom { get; set; }
        public int EndTo { get; set; }
        public required List<int> StudentFieldIds { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartFrom > EndTo)
                yield return new ValidationResult(
                    "Rok zahájení nesmí být větší než rok ukončení.",
                    [nameof(StartFrom), nameof(EndTo)]);
        }
    }
}
