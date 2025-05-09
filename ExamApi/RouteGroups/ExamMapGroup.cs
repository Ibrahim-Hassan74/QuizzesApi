using System;
using System.ComponentModel.DataAnnotations;
using ExamApi.ApplicationDbContextNamespace;
using ExamApi.DTO;
using ExamApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExamApi.RouteGroups
{
    public static class ExamMapGroup
    {
        public static RouteGroupBuilder MapExamGroup(this RouteGroupBuilder group)
        {
            group.MapGet("/quizzes", async (ApplicationDbContext db) =>
            {
                var quizzes = await db.Quizzes
                    .Include(q => q.Questions)
                        .ThenInclude(q => q.Options)
                    .Select(quiz => new QuizModelResponse
                    {
                        Id = quiz.Id,
                        Title = quiz.Title,
                        SubTitle = quiz.SubTitle,
                        Time = quiz.Time.ToString(),
                        Question = quiz.Questions.Select(q => new QuestionModelResponse
                        {
                            Question = q.Text,
                            Options = q.Options.Select(o => o.Text).ToList(),
                            CorrectOption = q.Options.FirstOrDefault(o => o.IsCorrect)!.Text
                        }).ToList()
                    })
                    .ToListAsync();

                return Results.Ok(quizzes);
            });

            group.MapPost("/quizzes", async (QuizDTO quizDto, ApplicationDbContext db) =>
            {
                var quiz = new Quiz
                {
                    Id = Guid.NewGuid(),
                    Title = quizDto.Title,
                    SubTitle = quizDto.SubTitle,
                    Time = int.TryParse(quizDto.Time, out var parsedTime) ? parsedTime : 0,
                    Questions = quizDto.Question.Select(q => new Question
                    {
                        Id = Guid.NewGuid(),
                        Text = q.Question,
                        Options = q.Options.Select(opt => new Option
                        {
                            Text = opt,
                            IsCorrect = opt == q.CorrectOption
                        }).ToList()
                    }).ToList()
                };

                db.Quizzes.Add(quiz);
                await db.SaveChangesAsync();

                return Results.Ok();
            }).AddEndpointFilter(async (context, next) =>
            {
                QuizDTO? product = context.Arguments.OfType<QuizDTO>().FirstOrDefault();

                if (product == null)
                    return Results.BadRequest(new { error = "Product details has not found" });

                var validationContext = new ValidationContext(product);
                List<ValidationResult> validationResults = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(product, validationContext, validationResults, true);

                if (!isValid)
                {
                    return Results.BadRequest(validationResults.FirstOrDefault()?.ErrorMessage);
                }

                var result = await next(context);


                return result;
            });


            return group;
        }
    }
}
