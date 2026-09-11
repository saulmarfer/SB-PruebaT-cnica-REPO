using System.Security.Cryptography;
using SB.PruebaTecnica.Application.Interfaces;

namespace SB.PruebaTecnica.Infrastructure.Security
{
    /// <summary>
    /// Hashing de contraseñas con PBKDF2 (Rfc2898DeriveBytes), sin depender de
    /// librerías externas de terceros. El salt se genera por contraseña y se
    /// almacena concatenado junto al hash.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private const int TamanoSalt = 16;
        private const int TamanoHash = 32;
        private const int Iteraciones = 100_000;

        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(TamanoSalt);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);

            var resultado = new byte[TamanoSalt + TamanoHash];
            Buffer.BlockCopy(salt, 0, resultado, 0, TamanoSalt);
            Buffer.BlockCopy(hash, 0, resultado, TamanoSalt, TamanoHash);

            return Convert.ToBase64String(resultado);
        }

        public bool Verificar(string password, string hashAlmacenado)
        {
            var bytes = Convert.FromBase64String(hashAlmacenado);

            var salt = new byte[TamanoSalt];
            Buffer.BlockCopy(bytes, 0, salt, 0, TamanoSalt);

            var hashEsperado = new byte[TamanoHash];
            Buffer.BlockCopy(bytes, TamanoSalt, hashEsperado, 0, TamanoHash);

            var hashCalculado = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);

            return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
        }
    }
}
