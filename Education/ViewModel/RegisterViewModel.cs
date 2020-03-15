using System.ComponentModel.DataAnnotations;

namespace Education.ViewModel
{
    public class RegisterViewModel
    {
        [Display(Name = "Прізвище")]
        [Required(ErrorMessage = "Введіть ваше прізвище")]
        public string LastName { get; set; }

        [Display(Name = "Ім'я")]
        [Required(ErrorMessage = "Введіть ваше ім'я")]
        public string FirstName { get; set; }

        [Display(Name = "Логін")]
        [Required(ErrorMessage = "Введіть ваш логін")]
        public string Login { get; set; }

        [Display(Name = "Ел. пошта")]
        [Required(ErrorMessage = "Введіть вашу електронну пошту")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Введіть ваш пароль")]
        public string Password { get; set; }

        [Display(Name = "Замам'ятати мене")]
        public bool RememberMe { get; set; }

        [Display(Name = "Хто ви?")]
        [Required(ErrorMessage = "Оберіть хто ви")]
        public string Role { get; set; }
    }
}
