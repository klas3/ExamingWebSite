using Education.Models;
using Education.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Education.Repositories
{
    public class Repository : IRepository
    {
        private Context _context;
        private const int requiredQuestionsCount = 5;

        public Repository(Context context)
        {
            _context = context;
        }

        public bool IsLoginUnique(string login)
        {
            return _context.DBUsers.SingleOrDefault(user => user.UserName == login) == null;
        }

        public bool IsEmailUnique(string email)
        {
            return _context.DBUsers.SingleOrDefault(user => user.Email == email) == null;
        }

        public User GetUser(string userId)
        {
            return _context.DBUsers.Where(user => user.Id == userId).SingleOrDefault();
        }

        public List<Exam> GetAllAvailableExams(string userId)
        {
            return _context.Exams.Where(exam => _context.Marks.Where(mark => mark.ExamId == exam.ExamId).SingleOrDefault() == null).ToList();
        }

        public List<Exam> GetAllTeacherExams(string teacherId)
        {
            return _context.Exams.Where(exam => exam.UserId == teacherId).ToList();
        }

        public async Task AddExam(Exam exam)
        {
            await _context.Exams.AddAsync(exam);
            await _context.SaveChangesAsync();
        }

        public string GetTeacherId(int examId)
        {
            return _context.Exams.Where(exam => exam.ExamId == examId).SingleOrDefault()?.UserId;
        }

        public async Task UpdateExam(Exam exam)
        {
            _context.Exams.Update(exam);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteExam(Exam exam)
        {
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
        }

        public Exam GetExam(int examId)
        {
            return _context.Exams.Where(exam => exam.ExamId == examId).SingleOrDefault();
        }

        public async Task AddQuestion(Question question)
        {
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();
        }

        public Question GetQuestion(int questionId)
        {
            return _context.Questions.Where(question => question.QuestionId == questionId).SingleOrDefault();
        }


        public Question GetQuestionByOptionId(int optionId)
        {
            return _context.Questions.Where(question => question.AnswerOptions.Where(option => option.AnswerOptionId == optionId).SingleOrDefault() != null).Include(question => question.AnswerOptions).SingleOrDefault();
        }
        public async Task UpdateQuestion(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuestion(Question question)
        {
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }

        public async Task AddOption(AnswerOption option)
        {
            await _context.AnswerOptions.AddAsync(option);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOption(AnswerOption option)
        {
            _context.AnswerOptions.Update(option);
            await _context.SaveChangesAsync();
        }

        public AnswerOption GetOption(int optionId)
        {
            return _context.AnswerOptions.Where(option => option.AnswerOptionId == optionId).SingleOrDefault();
        }

        public async Task RemoveOption(AnswerOption option)
        {
            _context.AnswerOptions.Remove(option);
            await _context.SaveChangesAsync();
        }

        public List<Question> GetQuestions(int examId)
        {
            return _context.Questions.Where(question => question.ExamId == examId).Include(question => question.AnswerOptions).ToList();
        }

        public async Task AddMark(Mark mark)
        {
            await _context.Marks.AddAsync(mark);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMark(Mark mark)
        {
            _context.Marks.Update(mark);
            await _context.SaveChangesAsync();
        }

        public Mark GetMark(int examId, string userId)
        {
            return _context.Marks.Where(mark => mark.ExamId == examId && mark.UserId == userId).SingleOrDefault();
        }

        public Mark GetMark(int markId)
        {
            return _context.Marks.Where(mark => mark.MarkId == markId).SingleOrDefault();
        }

        public List<Mark> GetMarks(int examId)
        {
            return _context.Marks.Where(mark => mark.ExamId == examId).Include(mark => mark.User).ToList();
        }

        public List<Mark> GetStudentMarks(string studentId)
        {
            return _context.Marks.Where(mark => mark.UserId == studentId).Include(mark => mark.Exam).ToList();
        }

        public int CalculateMark(int examId, List<List<AnswerOption>> options)
        {
            List<AnswerOption> correcctOptions = new List<AnswerOption>();

            foreach(List<AnswerOption> optionsList in options)
            {
                foreach(AnswerOption option in optionsList)
                {
                    correcctOptions.Add(GetOption(option.AnswerOptionId));
                }
            }

            int mark = 0;

            for(int i = 0; i < options.Count; i++)
            {
                bool markFlag = true;

                for (int j = 0; j < options[i].Count; j++)
                {
                    if(correcctOptions.Where(option => option.AnswerOptionId == options[i][j].AnswerOptionId).SingleOrDefault().IsOptionRight)
                    {
                        if(!options[i][j].IsOptionRight) 
                        {
                            markFlag = false;
                            break;
                        }
                    }
                    else
                    {
                        if (options[i][j].IsOptionRight)
                        {
                            markFlag = false;
                            break;
                        }
                    }
                }

                if(markFlag)
                {
                    mark++;
                }
            }

            return mark;
        }

        public async Task DeleteMark(Mark mark)
        {
            _context.Marks.Remove(mark);
            await _context.SaveChangesAsync();
        }

        public List<Question> GetRandomQuestion(int examId)
        {
            List<Question> randomQuestions = new List<Question>();
            List<Question> questions = GetQuestions(examId);
            Random random = new Random();

            if(questions.Count >= requiredQuestionsCount)
            {
                while(randomQuestions.Count != requiredQuestionsCount)
                {
                    int index = random.Next(questions.Count);

                    if(!randomQuestions.Contains(questions[index]))
                    {
                        randomQuestions.Add(questions[index]);
                    }
                }
            }
            
            return randomQuestions;
        }

        public List<List<AnswerOption>> ConvertOptions(List<Question> questions)
        {
            List<List<AnswerOption>> convertedOptions = new List<List<AnswerOption>>();

            for(int i = 0; i < questions.Count; i++)
            {
                List<AnswerOption> options = questions[i].AnswerOptions.ToList();
                convertedOptions.Add(new List<AnswerOption>());

                for (int j = 0; j < options.Count; j++)
                {
                    convertedOptions[i].Add(options[j]);
                }
            }

            return convertedOptions;
        }
    }
}
