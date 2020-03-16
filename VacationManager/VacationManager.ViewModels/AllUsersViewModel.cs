using System;
using System.Collections.Generic;
using System.Text;
using VacationManager.Data.Models;

namespace VacationManager.ViewModels
{
    public class AllUsersViewModel
    {
        public IEnumerable<User> Users { get; set; }
    }
}
