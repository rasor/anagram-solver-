using System;
using System.Text;
using System.Security.Cryptography;

namespace anagramsolver.helpers
{
    /// <summary>
    /// Methods for computing md5
    /// </summary>
    public class Md5Helper
    {
        public bool VerifyMd5Hash(MD5 md5HashComputer, string input, string[] md5HashArrayToVerifyAgainst)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5HashComputer, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            // Check aginst each md5 string 
            bool result = false;
            foreach (var md5HashToVerifyAgainst in md5HashArrayToVerifyAgainst)
            {
                result = (0 == comparer.Compare(hashOfInput, md5HashToVerifyAgainst));
                if (true)
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/system.security.cryptography.md5.aspx
        /// </summary>
        /// <param name="input"></param>
        /// <param name="md5HashToVerifyAgainst"></param>
        /// <returns></returns>
        public bool VerifyMd5Hash(MD5 md5HashComputer, string input, string md5HashToVerifyAgainst)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5HashComputer, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, md5HashToVerifyAgainst))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/system.security.cryptography.md5.aspx
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GetMd5Hash(MD5 md5HashComputer, string input)
        {
            StringBuilder hash = new StringBuilder();
            byte[] bytes = md5HashComputer.ComputeHash(Encoding.UTF8.GetBytes(input));
            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
