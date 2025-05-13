using System;
using System.ComponentModel.DataAnnotations;
using ExamApi.ApplicationDbContextNamespace;
using ExamApi.DTO;
using ExamApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ExamApi.RouteGroups
{
    public static class ExamMapGroup
    {
        public static RouteGroupBuilder MapExamGroup(this RouteGroupBuilder group)
        {
            group.MapGet("", async (ApplicationDbContext db) =>
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

            group.MapPost("", async (QuizDTO quizDto, ApplicationDbContext db) =>
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
                QuizDTO? quiz = context.Arguments.OfType<QuizDTO>().FirstOrDefault();

                if (quiz == null)
                    return Results.BadRequest(new { error = "quiz details has not found" });

                var validationContext = new ValidationContext(quiz);
                List<ValidationResult> validationResults = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(quiz, validationContext, validationResults, true);

                if (!isValid)
                {
                    return Results.BadRequest(validationResults.FirstOrDefault()?.ErrorMessage);
                }

                var result = await next(context);


                return result;
            });

            group.MapDelete("/{Id:guid}", async (Guid Id, ApplicationDbContext db) =>
            {
                var quiz = await db.Quizzes.FirstOrDefaultAsync(q => q.Id == Id);
                if (quiz == null)
                {
                    return Results.NotFound(new { error = "Quiz not found" });
                }
                db.Quizzes.Remove(quiz);
                await db.SaveChangesAsync();
                return Results.Ok();
            });

            group.MapPut("/{Id:guid}", async (Guid Id, QuizDTO quizDto, ApplicationDbContext db) =>
            {
                var quiz = await db.Quizzes.FirstOrDefaultAsync(q => q.Id == Id);
                if (quiz == null)
                {
                    return Results.NotFound(new { error = "Quiz not found" });
                }
                quiz.Title = quizDto.Title;
                quiz.SubTitle = quizDto.SubTitle;
                quiz.Time = int.TryParse(quizDto.Time, out var parsedTime) ? parsedTime : 0;
                await db.SaveChangesAsync();
                return Results.Ok();
            }).AddEndpointFilter(async (context, next) =>
            {
                QuizDTO? quiz = context.Arguments.OfType<QuizDTO>().FirstOrDefault();

                if (quiz == null)
                    return Results.BadRequest(new { error = "quiz details has not found" });

                if(string.IsNullOrEmpty(quiz.SubTitle) || string.IsNullOrEmpty(quiz.Title) || !int.TryParse(quiz.Time, out var res))
                {
                    return Results.BadRequest(new { error = "quiz details has not found" });
                }

                var result = await next(context);

                return result;
            });

            return group;
        }

    }
}
