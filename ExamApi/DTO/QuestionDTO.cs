using System.ComponentModel.DataAnnotations;

namespace ExamApi.DTO
{
    public class QuestionDTO
    {
        [Required(ErrorMessage = "{0} can't be blank")]
        [StringLength(1000, ErrorMessage = "{0} can't be more than {1} characters")]
        public string Question { get; set; }
        [Required(ErrorMessage = "{0} can't be blank")]
        public List<string> Options { get; set; } = new();
        [Required(ErrorMessage = "{0} can't be blank")]
        [StringLength(100, ErrorMessage = "{0} can't be more than {1} characters")]
        public string CorrectOption { get; set; } = string.Empty;
    }
}
