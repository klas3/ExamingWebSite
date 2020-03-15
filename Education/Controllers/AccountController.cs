using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Education.Models;
using Education.Repositories;
using Education.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Education.Controllers
{
    public class AccountController : Controller
    {
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IRepository _repository;

        public AccountController
        (
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IRepository repository
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _repository = repository;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Exams", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                if ((await _signInManager.PasswordSignInAsync(viewModel.Login, viewModel.Password, viewModel.RememberMe, false)).Succeeded)
                {
                    return RedirectToAction("Exams", "Home");
                }
            }

            ModelState.AddModelError("", "Не вдалося увійти");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                if(_repository.IsEmailUnique(viewModel.Email))
                {
                    if(_repository.IsLoginUnique(viewModel.Login))
                    {
                        User user = new User
                        {
                            FirstName = viewModel.FirstName,
                            LastName = viewModel.LastName,
                            Email = viewModel.Email,
                            UserName = viewModel.Login
                        };

                        var createUserResult = await _userManager.CreateAsync(user, viewModel.Password);

                        if (createUserResult.Succeeded)
                        {
                            if (!await _roleManager.RoleExistsAsync("Student"))
                            {
                                await _roleManager.CreateAsync(new IdentityRole("Student"));
                                await _roleManager.CreateAsync(new IdentityRole("Teacher"));
                            }

                            if(viewModel.Role == "Викладач")
                            {
                                await _userManager.AddToRoleAsync(user, "Teacher");
                            }
                            else
                            {
                                await _userManager.AddToRoleAsync(user, "Student");
                            }

                            if (!string.IsNullOrWhiteSpace(user.Email))
                            {
                                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email));
                            }

                            var signInUserResult = await _signInManager.PasswordSignInAsync(viewModel.Login, viewModel.Password, viewModel.RememberMe, false);

                            if (signInUserResult.Succeeded)
                            {
                                return RedirectToAction("Exams", "Home");
                            }
                        }
                        else
                        {
                            foreach (var error in createUserResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Цей логін вже використовується");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Ця пошта вже зареєстрована");
                }
            }

            return View();
        }
    }
}