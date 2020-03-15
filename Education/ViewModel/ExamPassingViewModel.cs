using Education.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Education.ViewModel
{
    public class ExamPassingViewModel
    {
        public List<Question> Questions { get; set; }
        public List<List<AnswerOption>> Options { get; set; }
    }
}
