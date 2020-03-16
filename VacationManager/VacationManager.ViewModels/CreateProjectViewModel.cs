using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VacationManager.ViewModels
{
    public class CreateProjectViewModel
    {
        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a first name")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$", ErrorMessage = "A name should only contain letters")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a description")]
        [RegularExpression("[A-Za-z][A-Za-z0-9.,;/?_\\s*]{10,200}", ErrorMessage = "A description should not contain only numbers and must be between 10 and 200 characters")]
        public string Description { get; set; }
    }
}
