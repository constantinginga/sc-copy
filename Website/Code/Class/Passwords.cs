using StartupCentral.Code.Controllers;
using System;

namespace StartupCentral.Code.Class
{
    public static class Passwords
    {
        /// <summary>
        /// Generates a new password for a member based on Umbraco's rules.
        /// </summary>
        /// <returns></returns>
        public static string GenerateNewPassword()
        {
            PasswordController passwordController = new PasswordController();
            string newPassword = System.Web.Security.Membership.GeneratePassword(passwordController.GetMinRequiredPasswordLength(), 0);
            Random rnd = new Random();
            return System.Text.RegularExpressions.Regex.Replace(newPassword, @"[^0-9]", m => rnd.Next(0, 9).ToString());
        }
    }
}