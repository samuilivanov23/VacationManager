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
    public class VacationsController : Controller, ICheckLoggedUser
    {
        private IUserService UserService;
        private IVacationService VacationService;
        private User loggedUser;
        private VacationManagerDbContext context;

        public VacationsController(IUserService UserService, IVacationService VacationService, VacationManagerDbContext context)
        {
            this.UserService = UserService;
            this.VacationService = VacationService;
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

        public IActionResult CreateVacation()
        {
            CheckLoggedUser();
            return View();
        }

        [HttpPost]
        public IActionResult CreateVacation(DateTime startDate, DateTime endDate, bool isHalfDayVacation, string type)
        {
            int result = VacationService.CreateVacation(startDate, endDate, isHalfDayVacation, type);
            if (result > 0)
            {

            }
            else
            {
                if (result == -1)
                {
                    ViewBag.Message = $"Type must be 'SickLeave', 'Paid' or 'Unpaid'!";
                    return View();
                }
                else if (result == -2)
                {
                    ViewBag.Message = $"Start date must be earlier than end date!";
                    return View();
                }
            }

            CheckLoggedUser();
            return this.RedirectToAction("Index", "Home");
        }

        public IActionResult EditVacation(int id)
        {
            Vacation vacation = context.Vacations.FirstOrDefault(v => v.Id == id);
            if (vacation == null)
            {
                return NotFound();
            }

            EditVacationViewModel editVacationViewModel= new EditVacationViewModel
            {
                Id = vacation.Id,
                StartDate = vacation.StartDate,
                EndDate = vacation.EndDate,
                IsHalfDayVacation = vacation.IsHalfDayVacation,
                Type = vacation.Type
            };

            return this.View(editVacationViewModel);
        }

        [HttpPost]
        public IActionResult EditVacation(EditVacationViewModel model)
        {
            int result = VacationService.EditVacation(model);
            if (result > 0)
            {

            }
            else
            {
                if (result == -1)
                {
                    ViewBag.Message = $"Type must be 'SickLeave', 'Paid' or 'Unpaid'!";
                    return View();
                }
                else if (result == -2)
                {
                    ViewBag.Message = $"Start date must be earlier than end date!";
                    return View();
                }
            }

            CheckLoggedUser();
            return RedirectToAction("ViewCreatedVacations", "Vacations");
        }

        public IActionResult DeleteVacation(int id)
        {
            VacationService.DeleteVacation(id);
            return this.RedirectToAction("index", "Home");
        }

        public IActionResult ViewVacation(int id)
        {
            ViewData["Vacation"] = VacationService.ViewVacation(id);

            CheckLoggedUser();
            return View();
        }
        
        public IActionResult ViewCreatedVacations()
        {
            ViewData["CreatedVacations"] = VacationService.GetAllVacations();
            CheckLoggedUser();
            ViewData["Context"] = context;
            return View();
        }

        public IActionResult ApproveVacation(int id)
        {
            VacationService.ApproveVacation(id);
            return RedirectToAction("Index", "Home");
        }
    }
}
