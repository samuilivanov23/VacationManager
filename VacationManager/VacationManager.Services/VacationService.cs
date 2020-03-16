using System;
using System.Collections.Generic;
using System.Text;
using VacationManager.Data;
using VacationManager.Data.Models;
using VacationManager.ViewModels;
using VacationManager.Services.Interfaces;
using System.Linq;

namespace VacationManager.Services
{
    public class VacationService : IVacationService
    {
        private VacationManagerDbContext context;
        private IUserService userService;

        public VacationService(VacationManagerDbContext context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        public int CreateVacation(DateTime startDate, DateTime endDate, bool isHalfDayVacation, string type)
        {
            if (type != "Unpaid" && type != "Paid" && type != "Sick Leave")
            {
                return -1;
            }
            else if(DateTime.Compare(startDate, endDate) >= 0)
            {
                return -2;
            }

            User loggedUser = userService.GetLoggedUser();
            var vacation = new Vacation
            {
                StartDate = startDate,
                EndDate = endDate,
                IsHalfDayVacation = isHalfDayVacation,
                Approved = false,
                Type = type,
                VacationCreatedOn = DateTime.UtcNow
            };

            vacation.Applicant = loggedUser;
            context.Vacations.Add(vacation);
            context.SaveChanges();

            return vacation.Id;
        }

        public int EditVacation(EditVacationViewModel model)
        {
            Vacation takenVacation = context.Vacations.FirstOrDefault(v => v.Id == model.Id);

            if (model.Type != "Unpaid" && model.Type != "Paid" && model.Type != "SickLeave")
            {
                return -1;
            }
            else if (DateTime.Compare(model.StartDate, model.EndDate) >= 0)
            {
                return -2;
            }

            takenVacation.StartDate = model.StartDate;
            takenVacation.EndDate = model.EndDate;
            takenVacation.IsHalfDayVacation = model.IsHalfDayVacation;
            takenVacation.Type = model.Type;

            context.Vacations.Update(takenVacation);
            context.SaveChanges();

            return takenVacation.Id;
        }

        public void DeleteVacation(int id)
        {
            Vacation vacation = context.Vacations.FirstOrDefault(v => v.Id == id);

            context.Vacations.Remove(vacation);
            context.SaveChanges();
        }

        public Vacation ViewVacation(int id)
        {
            var vacations = context.Vacations.Select(v => new Vacation()
            {
                Id = v.Id,
                StartDate = v.StartDate,
                EndDate = v.EndDate,
                VacationCreatedOn = v.VacationCreatedOn,
                Applicant = v.Applicant,
                Approved = v.Approved,
                Type = v.Type,
            });

            return vacations.FirstOrDefault(v => v.Id == id);
        }

        public AllVacationsViewModel GetAllVacations()
        {
            var vacations = context.Vacations.Select(v => new Vacation()
            {
                Id = v.Id,
                StartDate = v.StartDate,
                EndDate = v.EndDate,
                VacationCreatedOn = v.VacationCreatedOn,
                Applicant = v.Applicant,
                Approved = v.Approved,
                Type = v.Type,
            });

            var model = new AllVacationsViewModel() { Vacations = vacations };

            return model;
        }

        public void ApproveVacation(int id)
        {
            Vacation vacation = context.Vacations.FirstOrDefault(v => v.Id == id);
            vacation.Approved = true;
            context.Vacations.Update(vacation);
            context.SaveChanges();
        }
    }
}
