using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Education.Models
{
    public class Mark
    {
        public int MarkId { get; set; }

        public int MarkValue { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        public int ExamId { get; set; }
        public Exam Exam { get; set; }
    }
}
