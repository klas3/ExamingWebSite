using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Education.Models
{
    public class Question
    {
        public int QuestionId { get; set; }

        public string AskedQuestion { get; set; }

        public ICollection<AnswerOption> AnswerOptions { get; set; }

        public int ExamId { get; set; }
        public Exam Exam { get; set; }
    }
}
