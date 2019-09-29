using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BnetworkManagement.Services;

namespace BnetworkManagement.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "BNetwork - Confirm your email",
               
                $"Thank you for signing up with BNetwork!" +
                $"To complete your registration, please click the following link: <a href='{HtmlEncoder.Default.Encode(link)}'>Complete Registration</a> ");
        }
    }
}
