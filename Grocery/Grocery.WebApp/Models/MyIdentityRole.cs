using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


// it is a must to have this as well.
// add at least 1 property for the CustomIdentityRole
namespace Grocery.WebApp.Models
{
    public class MyIdentityRole : IdentityRole<Guid>
    {
        //[Required]
        [MaxLength(100)] // add constraint on the user interface (ui) // both need to be defined seperately
        [StringLength(100)] //add constraint on db schema

        public string Description { get; set; }

    }
}
