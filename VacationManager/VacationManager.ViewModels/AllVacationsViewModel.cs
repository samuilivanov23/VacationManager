using System;
using System.Collections.Generic;
using System.Text;
using VacationManager.Data.Models;

namespace VacationManager.ViewModels
{
    public class AllVacationsViewModel
    {
        public IEnumerable<Vacation> Vacations { get; set; }
    }
}
