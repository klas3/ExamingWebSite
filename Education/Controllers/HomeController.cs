using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Education.Models;
using Education.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Education.ViewModel;

namespace Education.Controllers
{
    public class HomeController : Controller
    {
        private IRepository _repository;
        private const string domenName = "http://localhost:53038";

        public HomeController(IRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Exams()
        {
            ExamViewModel viewModel = new ExamViewModel();

            if(User.IsInRole("Student"))
            {
                viewModel.Exams = _repository.GetAllAvailableExams(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            else if(User.IsInRole("Teacher"))
            {
                viewModel.Exams = _repository.GetAllTeacherExams(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Exams(ExamViewModel viewModel)
        {
            if(viewModel.NewExamName != null)
            {
                await _repository.AddExam(new Exam
                {
                    Name = viewModel.NewExamName,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                });
            }

            return RedirectToAction("Exams");
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public IActionResult ExamQuestions(int id, bool isediting)
        {
            if(CheckTeacher(id))
            {
                ViewBag.ExamName = _repository.GetExam(id)?.Name;
                ViewBag.ExamId = id;
                ViewBag.IsEditing = isediting;

                return View(new QuestionsViewModel { Questions = _repository.GetQuestions(id) });
            }
            
            return RedirectToAction("Exams");
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public IActionResult ChangeExamEditing(int examid, bool editing)
        {
            if (CheckTeacher(examid))
            {
                return RedirectToAction("ExamQuestions", new { id = examid, isediting = editing });
            }

            return RedirectToAction("Exams");
        }

        [HttpGet("Home/ExamTeacherMarks/{examid}")]
        [Authorize(Roles = "Teacher")]
        public IActionResult ExamTeacherMarks(int examid)
        {
            if (CheckTeacher(examid))
            {
                ViewBag.ExamName = _repository.GetExam(examid)?.Name;
                ViewBag.ExamId = examid;
                ViewBag.Marks = _repository.GetMarks(examid);

                return View();
            }

            return RedirectToAction("Exams");
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateExam(int examid, ExamViewModel model)
        {
            if(CheckTeacher(examid) && model.NewExamName != null)
            {
                Exam exam = _repository.GetExam(examid);
                exam.Name = model.NewExamName;
                await _repository.UpdateExam(exam);
            }

            return RedirectToAction("Exams");
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteExam(int examid)
        {
            if (CheckTeacher(examid))
            {
                await _repository.DeleteExam(_repository.GetExam(examid));
            }

            return RedirectToAction("Exams");
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> ExamQuestions(int examid, bool editing, QuestionsViewModel model)
        {
            if(model.AskedQuestion != null && CheckTeacher(examid))
            {
                await _repository.AddQuestion(new Question
                {
                    AskedQuestion = model.AskedQuestion,
                    ExamId = examid
                });

                return RedirectToAction("ExamQuestions", new { id = examid, isediting = editing });
            }

            return RedirectToAction("Exams");
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateQuestion(int examid, int questionid, bool editing, QuestionsViewModel model)
        {
            if (model.AskedQuestion != null && CheckTeacher(examid))
            {
                await _repository.UpdateQuestion(new Question
                {
                    QuestionId = questionid,
                    ExamId = examid,
                    AskedQuestion = model.AskedQuestion
                });

                return RedirectToAction("ExamQuestions", new { id = examid, isediting = editing });
            }

            return RedirectToAction("Exams");
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteQuestion(int examid, int questionid, bool editing)
        {
            if (CheckTeacher(examid))
            {
                await _repository.DeleteQuestion(_repository.GetQuestion(questionid));
                return RedirectToAction("ExamQuestions", new { id = examid, isediting = editing });
            }

            return RedirectToAction("Exams");
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> AddOption(int questionid, int examid, bool editing, QuestionsViewModel model)
        {
            if(model.NewOptionName != null && CheckTeacher(examid))
            {
                await _repository.AddOption(new AnswerOption
                {
                    QuestionId = questionid,
                    Option = model.NewOptionName,
                    IsOptionRight = model.IsOptionRight
                });

                return RedirectToAction("ExamQuestions", new { id = examid, isediting = editing });
            }

            return RedirectToAction("Exams");
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateOption(int examid, int optionid, int questionid, bool editing, QuestionsViewModel model)
        {
            if (model.NewOptionName != null && CheckTeacher(examid))
            {
                await _repository.UpdateOption(new AnswerOption
                {
                    AnswerOptionId = optionid,
                    QuestionId = questionid,
                    Option = model.NewOptionName,
                    IsOptionRight = model.IsOptionRight
                });

                return RedirectToAction("ExamQuestions", new { id = examid, isediting = editing });
            }

            return RedirectToAction("Exams");
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteOption(int optionid, int examid, bool editing)
        {
            if(CheckTeacher(examid))
            {
                await _repository.RemoveOption(_repository.GetOption(optionid));
                return RedirectToAction("ExamQuestions", new { id = examid, isediting = editing });
            }

            return RedirectToAction("Exams");
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ExamPassing(int id)
        {
            if(_repository.GetMark(id, User.FindFirstValue(ClaimTypes.NameIdentifier)) == null)
            {
                ViewBag.ExamName = _repository.GetExam(id)?.Name;
                ViewBag.ExamId = id;
                List<Question> questions = _repository.GetRandomQuestion(id);

                if(questions.Count != 5)
                {
                    return RedirectToAction("Exams");
                }

                ViewBag.Questions = questions;
                ViewBag.Options = _repository.ConvertOptions(questions);

                await _repository.AddMark(new Mark
                {
                    MarkValue = 0,
                    ExamId = id,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                });
            }
            else
            {
                return RedirectToAction("Exams");
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ExamPassing(int id, ExamPassingViewModel model)
        {
            Mark mark = _repository.GetMark(id, User.FindFirstValue(ClaimTypes.NameIdentifier));

            if(mark != null && (Request.Headers["Referer"].ToString()).Contains($"{domenName}/Home/ExamPassing/"))
            {
                mark.MarkValue = _repository.CalculateMark(id, model.Options);
                await _repository.UpdateMark(mark);
            }

            return RedirectToAction("Exams");
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public IActionResult StudentMarks()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = _repository.GetUser(userId);

            ViewBag.FirstName = user.FirstName;
            ViewBag.LastName = user.LastName;

            return View(_repository.GetStudentMarks(userId));
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteMark(int id, int markid)
        {
            if(CheckTeacher(id))
            {
                await _repository.DeleteMark(_repository.GetMark(markid));
                return RedirectToAction("ExamTeacherMarks", new { examid = id });
            }

            return RedirectToAction("Exams");
        }

        private bool CheckTeacher(int examId)
        {
            return _repository.GetTeacherId(examId) == User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
