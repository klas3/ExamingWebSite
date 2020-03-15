using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Education.Models
{
    public class Exam
    {
        public int ExamId { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }
        public User User;

        public ICollection<Question> Questions { get; set; }
    }
}
