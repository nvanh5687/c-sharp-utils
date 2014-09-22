using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace anhnv.csharp.utils.Sercurity
{
    public class Cryptography
    {
        public static string CreateStreamChecksum(Stream inputStream, HashAlgorithm algorithm)
        {
            if (inputStream == null) throw new ArgumentNullException("inputStream");
            if (algorithm == null) throw new ArgumentNullException("algorithm");

            byte[] hash = algorithm.ComputeHash(inputStream);
            return BitConverter.ToString(hash).Replace("-", String.Empty);
        }

        public static string GenerateMd5Hash(string text)
        {
            var md5 = MD5.Create();
            var bytes = Encoding.UTF8.GetBytes(text);
            var sb = new StringBuilder();
            foreach (var b in md5.ComputeHash(bytes))
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
