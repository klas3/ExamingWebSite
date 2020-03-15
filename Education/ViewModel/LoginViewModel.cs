using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Education.ViewModel
{
    public class LoginViewModel
    {
        [Display(Name = "Логін")]
        [Required(ErrorMessage = "Введіть ваш Логін")]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Введіть ваш пароль")]
        public string Password { get; set; }

        [Display(Name = "Замам'ятати мене")]
        public bool RememberMe { get; set; }
    }
}
