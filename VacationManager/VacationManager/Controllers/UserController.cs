using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationManager.Controllers.Interfaces;
using VacationManager.Data;
using VacationManager.Data.Models;
using VacationManager.Services;
using VacationManager.Services.Interfaces;
using VacationManager.ViewModels;

namespace VacationManager.Controllers
{
    public class UserController : Controller, ICheckLoggedUser
    {
        private IUserService service;
        private User loggedUser;
        private VacationManagerDbContext context;

        public UserController(IUserService service, VacationManagerDbContext context)
        {
            this.service = service;
            this.context = context;
        }

        public void CheckLoggedUser()
        {
            if (LoggedUserInfo.LoggedUserId != 0)
            {
                loggedUser = service.GetLoggedUser();
                ViewData["LoggedUser"] = loggedUser;
            }
        }

        public IActionResult Register()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Register(string firstName, string lastName, string username,
             string password, string confirmPassword, string email, string role)
        {
            int result = service.Register(firstName, lastName, username, password, confirmPassword, email, role);
            if (result > 0)
            {

            }
            else
            {
                if (result == -1)
                {
                    ViewBag.Message = "Username or Email is already taken!";
                    return View();
                }
                else if (result == -2)
                {
                    ViewBag.Message = "Role must be 'CEO', 'Team Lead', 'Developer' or 'Unassigned'!";
                    return View();
                }
            }

            return RedirectToAction("Login", "User");
        }

        public IActionResult EditUser(int id)
        {
            User user = context.Users.FirstOrDefault(u => u.Id == id);
            if(user == null)
            {
                return NotFound();
            }

            EditUserViewModel editUserViewModel = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Username = user.Username,
                Password = user.Password,
                ConfirmPassword = user.ConfirmPassword,
                Role = user.Role
            };

            return this.View(editUserViewModel);
        }

        [HttpPost]
        public IActionResult EditUser(EditUserViewModel model)
        {
            int result = service.EditUser(model);
            if (result > 0)
            {

            }
            else
            {
                if (result == -1)
                {
                    ViewBag.Message = "Username or Email is already taken!";
                    return View();
                }
                else if (result == -2)
                {
                    ViewBag.Message = "Role must be 'CEO', 'Team Lead', 'Developer' or 'Unassigned'!";
                    return View();
                }
            }

            return RedirectToAction("ViewCreatedUsers", "User");
        }

        public IActionResult DeleteUser(int id)
        {
            service.DeleteUser(id);
            return this.RedirectToAction("ViewCreatedUsers", "User");
        }

        public IActionResult ViewUser(int id)
        {
            ViewData["User"] = service.ViewUser(id);

            CheckLoggedUser();
            return View();
        }

        public IActionResult AddToTeam(int id)
        {
            CheckLoggedUser();

            service.AddUserToTeam(loggedUser.Id, id);

            return this.RedirectToAction("ViewCreatedUsers", "User");
        }

        public IActionResult ViewCreatedUsers()
        {
            ViewData["CreatedUsers"] = service.GetAllUsers();
            CheckLoggedUser();
            ViewData["Context"] = context;
            return View();
        }

        public IActionResult ViewCreatedRoles()
        {
            ViewData["CreatedUsers"] = service.GetAllUsers();
            CheckLoggedUser();
            ViewData["Context"] = context;
            return View();
        }

        public IActionResult Login()
        {
            LoggedUserInfo.LoggedUserId = 0;
            return this.View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                if (service.Login(username, password) == -1)
                {
                    return RedirectToAction("Login", "User");
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
