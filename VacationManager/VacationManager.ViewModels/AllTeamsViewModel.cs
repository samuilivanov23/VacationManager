using System;
using System.Collections.Generic;
using System.Text;
using VacationManager.Data.Models;

namespace VacationManager.ViewModels
{
    public class AllTeamsViewModel
    {
        public IEnumerable<Team> Teams { get; set; }
    }
}
