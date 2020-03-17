using Education.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Education.ViewModel
{
    public class ExamViewModel
    {
        [Display(Name = "Новий екзамен")]
        public string NewExamName { get; set; }
        public List<Exam> Exams { get; set; }
    }
}
