// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace IoTCenter.Utilities
{
    public static class CommonUtil
    {
        private static readonly Random random = new Random();

        private static T Random<T>(this T[] values)
        {
            return values[random.Next(0, values.Length)];
        }

        private static void Randomize<T>(this T[] values)
        {
            var numItems = values.Length;

            for (var i = 0; i < numItems - 1; i++)
            {
                var j = random.Next(i, numItems);
                T temp = values[i];
                values[i] = values[j];
                values[j] = temp;
            }
        }


        public static string RandomPassword(
            int minLength = 8, int maxLength = 16,
            bool allowLowercase = true, bool requireLowercase = true,
            bool allowUppercase = true, bool requireUppercase = true,
            bool allowDigit = true, bool requireDigit = true,
            bool allowSpecial = true, bool requireSpecial = true,
            bool allowOther = true, bool requireOther = true, string other = "")
        {
            if (minLength > maxLength)
            {
                throw new Exception($"{nameof(minLength)}大于{nameof(maxLength)}");
            }

            const string lowers = "abcdefghijklmnopqrstuvwxyz";
            const string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string special = @"~!@#$%^&*():;[]{}<>,.?/\|";

            var allowed = string.Empty;

            if (allowLowercase)
            {
                allowed = $"{allowed}{lowers}";
            }

            if (allowUppercase)
            {
                allowed = $"{allowed}{uppers}";
            }

            if (allowDigit)
            {
                allowed = $"{allowed}{digits}";
            }

            if (allowSpecial)
            {
                allowed = $"{allowed}{special}";
            }

            if (allowOther)
            {
                allowed = $"{allowed}{other}";
            }

            var passwordLength = random.Next(minLength, maxLength + 1);

            var password = string.Empty;

            if (requireLowercase)
            {
                password = $"{password}{lowers.ToCharArray().Random()}";
            }

            if (requireUppercase)
            {
                password = $"{password}{uppers.ToCharArray().Random()}";
            }

            if (requireDigit)
            {
                password = $"{password}{digits.ToCharArray().Random()}";
            }

            if (requireSpecial)
            {
                password = $"{password}{special.ToCharArray().Random()}";
            }

            if (requireOther && !string.IsNullOrEmpty(other))
            {
                password = $"{password}{other.ToCharArray().Random()}";
            }

            while (password.Length < passwordLength)
            {
                password = $"{password}{allowed.ToCharArray().Random()}";
            }

            var chars = password.ToCharArray();
            chars.Randomize();

            return new string(chars);
        }

        public static string ToJson(this object obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(obj, settings);
        }

        public static string GetPropertiesPath()
        {
            var parentPath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.FullName;

            var propertiesPath = Path.Combine(parentPath, "data", "AlarmCenter", "AlarmCenterProperties.xml");

            var xpath = FileUtils.NormalizePathDiagonalBar(propertiesPath);

            return xpath;
        }

        public static string GetAesKeyFileContent()
        {
            var parentPath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.FullName;
            var keyPath = FileUtils.NormalizePathDiagonalBar(Path.Combine(parentPath, "bin/Keys.txt"));
            if (!File.Exists(keyPath))
            {
                return string.Empty;
            }

            return File.ReadAllText(keyPath);
        }

        public static long ToLong(this DateTime date)
        {
            var startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);
            return (new DateTimeOffset(date).UtcTicks - startTime.Ticks) / 10000;
        }

        public static DateTime ConvertToDateTime(long timestamp)
        {
            var startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return startTime.Add(new TimeSpan(timestamp * 10000));
        }

        #region 手机号码正则

        public const string PhoneRegex = @"^1(3[0-9]|4[57]|5[0-35-9]|8[0-9]|7[0-9])\d{8}$";

        public static bool IsPhone(string phoneNumber)
        {
            return !string.IsNullOrEmpty(phoneNumber) && Regex.IsMatch(phoneNumber, PhoneRegex);
        }
        #endregion

        #region 电子邮箱正则

        private const string MailRegex = @"^([a-zA-Z0-9_.-])+@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,5})+";

        public static bool IsMail(string mail)
        {
            return !string.IsNullOrEmpty(mail) && Regex.IsMatch(mail, MailRegex, RegexOptions.IgnoreCase);
        }
        #endregion
    }
}
