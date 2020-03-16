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
    public class ProjectController : Controller, ICheckLoggedUser
    {
        private IUserService UserService;
        private IProjectService ProjectService;
        private User loggedUser;
        private VacationManagerDbContext context;

        public ProjectController(IUserService UserService, IProjectService ProjectService, VacationManagerDbContext context)
        {
            this.UserService = UserService;
            this.ProjectService = ProjectService;
            this.context = context;
        }

        public void CheckLoggedUser()
        {
            if (LoggedUserInfo.LoggedUserId != 0)
            {
                loggedUser = UserService.GetLoggedUser();
                ViewData["LoggedUser"] = loggedUser;
            }
        }

        public IActionResult CreateProject()
        {
            CheckLoggedUser();
            return View();
        }

        [HttpPost]
        public IActionResult CreateProject(string name, string description)
        {
            int result = ProjectService.CreateProject(name, description);
            if(result == -1)
            {
                ViewBag.Message = "Project name already exists!";
                return View();
            }

            CheckLoggedUser();
            return this.RedirectToAction("Index", "Home");
        }

        public IActionResult EditProject(int id)
        {
            Project project = context.Projects.FirstOrDefault(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            EditProjectViewModel editProjectViewModel = new EditProjectViewModel
            {
                Name = project.Name,
                Description = project.Description
            };

            return this.View(editProjectViewModel);
        }

        [HttpPost]
        public IActionResult EditProject(EditProjectViewModel model)
        {
            int result = ProjectService.EditProject(model);
            if (result == -1)
            {
                ViewBag.Message = "Project name already exists!";
                return View();
            }

            CheckLoggedUser();
            return RedirectToAction("ViewCreatedProjects", "Project");
        }

        public IActionResult DeleteProject(int id)
        {
            ProjectService.DeleteProject(id);
            return this.RedirectToAction("ViewCreatedProjects", "Project");
        }

        public IActionResult ViewCreatedProjects()
        {
            ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
            CheckLoggedUser();
            ViewData["Context"] = context;
            return View();
        }

        public IActionResult ViewProject(int id)
        {
            ViewData["Project"] = ProjectService.ViewProject(id);
            ViewData["Context"] = context;

            CheckLoggedUser();
            return View();
        }
    }
}
