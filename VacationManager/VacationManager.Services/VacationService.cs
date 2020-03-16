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

        /// <summary>
        /// This method is used to create a vacation
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isHalfDayVacation"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int CreateVacation(DateTime startDate, DateTime endDate, bool isHalfDayVacation, string type)
        {
            //Validating the vacation information
            if (type != "Unpaid" && type != "Paid" && type != "Sick Leave")
            {
                return -1;
            }
            else if(DateTime.Compare(startDate, endDate) >= 0)
            {
                return -2;
            }


            User loggedUser = userService.GetLoggedUser();

            //Creating the new vacation
            var vacation = new Vacation
            {
                StartDate = startDate,
                EndDate = endDate,
                IsHalfDayVacation = isHalfDayVacation,
                Approved = false,
                Type = type,
                VacationCreatedOn = DateTime.UtcNow
            };

            //Setting the applicant to be the logged user
            vacation.Applicant = loggedUser;
            //Saving the changes in the databse.
            context.Vacations.Add(vacation);
            context.SaveChanges();

            return vacation.Id;
        }

        /// <summary>
        /// This method is used to edit a given vacation's information
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditVacation(EditVacationViewModel model)
        {
            //Taking the correct vacation based on the given model id
            Vacation takenVacation = context.Vacations.FirstOrDefault(v => v.Id == model.Id);

            //Validating the new vacation information
            if (model.Type != "Unpaid" && model.Type != "Paid" && model.Type != "SickLeave")
            {
                return -1;
            }
            else if (DateTime.Compare(model.StartDate, model.EndDate) >= 0)
            {
                return -2;
            }

            //Editing the vacation information
            takenVacation.StartDate = model.StartDate;
            takenVacation.EndDate = model.EndDate;
            takenVacation.IsHalfDayVacation = model.IsHalfDayVacation;
            takenVacation.Type = model.Type;

            //Saving the changes in the databse
            context.Vacations.Update(takenVacation);
            context.SaveChanges();

            return takenVacation.Id;
        }

        /// <summary>
        /// This method is used to delete a vacation.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteVacation(int id)
        {
            //Taking the correct vacation based on the given id.
            Vacation vacation = context.Vacations.FirstOrDefault(v => v.Id == id);

            //Removing the vacation from the database and saving the changes in the database.
            context.Vacations.Remove(vacation);
            context.SaveChanges();
        }

        /// <summary>
        /// This method is used to return a vacation that is shown in the ViewVacation view.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

            //Filtering the vacation based on the given id.
            return vacations.FirstOrDefault(v => v.Id == id);
        }

        /// <summary>
        /// This method is used to return a model with all teh vacations in the database.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// This method is used when approving a request for vacation.
        /// </summary>
        /// <param name="id"></param>
        public void ApproveVacation(int id)
        {
            Vacation vacation = context.Vacations.FirstOrDefault(v => v.Id == id);
            vacation.Approved = true;
            context.Vacations.Update(vacation);
            context.SaveChanges();
        }
    }
}
