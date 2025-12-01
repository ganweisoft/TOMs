// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace IoTCenter.Utilities.Extensions
{

    public static class StringExtensions
    {
        private static readonly object _lock = new();

        public static bool IsEmpty(this string inputString)
        {
            return string.IsNullOrWhiteSpace(inputString);
        }

        public static bool HasValue(this string inputString)
        {
            return !IsEmpty(inputString);
        }

        public static IEnumerable<string> SplitString(this string inputString, string separator = ",")
        {
            if (inputString.IsEmpty())
            {
                return Enumerable.Empty<string>();
            }
            if (separator.IsEmpty())
            {
                separator = ",";
            }
            return inputString
              .Trim()
              .Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public static IEnumerable<string> SplitStringWithDistinct(this string inputString, string separator = ",")
        {
            var stringArray = SplitString(inputString, separator);
            if (stringArray.IsEmpty())
            {
                return Enumerable.Empty<string>();
            }
            return stringArray.Distinct();
        }

        public static IEnumerable<int> SplitStringToIntArray(this string inputString, string separator = ",")
        {
            var stringArray = SplitString(inputString, separator);
            if (stringArray.IsEmpty())
            {
                return Enumerable.Empty<int>();
            }
            var intValues = new List<int>();
            if (stringArray.HasValues())
            {
                foreach (string value in stringArray)
                {
                    if (int.TryParse(value, out int intValue))
                    {
                        intValues.Add(intValue);
                    }
                }
            }
            return intValues;
        }

        public static int ToInt(this string value)
        {
            if (value.IsEmpty())
            {
                return 0;
            }
            value = value.Trim();
            if (int.TryParse(value, out int _value))
            {
                return _value;
            }
            return 0;
        }

        public static decimal ToDecimal(this string value, int decimals = 0)
        {
            if (value.IsEmpty())
            {
                return 0M;
            }
            value = value.Trim();
            if (decimal.TryParse(value, out decimal _value))
            {
                if (decimals == 0)
                {
                    return _value;
                }
                return _value.ToDecimals(decimals);
            }
            else
            {
                return 0M;
            }
        }

        public static DateTime? ToDateTime(this string value)
        {
            if (value.IsEmpty())
            {
                return null;
            }
            if (DateTime.TryParse(value, out DateTime _value))
            {
                return _value;
            }

            return null;
        }


        public static double? ToDouble(this string value, int decimals = 0)
        {
            if (value.IsEmpty())
            {
                return null;
            }
            value = value.Trim();
            if (double.TryParse(value, out double _value))
            {
                if (decimals == 0)
                {
                    return _value;
                }
                return _value.ToDecimals(decimals);
            }
            else
            {
                return null;
            }
        }
        static double ToDecimals(this double value, int decimals = 0)
        {
            if (value == 0)
            {
                return 0d;
            }
            return Math.Round(value, decimals, MidpointRounding.AwayFromZero);
        }
        static decimal ToDecimals(this decimal value, int decimals = 0)
        {
            if (value == 0)
            {
                return 0m;
            }
            return Math.Round(value, decimals, MidpointRounding.AwayFromZero);
        }

        public static string ComputeHash(this string plaintext)
        {
            if (plaintext.IsEmpty())
            {
                return string.Empty;
            }

            lock (_lock)
            {
                var hash = Encoding.UTF8.GetBytes(plaintext);
                using var md5 = MD5.Create();
                hash = md5.ComputeHash(hash);
                var sb = new StringBuilder();
                foreach (var x in hash)
                {
                    sb.Append(x.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static string EnsureTrailingSlash(this string url)
        {
            if (!url.EndsWith("/"))
            {
                return url + "/";
            }

            return url;
        }

        public static string ToWrap(this string value, int length, string separator)
        {
            if (value.IsEmpty())
            {
                return value;
            }

            var valueLength = value.Length;
            if (valueLength <= length)
            {
                return value;
            }

            var results = value.SplitByLength(length);

            return string.Join(separator, results);
        }

        public static ICollection<string> SplitByLength(this string value, int length)
        {
            if (value.IsEmpty())
            {
                return new[] { string.Empty };
            }

            var valueLength = value.Length;
            if (valueLength <= length)
            {
                return new[] { value };
            }

            var times = valueLength / length;

            var results = new List<string>();

            var startIndex = 0;
            for (int i = 0; i < times; i++)
            {
                startIndex = i * length;
                results.Add(value.Substring(startIndex, length));
            }

            if (valueLength % length != 0)
            {
                results.Add(value.Substring(times * length));
            }


            return results;
        }

        public static string AddPlaceholder(this string value, int startIndex = 6, int count = 4, char specialChar = '*')
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            if (startIndex < 0)
            {
                return value;
            }

            if (count <= 0)
            {
                return value;
            }

            if (string.IsNullOrEmpty(specialChar.ToString()))
            {
                return value;
            }

            int lenth = value.Length - startIndex - count;
            if (lenth <= 0)
            {
                return value;
            }

            var specialStringBuilder = new StringBuilder();
            for (int i = count; i > 0; i--)
            {
                specialStringBuilder.Append(specialChar);
            }

            var specialString = specialStringBuilder.ToString();

            value = value.Remove(startIndex, count);
            value = value.Insert(startIndex, specialString);

            return value;
        }

        public static string UrlDecode(this string str)
        {
            return HttpUtility.UrlDecode(str);
        }
        public static string UrlEncode(this string str)
        {
            return HttpUtility.UrlEncode(str);
        }
        public static string ToByte64String(this string result)
        {
            return Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(result));
        }
        public static string FromByte64String(this string result)
        {
            return System.Text.Encoding.Default.GetString(Convert.FromBase64String(result));
        }
        public static SecureString ToSecretString(this string strInput)
        {
            SecureString secureString = new SecureString();
            for (int i = 0; i < strInput.Length; i++)
            {
                secureString.AppendChar(strInput[i]);
            }
            return secureString;
        }
        public static byte[] GetUTF8Bytes(this string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static string GetStringFromUTF8Bytes(this byte[] inputBytes)
        {
            return System.Text.Encoding.UTF8.GetString(inputBytes);
        }
        public static string Base64UrlEncode(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes).Replace("_", "/").Replace("-", "+");
        }

        public static byte[] Base64UrlDecode(this string base64Url)
        {
            return Convert.FromBase64String(base64Url.Replace("_", "/").Replace("-", "+"));
        }
    }
}
