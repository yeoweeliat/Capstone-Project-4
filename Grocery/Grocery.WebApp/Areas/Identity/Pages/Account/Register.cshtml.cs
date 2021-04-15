using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Grocery.WebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Grocery.WebApp.Data.Enums;

namespace Grocery.WebApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<MyIdentityUser> _signInManager;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<MyIdentityUser> userManager,
            SignInManager<MyIdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }


        // @Model.ReturnUrl will pull the value here
        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        // The viewmodel for the register page
        public class InputModel
        {  
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


            // need to change both Register.cshtml.cs and Register.cshtml

            [Required (ErrorMessage = "How would you like your name to be displayed?")]
            [Display(Name = "Display Name")]
            [MinLength(2)]
            [MaxLength(60)]
            //[StringLength(60)] // can remove db related
            public string DisplayName { get; set; }

            [Required (ErrorMessage = "Please select your gender...")]
            [Display(Name = "Gender")]
            //[PersonalData] // for GDPR compliance, infor
            public MyAppGenderTypes Gender { get; set; }

            
            [Required (ErrorMessage = "Please enter your date of birth...")]
            [Display(Name = "Date of Birth")]
            //[PersonalData]
            //[Column(TypeName="smalldatetime")] //create 
            public DateTime DateOfBirth { get; set; }


        }

        //onget displays the form
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        //onpost captures data on the form
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid) // if modelstate is valid, user object is created
            {
                var user = new MyIdentityUser 
                { 
                    UserName = Input.Email, 
                    Email = Input.Email,              
                    DateOfBirth = Input.DateOfBirth, //populate additional fields we want
                    DisplayName = Input.DisplayName,
                    Gender = Input.Gender,
                    IsAdminUser = false // customer user, hence always false
                };

                var result = await _userManager.CreateAsync(user, Input.Password); // where user is created

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Add user to the role of customeroption
                    await _userManager.AddToRoleAsync(user, MyAppRoleTypes.Customer.ToString());


                    //generate email confirmation
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount) // does app require confirmed account
                    {
                        // add here
                        try
                        {
                            // send email to [mailtrap] to ask the person to confirm his account
                            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                           $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                        }
                        catch(Grocery.WebApp.Services.MyEmailSenderException exp)
                        {
                            // error msg: user confirmed successfully, please confirm...
                            string message = $"user Created!! Confirm using {callbackUrl}. Error: {exp.Message}";
                            ModelState.AddModelError("RegistrationEmail", message);

                            return Page();
                        }

                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
