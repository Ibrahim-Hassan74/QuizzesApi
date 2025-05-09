namespace ExamApi.DTO
{
    public class QuizModelResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Time { get; set; }
        public List<QuestionModelResponse> Question { get; set; } = new();
    }
}
