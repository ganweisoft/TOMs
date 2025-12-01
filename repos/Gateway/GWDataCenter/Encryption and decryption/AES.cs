﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using System;
using System.Security.Cryptography;
using System.Text;
namespace GWDataCenter
{
    public static partial class DataCenter
    {
        public static string MD5Key = "GanweiCloud";
        public static string GeneratAESKey()
        {
            string key = GetEncryptKey4Project().Substring(0, 32);
            return key;
        }
        static string GetIVString()
        {
            return MD5Key + "add for AES";
        }
        public static string AESEncrypt(string encriyptString, string key)
        {
            if (string.IsNullOrEmpty(encriyptString))
                return "";
            try
            {
                using (Aes aes = Aes.Create())
                {
                    string ivString = GetIVString();
                    byte[] iv = Encoding.UTF8.GetBytes(ivString.Substring(0, 16));
                    aes.Key = Encoding.UTF8.GetBytes(key.Substring(0, 32));
                    aes.Mode = CipherMode.ECB;
                    aes.IV = iv;
                    aes.Padding = PaddingMode.PKCS7;
                    ICryptoTransform encryptor = aes.CreateEncryptor();
                    byte[] inputData = Encoding.UTF8.GetBytes(encriyptString);
                    byte[] encryptedData = encryptor.TransformFinalBlock(inputData, 0, inputData.Length);
                    return Convert.ToBase64String(encryptedData);
                }
            }
            catch (Exception e)
            {
                WriteLogFile(e.ToString());
            }
            return "";
        }
        public static string AESDecrypt(string decryptString, string key)
        {
            if (!(bool)DataCenter.brunning)
            {
                WriteLogFile("不能在程序没有运行的情况下调用解密函数!");
                return "";
            }
            if (string.IsNullOrEmpty(decryptString))
            {
                WriteLogFile("不能对空字符调用解密函数!");
                return "";
            }
            try
            {
                using (Aes aes = Aes.Create())
                {
                    string ivString = GetIVString();
                    byte[] iv = Encoding.UTF8.GetBytes(ivString.Substring(0, 16));
                    aes.Key = Encoding.UTF8.GetBytes(key.Substring(0, 32));
                    aes.Mode = CipherMode.ECB;
                    aes.IV = iv;
                    aes.Padding = PaddingMode.PKCS7;
                    ICryptoTransform decryptor = aes.CreateDecryptor();
                    byte[] inputData = Convert.FromBase64String(decryptString);
                    byte[] xBuff = decryptor.TransformFinalBlock(inputData, 0, inputData.Length);
                    return Encoding.UTF8.GetString(xBuff);
                }
            }
            catch (Exception e)
            {
                WriteLogFile(e.ToString());
            }
            return "";
        }
        public static string EncodeBase64(Encoding encode, string source)
        {
            byte[] bytes = encode.GetBytes(source);
            string result;
            try
            {
                result = Convert.ToBase64String(bytes);
            }
            catch
            {
                result = source;
            }
            return result;
        }
        public static string EncodeBase64(string source)
        {
            return EncodeBase64(Encoding.UTF8, source);
        }
        public static string DecodeBase64(Encoding encode, string result)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encode.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }
        public static string DecodeBase64(string result)
        {
            return DecodeBase64(Encoding.UTF8, result);
        }
        public static string GetSHA512HashFromString(string strData)
        {
            byte[] bytValue = Encoding.UTF8.GetBytes(strData);
            try
            {
                using (SHA512 sha512 = SHA512.Create())
                {
                    byte[] retVal = sha512.ComputeHash(bytValue);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }
                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                WriteLogFile(ex.ToString());
                return null;
            }
        }
        public static string GetEncryptKey4Project()
        {
            string msg = "https://www.ganweicloud.com";
            string Key = msg.PadRight(32, '#');
            return DataCenter.GetSHA512HashFromString(Key).PadRight(32, '#');
        }
    }
}
