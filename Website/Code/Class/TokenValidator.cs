using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace StartupCentral.Code.Class
{
    public static class TokenValidator
    {
        public static string VeryVeryVerySecretPasswordKey = "sdflj34fsæ34213tggqwert";

        public static async Task<ClaimsPrincipal> ValidateTokenAsync(string token)
        {
            SecurityToken securityToken = new JwtSecurityToken(token);
            JwtSecurityTokenHandler securityTokenHandler = new JwtSecurityTokenHandler();

            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidIssuer = "https://www.startupcentral.dk",
                ValidAudience = "https://www.startupcentral.dk",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(VeryVeryVerySecretPasswordKey))
            };

            ClaimsPrincipal claimsPrincipal = null;
            try
            {
                claimsPrincipal = securityTokenHandler.ValidateToken(token, validationParameters, out securityToken);
                return await System.Threading.Tasks.Task.FromResult<ClaimsPrincipal>(claimsPrincipal);
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException)
            {
            }

            return null;
        }

        public static IMember ValidateToken(string token)
        {
            ClaimsPrincipal claimsPrincipal = null;
            try
            {
                claimsPrincipal = ValidateTokenAsync(token).Result;
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException)
            {
                // Token is expired - after 10 years?
                //return null;
            }

            return ValidateUserByClaim(claimsPrincipal, token);
        }

        public static IMember ValidateUserByClaim(ClaimsPrincipal claimsPrincipal, string token)
        {
            if (claimsPrincipal != null)
            {
                string userKey = claimsPrincipal.Identities?.First()?.Claims?.Where(itm => itm.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").First()?.Value;
                if (!string.IsNullOrEmpty(userKey))
                {
                    Guid? guidKey = null;
                    try
                    {
                        guidKey = new Guid(userKey);
                    }
                    catch (System.Exception)
                    {
                    }

                    if (guidKey != null)
                    {
                        var member = ApplicationContext.Current.Services.MemberService.GetByKey(guidKey.Value);

                        if (member != null)
                        {
                            return member;
                        }
                    }
                }
            }

            return null;
        }

        public static JwtSecurityToken GetJwtSecurityToken(Guid userKey)
        {
            return new JwtSecurityToken(
                issuer: "https://www.startupcentral.dk",
                audience: "https://www.startupcentral.dk",
                claims: GetTokenClaims(userKey),
                expires: DateTime.UtcNow.AddYears(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(VeryVeryVerySecretPasswordKey)), SecurityAlgorithms.HmacSha256)
            );
        }

        private static IEnumerable<Claim> GetTokenClaims(Guid userKey)
        {
            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, userKey.ToString())
            };
        }

        public static IMember GetCurrentMember(System.Web.HttpRequest request)
        {
            if (request != null)
            {
                if (string.IsNullOrEmpty(request.Headers["Authorization"]))
                {
                    return null;
                }

                var token = request.Headers["Authorization"].Split(' ').Last();

                ClaimsPrincipal claimsPrincipal = null;
                try
                {
                    claimsPrincipal = ValidateTokenAsync(token).Result;
                }
                catch (Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException)
                {
                    return null;
                }

                string userKey = claimsPrincipal.Identities?.First()?.Claims?.Where(itm => itm.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").First()?.Value;
                if (!string.IsNullOrEmpty(userKey))
                {
                    return ApplicationContext.Current.Services.MemberService.GetByKey(new Guid(userKey));
                }
            }

            return null;
        }
    }
}