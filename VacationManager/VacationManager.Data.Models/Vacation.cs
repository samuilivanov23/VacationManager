using System;
using System.Collections.Generic;
using System.Text;

namespace VacationManager.Data.Models
{
    public class Vacation
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime VacationCreatedOn { get; set; }

        public bool Approved { get; set; }

        public bool IsHalfDayVacation { get; set; }

        public string Type { get; set; }

        public User Applicant { get; set; }
    }
}
