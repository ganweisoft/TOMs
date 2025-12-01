//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace IoTCenterHost.Core.Extension
{
    [ExcludeFromCodeCoverage]
    public static class HashHelper
    {
        public static string MD5Encrypt16(this string password)
        {
            var md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(password)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }
        public static string MD5Encrypt32(this string password)
        {
            string cl = password;
            StringBuilder pwd = new StringBuilder();
            MD5 md5 = System.Security.Cryptography.MD5.Create(); //实例化一个md5对像
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            for (int i = 0; i < s.Length; i++)
            {
                pwd.Append(pwd + s[i].ToString("X"));
            }
            return pwd.ToString();
        }
        public static string MD5Encrypt64(this string password)
        {
            string cl = password;
            MD5 md5 = System.Security.Cryptography.MD5.Create(); //实例化一个md5对像
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            return Convert.ToBase64String(s);
        }
        public static string MD5Byte(this byte[] bytes)
        {
            String hashMD5 = String.Empty;
            if (bytes != null)
            {
                System.Security.Cryptography.MD5 calculator = System.Security.Cryptography.MD5.Create();
                Byte[] buffer = calculator.ComputeHash(bytes);
                calculator.Clear();
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < buffer.Length; i++)
                {
                    stringBuilder.Append(buffer[i].ToString("x2"));
                }
                hashMD5 = stringBuilder.ToString();

            }//结束计算
            return hashMD5;
        }

        public static string MD5(this string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(str));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }

        public static string SHA512Hash(this byte[] source)
        {
            SHA512 md5 = SHA512.Create();
            byte[] encryptedBytes = md5.ComputeHash(source);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }

        public static string SHA256Hash(this byte[] source)
        {
            SHA256 md5 = SHA256.Create();
            byte[] encryptedBytes = md5.ComputeHash(source);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }
    }
}
