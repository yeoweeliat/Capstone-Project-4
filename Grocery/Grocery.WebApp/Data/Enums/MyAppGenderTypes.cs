using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;



namespace Grocery.WebApp.Data.Enums // inside data folder, inside enums folder
{
    public enum MyAppGenderTypes
    {

        [Display(Name = "Male")] // display when this particular word appears, define the way you want smth to be displayed
        Male,

        [Display(Name = "Female")]
        Female,

        [Display(Name = "Third Gender")]
        ThirdGender

    }
}
