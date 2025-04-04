using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace WebAPP.Security
{
    public class PasswordHashProvider
    {
        public static string GetSalt()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            string base64Salt = Convert.ToBase64String(salt);

            return base64Salt;
        }

        public static string GetHash(string password, string base64Salt)
        {
            byte[] salt = Convert.FromBase64String(base64Salt);

            byte[] hash = KeyDerivation.Pbkdf2(
                               password: password,
                                              salt: salt,
                                                             prf: KeyDerivationPrf.HMACSHA256,
                                                                            iterationCount: 10000,
                                                                                           numBytesRequested: 256 / 8);
            string base64Hash = Convert.ToBase64String(hash);
            return base64Hash;
        }
    }
}
