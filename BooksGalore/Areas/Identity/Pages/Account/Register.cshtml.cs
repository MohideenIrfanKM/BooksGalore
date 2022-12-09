// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using BooksGalore.Models;
using BooksGalore.Repository;
using BooksGalore.Repository.IRepository;
using BooksGalore.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;


namespace BooksGalore.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender; //create dummy emailsender in first place utility class
        private readonly RoleManager<IdentityRole> _roleManager;
        public readonly IUnitofWork _db;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,RoleManager<IdentityRole> roleManager,IUnitofWork db)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _db = db;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel  //THIS IS DUMMY VIEW MODEL ORIGINAL MODEL IS APPLICATIONUSER MODEL WHICH EXTENDS IDENTITY USER
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>


            [Required]
            [DataType(DataType.Password)]
			[Display(Name = "Confirm password")]
			[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
			public string ConfirmPassword { get; set; }

			[Required]
			[Display(Name = "Name")]
			public string Name { get; set; }

			
			
			[Display(Name = "Street Address")]
			public string ?StreetAddress { get; set; }

			[Display(Name = "City")]
			public string ?City { get; set; }

			[Display(Name = "State")]
			public string ?State { get; set; }

			
			[Display(Name = "Postal Code")]
			public string ?PostalCode { get; set; }

            [Phone]
			[Display(Name = "Phone ")]
			public string ?PhoneNumber { get; set; }

            
            public string? Role { get; set; }

           
            public int? CompanyId { get; set; }

            [Display(Name = " Select Role")]
            public IEnumerable<SelectListItem>? Roleslist { get; set; }
            public IEnumerable<SelectListItem>? Companylist { get; set; }
            //THIS IS DUMMY VIEW MODEL ORIGINAL MODEL IS APPLICATIONUSER MODEL WHICH EXTENDS IDENTITY USER

        }


        public async Task OnGetAsync(string returnUrl = null)

        {
            if (!_roleManager.RoleExistsAsync(Util._Emp).GetAwaiter().GetResult()) 
            { 
                _roleManager.CreateAsync(new IdentityRole(Util._ind)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(Util._Emp)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(Util._com)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(Util._Adm)).GetAwaiter().GetResult();

			}
			ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            //input is the object of this InputModel which van be found above
            Input = new InputModel()
            {
                //we are populating the rolelist here to pass it to the views
                Roleslist = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i,
                }),
                Companylist=_db.CompanyRepository.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),//value here is string property
                })

            };
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)//if any problem add ? in string
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);
                user.State=Input.State;
                user.PhoneNumber=Input.PhoneNumber;
                user.City=Input.City;
                user.Name=Input.Name;
                user.StreetAddress=Input.StreetAddress; 
                user.PostalCode=Input.PostalCode;
                if (Input.Role == Util._com)
                {
                    user.CompanyId = Input.CompanyId;
                }
                //we are doing manually above because it is defaulty add useremail and password only so we
                //manually adding it using user object

                if (result.Succeeded)
                {
                    
                    _logger.LogInformation("User created a new account with password.");
          
                    if (Input.Role != null)
                    {
                        await _userManager.AddToRoleAsync(user, Input.Role); //it is related with user so usermanager used
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, Util._ind);
                    }//AddtoRoleS is for assigning multiple roles.
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
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
            Input = new InputModel()
            {
                //we are populating the rolelist here to pass it to the views
                Roleslist = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i,
                })

            };//we added it again because there is some error when form validation unsuccessfull asp-items became null so that only
            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
