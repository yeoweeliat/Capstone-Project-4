using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Grocery.WebApp.Data.Enums;
using Grocery.WebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Grocery.WebApp.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly SignInManager<MyIdentityUser> _signInManager;

        public IndexModel(
            UserManager<MyIdentityUser> userManager,
            SignInManager<MyIdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }


            [Required(ErrorMessage = "How would you like your name to be displayed?")]
            [Display(Name = "Display Name")]
            [MinLength(2)]
            [MaxLength(60)]
            public string DisplayName { get; set; }


            [Required(ErrorMessage = "Please enter your date of birth...")]
            [Display(Name = "Date of Birth")]
            public DateTime DateOfBirth { get; set; }


            [Required(ErrorMessage = "Please select your gender...")]
            [Display(Name = "Gender")]
            public MyAppGenderTypes Gender { get; set; }


            

        }

        private async Task LoadAsync(MyIdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = user.PhoneNumber,
                DisplayName = user.DisplayName,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user); // method defined above

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                //submit back using the httppost request
                return Page(); // errors on page, exit. //return current page in the current state, eg error, we want to throw back same page and display error msg
            }


            // assign values from viewmodel to user object
            user.DisplayName = Input.DisplayName;
            user.DateOfBirth = Input.DateOfBirth;
            user.Gender = Input.Gender;
            //user.PhoneNumber = Input.PhoneNumber;



            //update the data into the database uing _userManager
            await _userManager.UpdateAsync(user);


            // phone number updated through workflow (its the way the identity management works)
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            #region  Original Code - Commented Block

            //if (!ModelState.IsValid)
            //{
            //    await LoadAsync(user);
            //    return Page(); // errors on page, exit.
            //}

            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            //if (Input.PhoneNumber != phoneNumber)
            //{
            //    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            //    if (!setPhoneResult.Succeeded)
            //    {
            //        StatusMessage = "Unexpected error when trying to set phone number.";
            //        return RedirectToPage();
            //    }
            //}

            #endregion


            await _signInManager.RefreshSignInAsync(user); //refresh so that any claims management value is refreshed
            StatusMessage = "Your profile has been updated"; // inform user

            return RedirectToPage(); // redirect user to the httpget page. (ongetpage)
        }
    }
}
