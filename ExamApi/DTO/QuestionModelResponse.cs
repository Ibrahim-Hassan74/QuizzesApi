namespace ExamApi.DTO
{
    public class QuestionModelResponse
    {
        public string Question { get; set; }
        public List<string> Options { get; set; } = new();
        public string CorrectOption { get; set; }
    }
}
