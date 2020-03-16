using System;
using System.Collections.Generic;
using System.Text;

namespace VacationManager.Data.Models
{
    public class Project
    {
        public Project()
        {
            Teams = new List<Team>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Team> Teams { get; set; }

    }
}
