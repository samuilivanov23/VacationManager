using System;
using System.Collections.Generic;
using System.Text;

namespace VacationManager.Data.Models
{
    public class Team
    {
        public Team()
        {
            Users = new List<User>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int? ProjectId { get; set; }

        public Project Project { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
