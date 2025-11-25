using System.Security.Cryptography;


namespace apiPrisma.Helpers
{
    public static class SecretHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;
        private const char SegmentDelimiter = ':';

        public static string Hash(string input)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, Iterations, Algorithm, KeySize);

            return string.Join(SegmentDelimiter,
                Convert.ToHexString(hash),
                Convert.ToHexString(salt),
                Iterations.ToString(),
                Algorithm.Name);
        }

        public static bool Verify(string input, string hashString)
        {
            var segments = hashString.Split(SegmentDelimiter);

            if (segments.Length != 4)
                throw new FormatException("Hash string format is invalid.");

            byte[] hash = Convert.FromHexString(segments[0]);
            byte[] salt = Convert.FromHexString(segments[1]);
            int iterations = int.Parse(segments[2]);
            var algorithm = new HashAlgorithmName(segments[3]);

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                input, salt, iterations, algorithm, hash.Length);

            return CryptographicOperations.FixedTimeEquals(inputHash, hash);
        }
    }
}
