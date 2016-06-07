using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Helpers
{
    public class SecurityHelper
    {
        /// <summary>
        /// 获取MD5加密的字符串(Unicode编码)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5String(string input)
        {
            return GetMD5String(input, Encoding.UTF8);
        }

        /// <summary>
        /// 获取MD5加密的字符串(指定编码)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5String(string input, Encoding enc)
        {
            if (String.IsNullOrEmpty(input))
            {
                return "";
            }

            var md5 = new MD5CryptoServiceProvider();
            var source = enc.GetBytes(input);
            var hashed = md5.ComputeHash(source);
            var sb = new StringBuilder();

            for (int i = 0; i < hashed.Length; i++)
            {
                sb.Append(hashed[i].ToString("x"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取SHA1加密的字符串(Unicode编码)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetSHA1String(string input)
        {
            return GetSHA1String(input, Encoding.UTF8);
        }

        /// <summary>
        /// 获取SHA1加密的字符串(指定编码)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string GetSHA1String(string input, Encoding enc)
        {
            if (String.IsNullOrEmpty(input))
            {
                return "";
            }

            var sha1 = new SHA1CryptoServiceProvider();
            var source = enc.GetBytes(input);
            var hashed = sha1.ComputeHash(source);
            var sb = new StringBuilder();

            for (int i = 0; i < hashed.Length; i++)
            {
                sb.Append(hashed[i].ToString("x"));
            }

            return sb.ToString();
        }
    }
}
