using System;
using System.Collections.Generic;
using System.Text;
using VacationManager.Data.Models;

namespace VacationManager.ViewModels
{
    public class AllProjectsViewModel
    {
        public IEnumerable<Project> Projects { get; set; }
    }
}
