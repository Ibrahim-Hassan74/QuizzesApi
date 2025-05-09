using System.ComponentModel.DataAnnotations;

namespace ExamApi.DTO
{
    public class QuizDTO
    {
        [Required(ErrorMessage = "{0} can't be blank")]
        [StringLength(100, ErrorMessage = "{0} can't be more than {1} characters")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} can't be blank")]
        [StringLength(1000, ErrorMessage = "{0} can't be more than {1} characters")]
        public string SubTitle { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} can't be blank")]
        public string Time { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} can't be blank")]
        public List<QuestionDTO> Question { get; set; } = new();
    }
}
