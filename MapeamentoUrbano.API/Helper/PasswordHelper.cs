using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace MapeamentoUrbano.API.Helper
{
    public class PasswordHasherOptions
    {
        private static readonly RandomNumberGenerator _defaultRng = RandomNumberGenerator.Create();

        internal RandomNumberGenerator Rng { get; set; } = _defaultRng;
    }

    public class PasswordHelper
    {
        private readonly RandomNumberGenerator _rng;

        public PasswordHelper(IOptions<PasswordHasherOptions> optionsAccessor = null)
        {
            var options = optionsAccessor?.Value ?? new PasswordHasherOptions();
            _rng = options.Rng;
        }

        #region [Hash Password]
        public string HashPassword(string password)
        {
            ArgumentNullException.ThrowIfNull(nameof(password));

            return Convert.ToBase64String(HashPassword(password, _rng));
        }

        private static byte[] HashPassword(string password, RandomNumberGenerator rng)
        { 
            const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA1; // default for Rfc2898DeriveBytes
            const int Pbkdf2IterCount = 1000; // default for Rfc2898DeriveBytes
            const int Pbkdf2SubkeyLength = 256 / 8; // 256 bits
            const int SaltSize = 128 / 8; // 128 bits

            byte[] salt = new byte[SaltSize];
            rng.GetBytes(salt);
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, Pbkdf2Prf, Pbkdf2IterCount, Pbkdf2SubkeyLength);

            var outputBytes = new byte[1 + SaltSize + Pbkdf2SubkeyLength];
            outputBytes[0] = 0x00; // format marker
            Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, Pbkdf2SubkeyLength);
            return outputBytes;
        }
        #endregion

        #region [Verify Hashed Password]
        public virtual bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            ArgumentNullException.ThrowIfNull(nameof(hashedPassword));

            ArgumentNullException.ThrowIfNull(nameof(providedPassword));

            byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);

            // read the format marker from the hashed password
            if (decodedHashedPassword.Length == 0)
                return false; 

            return VerifyHashedPassword(decodedHashedPassword, providedPassword);
        }   

        private static bool VerifyHashedPassword(byte[] hashedPassword, string password)
        {
            const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA1; // default for Rfc2898DeriveBytes
            const int Pbkdf2IterCount = 1000; // default for Rfc2898DeriveBytes
            const int Pbkdf2SubkeyLength = 256 / 8; // 256 bits
            const int SaltSize = 128 / 8; // 128 bits

            // We know ahead of time the exact length of a valid hashed password payload.
            if (hashedPassword.Length != 1 + SaltSize + Pbkdf2SubkeyLength)
                return false; // bad size

            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(hashedPassword, 1, salt, 0, salt.Length);

            byte[] expectedSubkey = new byte[Pbkdf2SubkeyLength];
            Buffer.BlockCopy(hashedPassword, 1 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

            // Hash the incoming password and verify it
            byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, Pbkdf2Prf, Pbkdf2IterCount, Pbkdf2SubkeyLength);
            return ByteArraysEqual(actualSubkey, expectedSubkey);
        }

        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null || a.Length != b.Length)
                return false;

            var areSame = true;
            for (var i = 0; i < a.Length; i++)
                areSame &= (a[i] == b[i]);

            return areSame;
        }
        #endregion
    }
}
