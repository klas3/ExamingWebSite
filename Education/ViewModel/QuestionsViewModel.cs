using Education.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Education.ViewModel
{
    public class QuestionsViewModel
    {
        [Display(Name = "Запитання")]
        public string AskedQuestion { get; set; }

        [Display(Name = "Назва варіанту")]
        public string NewOptionName { get; set; }

        [Display(Name = "Чи вірний цей варіант?")]
        public bool IsOptionRight { get; set; }
        public List<Question> Questions { get; set; }
    }
}
