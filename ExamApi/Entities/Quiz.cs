using System.ComponentModel.DataAnnotations;

namespace ExamApi.Entities
{
    public class Quiz
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = "{0} can't be blank")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} can't be blank")]
        public string SubTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} can't be blank")]
        public int Time { get; set; } 

        public List<Question> Questions { get; set; } = new();
    }

}
