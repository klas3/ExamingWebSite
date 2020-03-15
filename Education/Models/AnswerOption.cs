using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Education.Models
{
    public class AnswerOption
    {
        public int AnswerOptionId { get; set; }

        public string Option { get; set; }
        public bool IsOptionRight { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
