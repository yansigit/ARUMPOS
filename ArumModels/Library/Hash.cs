using System.Security.Cryptography;
using System.Text;

namespace ArumModels.Library
{
    public class Hash
    {
        public static string HashSha1(string data)
        {
            using var sha1 = SHA1.Create();
            var bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(data));
            var s = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                s.Append(b.ToString("X2"));
            }
            return s.ToString();
        }
    }
}