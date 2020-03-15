using Education.Models;
using Education.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Education.Repositories
{
    public interface IRepository
    {
        bool IsLoginUnique(string login);
        bool IsEmailUnique(string email);
        List<Exam> GetAllAvailableExams(string userId);
        List<Exam> GetAllTeacherExams(string teacherId);
        Task AddExam(Exam exam);
        List<Question> GetQuestions(int examId);
        Exam GetExam(int examId);
        Task AddQuestion(Question question);
        Task DeleteExam(Exam exam);
        Task DeleteQuestion(Question question);
        Task AddOption(AnswerOption option);
        Task RemoveOption(AnswerOption option);
        AnswerOption GetOption(int optionId);
        string GetTeacherId(int examId);
        Question GetQuestion(int questionId);
        Task UpdateOption(AnswerOption option);
        Task UpdateQuestion(Question question);
        Task UpdateExam(Exam exam);
        List<Question> GetRandomQuestion(int examId);
        List<List<AnswerOption>> ConvertOptions(List<Question> questions);
        Task AddMark(Mark mark);
        Task UpdateMark(Mark mark);
        Mark GetMark(int examId, string userId);
        Task DeleteMark(Mark mark);
        int CalculateMark(int examId, List<List<AnswerOption>> options);
        Question GetQuestionByOptionId(int optionId);
        List<Mark> GetMarks(int examId);
        Mark GetMark(int markId);
        List<Mark> GetStudentMarks(string studentId);
        User GetUser(string userId);
    }
}
