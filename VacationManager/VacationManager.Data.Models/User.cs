using System;
using System.Collections.Generic;
using System.Text;

namespace VacationManager.Data.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Role { get; set; }

        public int PermissionLevel { get; set; }

        public int? TeamId { get; set; }

        public Team Team { get; set; }
    }
}
