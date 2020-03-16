using System;
using System.Collections.Generic;
using System.Text;
using VacationManager.ViewModels;
using VacationManager.Data.Models;

namespace VacationManager.Services.Interfaces
{
    public interface IVacationService
    {
        int CreateVacation(DateTime startDate, 
                            DateTime endDate, 
                            bool isHalfDayVacation, 
                            string vacationType);

        int EditVacation(EditVacationViewModel model);

        void DeleteVacation(int id);

        Vacation ViewVacation(int id);

        AllVacationsViewModel GetAllVacations();

        void ApproveVacation(int id);
    }
}
