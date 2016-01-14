using System;
using System.Security.Cryptography;
using System.Text;

namespace LuniaAssembly
{
    public class Encrypter
    {
        public static string GetHashed(string TextToHash)
        {
            if ((TextToHash == null) || (TextToHash.Length == 0))
            {
                return string.Empty;
            }

            TextToHash += int.MaxValue;
          
            MD5 md5 = new MD5CryptoServiceProvider();
            SHA1 sha1 = new SHA1CryptoServiceProvider();

            byte[] textToHash = Encoding.Default.GetBytes(TextToHash);
            byte[] md5Result = md5.ComputeHash(textToHash);

            byte[] hashedResult = sha1.ComputeHash(md5Result);

            return System.BitConverter.ToString(hashedResult).Replace("-", "").ToLower();
        }
    }
}