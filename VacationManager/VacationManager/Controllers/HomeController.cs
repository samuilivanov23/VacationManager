using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VacationManager.Controllers.Interfaces;
using VacationManager.Data.Models;
using VacationManager.Models;
using VacationManager.Services;
using VacationManager.Services.Interfaces;
using VacationManager.Data;

namespace VacationManager.Controllers
{
    public class HomeController : Controller, ICheckLoggedUser
    {
        private IUserService UserService;
        private IVacationService VacationService;
        private VacationManagerDbContext context;
        private User loggedUser;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUserService UserService, IVacationService VacationService, VacationManagerDbContext context)
        {
            _logger = logger;
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

        public IActionResult Index()
        {
            CheckLoggedUser();

            ViewData["CreatedVacations"] = VacationService.GetAllVacations();
            CheckLoggedUser();
            //ViewData["Context"] = context;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
