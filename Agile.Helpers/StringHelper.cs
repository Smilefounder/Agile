using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Helpers
{
    public class StringHelper
    {
        /// <summary>
        /// 数字
        /// </summary>
        private static readonly string Numbers = "1234567890";

        /// <summary>
        /// 字母
        /// </summary>
        private static readonly string Letters = "abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// 特殊字符
        /// </summary>
        private static readonly string Symbols = "`~!@#$%^&*()-_=+[{]}\\|;:'\",<.>/?";

        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <param name="stringType"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string GetRandomString(int stringType = 1, int minLength = 1, int maxLength = 10)
        {
            var pattern = GetPatternString(stringType);
            if (String.IsNullOrEmpty(pattern))
            {
                throw new ArgumentException("stringType无效");
            }

            if (maxLength <= 0 || minLength <= 0 || maxLength < minLength)
            {
                throw new ArgumentException("minLength,maxLength取值不正确");
            }

            var length = maxLength;
            if (minLength != maxLength)
            {
                length = RandomHelper.Instance.Next(minLength, maxLength + 1);
            }

            var sb = new StringBuilder();
            for (var i = 1; i <= length; i++)
            {
                var idx = RandomHelper.Instance.Next(0, pattern.Length);
                sb.Append(pattern[idx]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 根据字符串类型获取模板
        /// </summary>
        /// <param name="stringType"></param>
        /// <returns></returns>
        private static string GetPatternString(int stringType)
        {
            var total = 0;
            var values = Enum.GetValues(typeof(StringTypeEnum));
            foreach (var v in values)
            {
                total += Convert.ToInt32(v);
            }

            var pattern = "";
            if (stringType > total)
            {
                pattern = Numbers + Letters + Letters.ToUpper() + Symbols;
            }

            var valueOfSymbol = (int)StringTypeEnum.Symbols;
            if (stringType >= valueOfSymbol)
            {
                stringType = stringType % valueOfSymbol;
                pattern += Symbols;
            }

            var valueOfUpperCaseLetter = (int)StringTypeEnum.UpperCaseLetters;
            if (stringType >= valueOfUpperCaseLetter)
            {
                stringType = stringType % valueOfUpperCaseLetter;
                pattern += Letters.ToUpper();
            }

            var valueOfLowerCaseLetter = (int)StringTypeEnum.LowerCaseLetters;
            if (stringType >= valueOfLowerCaseLetter)
            {
                stringType = stringType % valueOfLowerCaseLetter;
                pattern += Letters;
            }

            var valueOfNumber = (int)StringTypeEnum.Numbers;
            if (stringType >= valueOfNumber)
            {
                stringType = stringType % valueOfNumber;
                pattern += Numbers;
            }

            return pattern;
        }

        /// <summary>
        /// 从UserAgent中获得操作系统名称
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static string GetOsFromUserAgent(string userAgent)
        {
            if (String.IsNullOrEmpty(userAgent))
            {
                return "";
            }

            userAgent = userAgent.ToLower();
            if (userAgent.Contains("windows nt 10.0"))
            {
                return "Windows10";
            }

            if (userAgent.Contains("windows nt 6.3"))
            {
                return "Windows8.1";
            }

            if (userAgent.Contains("windows nt 6.2"))
            {
                return "Windows8";
            }

            if (userAgent.Contains("windows nt 6.1"))
            {
                return "Windows7";
            }

            if (userAgent.Contains("windows nt 6.0"))
            {
                return "Vista";
            }

            if (userAgent.Contains("windows nt 5.2"))
            {
                return "Server2003";
            }

            if (userAgent.Contains("windows nt 5.1"))
            {
                return "XP";
            }

            if (userAgent.Contains("windows nt 5.0"))
            {
                return "Windows2000";
            }

            if (userAgent.Contains("iphone"))
            {
                return "iOS";
            }

            if (userAgent.Contains("ipad"))
            {
                return "iOS";
            }

            if (userAgent.Contains("android"))
            {
                return "Android";
            }

            if (userAgent.Contains("windows phone"))
            {
                return "Windows Phone";
            }

            if (userAgent.Contains("os x"))
            {
                return "OS X";
            }

            if (userAgent.Contains("solaris"))
            {
                return "Solaris";
            }

            if (userAgent.Contains("linux"))
            {
                return "Linux";
            }

            if (userAgent.Contains("unix"))
            {
                return "Unix";
            }

            return "";
        }

        /// <summary>
        /// 从UserAgent中获得设备名称
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static string GetDeviceFromUserAgent(string userAgent)
        {
            if (String.IsNullOrEmpty(userAgent))
            {
                return "";
            }

            userAgent = userAgent.ToLower();
            if (userAgent.Contains("windows nt"))
            {
                return "Desktop";
            }

            if (userAgent.Contains("iphone"))
            {
                return "iPhone";
            }

            if (userAgent.Contains("ipad"))
            {
                return "iPad";
            }

            if (userAgent.Contains("macintosh"))
            {
                return "Mac";
            }

            return "";
        }

        /// <summary>
        /// 是否手机端发起的请求
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static bool IsPhoneRequest(string userAgent)
        {
            if (String.IsNullOrEmpty(userAgent))
            {
                return false;
            }

            userAgent = userAgent.ToLower();
            if (userAgent.Contains("iphone"))
            {
                return true;
            }

            if (userAgent.Contains("android"))
            {
                return true;
            }

            if (userAgent.Contains("windows phone"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取字符串中的数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetNumberFromStr(string str)
        {
            var sb = new StringBuilder();
            foreach (var ch in str)
            {
                if (Char.IsDigit(ch))
                {
                    sb.Append(ch.ToString());
                }
            }

            int tempint;
            int.TryParse(sb.ToString(), out tempint);

            return tempint;
        }
    }

    /// <summary>
    /// 字符类型的枚举
    /// </summary>
    [Flags]
    public enum StringTypeEnum
    {
        /// <summary>
        /// 数字
        /// </summary>
        [Description("数字")]
        Numbers = 1,

        /// <summary>
        /// 小写字母
        /// </summary>
        [Description("小写字母")]
        LowerCaseLetters = 2,

        /// <summary>
        /// 大写字母
        /// </summary>
        [Description("大写字母")]
        UpperCaseLetters = 4,

        /// <summary>
        /// 标点符号
        /// </summary>
        [Description("标点符号")]
        Symbols = 8
    }
}
