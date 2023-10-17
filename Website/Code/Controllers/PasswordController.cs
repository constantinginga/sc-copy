using StartupCentral.Code.Model;
using Umbraco.Core.Security;
using Umbraco.Web.Mvc;

namespace StartupCentral.Code.Controllers
{
    public class PasswordController
    {

        public ApiResponse ValidatePasswords(string pass1, string pass2)
        {
            //var userMembershipProvider = MembershipProviderExtensions.GetUsersMembershipProvider();

            if (!string.IsNullOrEmpty(pass1))
            {
                var lengthRes = ValidatePasswordLength(pass1);
                
                if (!lengthRes.Success)
                {
                    return lengthRes;
                }

                if (pass1 != pass2)
                {
                    return new ApiResponse(false, "Du har ikke skrevet adgangskoden ens begge steder.");
                    //return new ApiResponse(false, "You did not enter the same password in both places.");
                }
            }
            else
            {
                return new ApiResponse(false, "Adgangskode skal udfyldes.");
                //return new ApiResponse(false, "Password must be filled in.");
            }

            return new ApiResponse(true, "OK");
        }

        public ApiResponse ValidatePasswordLength(string pass)
        {
            var userMembershipProvider = MembershipProviderExtensions.GetUsersMembershipProvider();

            if (!string.IsNullOrEmpty(pass))
            {
                if (pass.Length < userMembershipProvider.MinRequiredPasswordLength) return new ApiResponse(false, $"Adgangskoden er for kort. Minimum {userMembershipProvider.MinRequiredPasswordLength} karakterer.");
            }
            else
            {
                return new ApiResponse(false, "Adgangskode skal udfyldes.");
            }

            return new ApiResponse(true, "OK");
        }

        public int GetMinRequiredPasswordLength()
        {
            var userMembershipProvider = MembershipProviderExtensions.GetUsersMembershipProvider();
            return userMembershipProvider.MinRequiredPasswordLength;
        }
    }
}