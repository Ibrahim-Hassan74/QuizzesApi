using System.ComponentModel.DataAnnotations;

namespace ExamApi.Entities
{
    public class Option
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} can't be blank")]
        public string Text { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        // Foreign key
        public Guid QuestionId { get; set; }

        // Navigation property
        public Question Question { get; set; } = new();
    }

}
