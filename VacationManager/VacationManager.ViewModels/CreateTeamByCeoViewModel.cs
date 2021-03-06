﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VacationManager.ViewModels
{
    public class CreateTeamByCeoViewModel
    {
        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the name of the team")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$", ErrorMessage = "A name should only contain letters")]
        public string Name { get; set; }

        [Display(Name = "ProjectName")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the name of the project the team is working on")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$", ErrorMessage = "A project name should only contain letters")]
        public string ProjectName { get; set; }

        [Display(Name = "TeamLeadFirstName")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the first name of the team lead")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$", ErrorMessage = "The name should only contain letters")]
        public string TeamLeadFirstName { get; set; }

        [Display(Name = "TeamLeadLastName")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the last name of the team lead")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$", ErrorMessage = "The name should only contain letters")]
        public string TeamLeadLastName { get; set; }
    }
}
