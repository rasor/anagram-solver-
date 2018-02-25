using System;
using System.Text;
using System.Security.Cryptography;
using anagramsolver.containers;
using System.Collections.Generic;

namespace anagramsolver.services
{
    /// <summary>
    /// Methods for computing md5
    /// </summary>
    public class Md5Helper
    {
        private MD5 _md5HashComputer;
        private string[] _md5Hashes;
        public string[] Md5Hashes
        {
            get { return _md5Hashes; }
            set { _md5Hashes = value; }
        }

        public Md5Helper(MD5 Md5HashComputer, IAnagramContainer anagramContainer)
            :this(Md5HashComputer, anagramContainer.Md5Hashes)
        {}
        public Md5Helper(MD5 Md5HashComputer, string[] Md5Hashes)
        {
            _md5HashComputer = Md5HashComputer;
            _md5Hashes = Md5Hashes;
        }

        /// <summary>
        /// Check md5 against _md5Hashes.
        /// Remove hash from list if found
        /// If no hashes left in list return null
        /// </summary>
        /// <param name="input">string to check md5 on</param>
        /// <returns>Did string match any of the hashes?</returns>
        public bool? VerifyMd5HashRemoveHashIfFound(string input)
        {
            bool? result = null;
            if (_md5Hashes.Length > 0)
            {
                // Hash the input.
                string hashOfInput = GetMd5Hash(_md5HashComputer, input);

                // Create a StringComparer an compare the hashes.
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;

                // Check aginst each md5 string 
                for (int i = 0; i < _md5Hashes.Length; i++)
                {
                    result = (0 == comparer.Compare(hashOfInput, _md5Hashes[i]));
                    if (result.HasValue && result.Value == true)
                    {
                        // A correct sentence was found
                        // Remove hash from list, so we won't have to check against it anymore
                        var hashList= new List<string>(_md5Hashes);
                        hashList.RemoveAt(i);
                        _md5Hashes = hashList.ToArray();
                        break;
                    }
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
