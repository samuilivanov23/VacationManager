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
    public class TeamController : Controller, ICheckLoggedUser
    {
        private IUserService UserService;
        private ITeamService TeamService;
        private IProjectService ProjectService;
        private User loggedUser;
        private VacationManagerDbContext context;

        public TeamController(IUserService UserService, ITeamService TeamService, IProjectService ProjectService, VacationManagerDbContext context)
        {
            this.UserService = UserService;
            this.TeamService = TeamService;
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

        public IActionResult CreateTeam()
        {
            ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
            CheckLoggedUser();
            return View();
        }

        [HttpPost]
        public IActionResult CreateTeam(string name, string projectName)
        {
            CheckLoggedUser();
            int result = TeamService.CreateTeam(name, projectName, loggedUser.Id);

            if (result > 0)
            {

            }
            else if (result <= 0)
            {
                if (result == 0)
                {
                    ViewBag.Message = $"Such team name already exists!";
                    return View();
                }
                else if (result == -1)
                {
                    ViewBag.Message = $"This project has not been created yet. Please choose an existing project!";
                    return View();
                }
            }

            return this.RedirectToAction("Index", "Home");
        }

        public IActionResult EditTeam(int id)
        {
            Team team = context.Teams.FirstOrDefault(t => t.Id == id);
            Project project = null;

            if (team == null)
            {
                return NotFound();
            }

            EditTeamViewModel editTeamViewModel = new EditTeamViewModel
            {
                Id = team.Id,
                Name = team.Name
            };

            if(team.Project != null)
            {
                project = context.Projects.FirstOrDefault(p => p.Id == team.ProjectId);
                editTeamViewModel.ProjectName = project.Name;
            }
            else
            {
                editTeamViewModel.ProjectName = null;
            }

            ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
            return this.View(editTeamViewModel);
        }

        [HttpPost]
        public IActionResult EditTeam(EditTeamViewModel model)
        {
            int result = TeamService.EditTeam(model);

            if (result > 0)
            {

            }
            else if (result <= 0)
            {
                if (result == 0)
                {
                    ViewBag.Message = $"Such team name already exists!";
                    return View();
                }
                else if (result == -1)
                {
                    ViewBag.Message = $"This project has not been created yet. Please choose an existing project!";
                    return View();
                }
            }

            ViewData["AllCreatedTeams"] = TeamService.GetAllTeams();
            CheckLoggedUser();
            return RedirectToAction("ViewAllCreatedTeams", "Team");
        }

        public IActionResult CreateTeamByCeo()
        {
            ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
            ViewData["CreatedUsers"] = UserService.GetAllUsers();
            CheckLoggedUser();
            return View();
        }

        [HttpPost]
        public IActionResult CreateTeamByCeo(string name, string projectName, string teamLeadFirstName, string teamLeadLastName)
        {
            CheckLoggedUser();
            int result = TeamService.CreateTeamByCeo(name, projectName, teamLeadFirstName, teamLeadLastName);

            if (result > 0)
            {

            }
            else if (result <= 0)
            {
                if (result == 0)
                {
                    ViewBag.Message = $"Such team name already exists!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -1)
                {
                    ViewBag.Message = $"Project does not exist!" + Environment.NewLine +
                                      $"First name does not exist!" + Environment.NewLine +
                                      $"Last name does not exist!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -2)
                {
                    ViewBag.Message = $"Project does not exist!" + Environment.NewLine +
                                      $"First name does not exist!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -3)
                {
                    ViewBag.Message = $"Project does not exist!" + Environment.NewLine +
                                      $"Last name does not exist!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -4)
                {
                    ViewBag.Message = $"Project does not exist!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -5)
                {
                    ViewBag.Message = $"First name does not exist!" + Environment.NewLine +
                                      $"Last name does not exist!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -6)
                {
                    ViewBag.Message = $"First name does not exist!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -7)
                {
                    ViewBag.Message = $"Last name does not exist!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -8)
                {
                    ViewBag.Message = $"This user already has a team!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -9)
                {
                    ViewBag.Message = $"There is no user with this combination of first and last names!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
            }

            return this.RedirectToAction("Index", "Home");
        }

        public IActionResult EditTeamByCeo(int id)
        {
            Team team = context.Teams.FirstOrDefault(t => t.Id == id);
            Project project = null;

            if (team == null)
            {
                return NotFound();
            }

            EditTeamByCeoViewModel editTeamByCeoViewModel = new EditTeamByCeoViewModel
            {
                Id = team.Id,
                Name = team.Name,
                TeamLeadFirstName = null,
                TeamLeadLastName = null
            };

            if (team.Project != null)
            {
                project = context.Projects.FirstOrDefault(p => p.Id == team.ProjectId);
                editTeamByCeoViewModel.ProjectName = project.Name;
            }
            else
            {
                editTeamByCeoViewModel.ProjectName = null;
            }

            ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
            ViewData["CreatedUsers"] = UserService.GetAllUsers();
            CheckLoggedUser();
            return this.View(editTeamByCeoViewModel);
        }

        [HttpPost]
        public IActionResult EditTeamByCeo(EditTeamByCeoViewModel model)
        {
            int result = TeamService.EditTeamByCeo(model);

            if (result > 0)
            {

            }
            else if (result <= 0)
            {
                if (result == 0)
                {
                    ViewBag.Message = $"Such team name already exists!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -1)
                {
                    ViewBag.Message = $"Project does not exist!" + Environment.NewLine +
                                      $"First name does not exist!" + Environment.NewLine +
                                      $"Last name does not exist!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -2)
                {
                    ViewBag.Message = $"Project does not exist!" + Environment.NewLine +
                                      $"First name does not exist!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -3)
                {
                    ViewBag.Message = $"Project does not exist!" + Environment.NewLine +
                                      $"Last name does not exist!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -4)
                {
                    ViewBag.Message = $"Project does not exist!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -5)
                {
                    ViewBag.Message = $"First name does not exist!" + Environment.NewLine +
                                      $"Last name does not exist!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -6)
                {
                    ViewBag.Message = $"First name does not exist!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -7)
                {
                    ViewBag.Message = $"Last name does not exist!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -8)
                {
                    ViewBag.Message = $"This user already has a team!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
                else if (result == -9)
                {
                    ViewBag.Message = $"There is no user with this combination of first and last names!";
                    ViewData["CreatedProjects"] = ProjectService.GetAllProjects();
                    ViewData["CreatedUsers"] = UserService.GetAllUsers();
                    CheckLoggedUser();
                    return View();
                }
            }

            ViewData["AllCreatedTeams"] = TeamService.GetAllTeams();
            CheckLoggedUser();
            return RedirectToAction("ViewAllCreatedTeams", "Team");
        }

        public IActionResult ViewAllCreatedTeams()
        {
            ViewData["AllCreatedTeams"] = TeamService.GetAllTeams();
            CheckLoggedUser();
            ViewData["Context"] = context;
            return View();
        }

        public IActionResult ViewUserCreatedTeams(int id)
        {
            ViewData["UserCreatedVacation"] = TeamService.GetUserCreatedTeams(id);
            CheckLoggedUser();
            ViewData["Context"] = context;
            return View();
        }

        public IActionResult ViewTeam(int id)
        {
            ViewData["Team"] = TeamService.ViewTeam(id);
            CheckLoggedUser();
            ViewData["Context"] = context;
            return View();
        }
    }
}