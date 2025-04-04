using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Security
{
    public class JwtTokenProvider
    {
        public static string CreateToken(string secureKey, int expiration, string subject = null)
        {
            //Get bytes
              var tokenKey = Encoding.UTF8.GetBytes(secureKey);

            //Create token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            { 
                Expires = DateTime.UtcNow.AddMinutes(expiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            if (!string.IsNullOrEmpty(subject))
            {
                tokenDescriptor.Subject = new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[]
                {
                    new System.Security.Claims.Claim(ClaimTypes.Name, subject),
                    new System.Security.Claims.Claim(ClaimTypes.Role, "Admin"),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, subject),
                });
            }

            //Create token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var serializedToken = tokenHandler.WriteToken(token);

            return serializedToken;

        }
    }
}
