using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Entities;
using Library.ViewModels;
using Library.Data;
using Library.ViewModels.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Library.Helpers;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using FluentEmail.Smtp;
using System.Net.Mail;
using FluentEmail.Core;
using System.Text;
using FluentEmail.Razor;
using Library.Interfaces;

namespace Library.Controllers
{
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(IMapper mapper,IUserRepository userRepository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
        } 
        private bool StudentExists(string name)
        {
            return _userRepository.CheckIfUserExist(name);
        }

        public IActionResult Create()
        {
            var registerViewModel = new RegisterViewModel();
            return View(registerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterViewModel registerViewModel)
        {
            var validationResult = new RegisterResultViewModel();
            try
            {
                ValidateRegistrationModel(validationResult, registerViewModel);
                if (validationResult.IsSuccess)
                {
                    var user = new ApplicationUser { UserName = registerViewModel.Username, Email = registerViewModel.Email, EmailConfirmed = false };
                    var result = await _userManager.CreateAsync(user, registerViewModel.Password);
                   
                    //email confirmation
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "User",new { userId = user.Id, token = token }, Request.Scheme);

                    //take the link for confirmation from current directory
                    if(!String.IsNullOrWhiteSpace(confirmationLink))WriteToFile(confirmationLink);

                    //make every user as a student
                    var student = new Student { Name = registerViewModel.Username,Email=registerViewModel.Email,CreatedDate=DateTime.Now }; 
                    if (!StudentExists(registerViewModel.Username))
                    {
                        await _userRepository.CreateUser(student);
                    }
                    var Studentid = await _userRepository.GetStudentId(student);

                    await _userManager.AddClaimAsync(user, new Claim("Student", Studentid));

                    validationResult = CheckForRegistrationErrors(validationResult, result, registerViewModel.Email);
                    if (!validationResult.IsSuccess)
                        return BadRequest(validationResult);
                    else
                    {
                        //send email functionallity

                        var sender = new SmtpSender(() => new SmtpClient(host: "localhost")
                        {
                            EnableSsl = false,//this only for test,in production i want it allways true
                            DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                            PickupDirectoryLocation = @$"{Directory.GetCurrentDirectory()}\Emails"
                        });

                        //create template
                        StringBuilder template = new();
                        template.AppendLine(value: "Dear @Model.FirsteName .");
                        template.AppendLine(value: "<p>You have successfully registrate to our application.</p>");


                        Email.DefaultSender = sender;
                        Email.DefaultRenderer = new RazorRenderer();//for razor syntax 

                        var email=Email
                            .From(emailAddress:"test@test.com")//client
                            .To(emailAddress:$"{registerViewModel.Email}",name:$"{registerViewModel.Username}")
                            .Subject(subject:"Thanks")
                            .UsingTemplate(template.ToString(), new { FirsteName = $"{registerViewModel.Username}" })                           
                            .Send();

                        return RedirectToAction(nameof(Login)); 
                    }
                }
                else
                    return BadRequest(validationResult);
            }
            catch (Exception exc)
            {
                validationResult.GeneralError = exc.Message;
                validationResult.IsSuccess = false;
                return BadRequest(validationResult);
            }
        }
        public void WriteToFile(string messageIn)
        {
            string directory = @$"{Directory.GetCurrentDirectory()}\\Links";
            string filename = String.Format("{0:yyyy-MM-dd}__{1}", DateTime.Now, "LibraryTest");
            string path = Path.Combine(directory, filename);

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter str = new StreamWriter(fs))
                {             
                    str.WriteLine(messageIn);          
                    str.Flush();
                }
            }
        }
        public IActionResult Login()
        {
            var loginViewModel = new LoginViewModel();
            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            var validationResult = new LoginResultViewModel();
            try
            {
                if (!string.IsNullOrEmpty(loginViewModel.UserName) && !string.IsNullOrEmpty(loginViewModel.Password))
                {
                    // ensure we have a user with the given user name caution to NormalizedUserName(AspNetUsers)
                    var user = await _userManager.FindByNameAsync(loginViewModel.UserName);
                    if (user != null)
                    {
                        var signInResult = await _signInManager.PasswordSignInAsync(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe, false);
                        if (signInResult.Succeeded)
                        {
                            return Redirect("/");                            
                        }
                        else
                        {
                            validationResult.IsSuccess = false;
                            validationResult.UserOrPasswordWrong = true;
                            return BadRequest(validationResult);
                        }
                    }
                    else
                    {
                        validationResult.IsSuccess = false;
                        validationResult.UserOrPasswordWrong = true;
                        return BadRequest(validationResult);
                    }
                }
                else
                    return BadRequest();

            }
            catch (Exception)
            {
                validationResult.IsSuccess = false;
                return BadRequest(validationResult);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("NotFound");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
              
                var user = await _userManager.FindByEmailAsync(model.Email);
           
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {

                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);


                    var passwordResetLink = Url.Action("ResetPassword", "User",
                            new { email = model.Email, token = token }, Request.Scheme);
                    if (!String.IsNullOrWhiteSpace(passwordResetLink)) WriteToFile(passwordResetLink);
                    return View("ForgotPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return View("NotFound");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    // reset the user password
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    // Display validation errors. For example, password reset token already
                    // used to change the password or password complexity rules not met
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return View("ResetPasswordConfirmation");
            }
            // Display validation errors if model state is not valid
            return View(model);
        }




        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

        #region Helpers
        private void ValidateRegistrationModel(RegisterResultViewModel validationResult, RegisterViewModel model)
        {
            if (model.Password.Length < 8)
            {
                validationResult.InvalidPassword = true;
                validationResult.IsSuccess = false;
            }
            var isValidEmail = Regex.IsMatch(model.Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            if (!isValidEmail)
            {
                validationResult.InvalidEmail = true;
                validationResult.IsSuccess = false;
            }
        }
        private RegisterResultViewModel CheckForRegistrationErrors(RegisterResultViewModel validationResult, IdentityResult result, string email)
        {
            var error = result.Errors.FirstOrDefault();
            if (error != null)
            {
                if (error.Code == nameof(LocalizedIdentityErrorDescriber.DuplicateUserName) || error.Code == nameof(LocalizedIdentityErrorDescriber.DuplicateEmail))
                    validationResult.DuplicateUser = true;
                else
                if (error.Code == nameof(LocalizedIdentityErrorDescriber.InvalidUserName))
                    validationResult.InvalidUserName = true;
                else
                if (error.Code == nameof(LocalizedIdentityErrorDescriber.InvalidEmail))
                    validationResult.InvalidEmail = true;
                else
                if (error.Code == nameof(LocalizedIdentityErrorDescriber.PasswordMismatch)
                    || error.Code == nameof(LocalizedIdentityErrorDescriber.PasswordRequiresDigit)
                    || error.Code == nameof(LocalizedIdentityErrorDescriber.PasswordRequiresLower)
                    || error.Code == nameof(LocalizedIdentityErrorDescriber.PasswordRequiresNonAlphanumeric)
                    || error.Code == nameof(LocalizedIdentityErrorDescriber.PasswordRequiresUpper)
                    || error.Code == nameof(LocalizedIdentityErrorDescriber.PasswordTooShort))
                    validationResult.InvalidPassword = true;
                else
                    validationResult.GeneralError = error.Description;

                validationResult.IsSuccess = false;
            }

            return validationResult;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion
    }
}
