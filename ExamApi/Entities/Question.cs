using System.ComponentModel.DataAnnotations;

namespace ExamApi.Entities
{
    public class Question
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "{0} can't be blank")]
        public string Text { get; set; } = string.Empty;

        // Navigation to options
        public List<Option> Options { get; set; } = new();

        // Foreign key
        public Guid QuizId { get; set; }

        // Navigation property
        public Quiz Quiz { get; set; } = new();
    }

}
