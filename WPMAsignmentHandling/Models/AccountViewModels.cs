using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WPMAsignmentHandling.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessage = "Email-Adresse wurde nicht eingetragen")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email-Adresse wurde nicht eingetragen")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage="Email-Adresse ist ungültig")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Passwort wurde nicht eingetragen")]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {

        [Required(ErrorMessage = "Vorname wurde nicht eingetragen")]
        public string Vorname { get; set; }

        [Required(ErrorMessage = "Nachname wurde nicht eingetragen")]
        public string Nachname { get; set; }

        [Required(ErrorMessage = "Email-Adresse wurde nicht eingetragen")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefonnummer wurde nicht eingetragen")]
        [Display(Name = "Telefonnummer")]
        public string Telefonnummer { get; set; }

        [Required(ErrorMessage = "Passwort wurde nicht eingetragen")]
        [StringLength(100, ErrorMessage = "Das {0} muss mindesten {2} Zeichen haben.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Passwort bestätigen")]
        [Compare("Password", ErrorMessage = "Passwörter stimmen nicht überein!")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Email-Adresse wurde nicht eingetragen")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email-Adresse ist ungültig")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Das {0} muss mindesten {2} Zeichen haben.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Passwort bestätigen")]
        [Compare("Password", ErrorMessage = "Passwörter stimmen nicht überein!")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email-Adresse wurde nicht eingetragen")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email-Adresse ist ungültig")]
        public string Email { get; set; }
    }
}