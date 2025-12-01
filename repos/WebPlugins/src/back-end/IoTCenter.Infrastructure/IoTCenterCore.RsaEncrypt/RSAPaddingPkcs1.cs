// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace IoTCenterCore.RsaEncrypt;

#region 创建公钥和私钥
internal static class RsaProviderExetesion
{
    static public void WriteAll(this Stream stream, byte[] byts)
    {
        stream.Write(byts, 0, byts.Length);
    }

    internal static T[] Sub<T>(this T[] arr, int start, int count)
    {
        T[] val = new T[count];
        for (var i = 0; i < count; i++)
        {
            val[i] = arr[start + i];
        }
        return val;
    }
    public static (string publicPem, string privatePem) RSAToPem(bool isPKCS8, int keySize = 2048)
    {
        if (keySize < 2048)
        {
            throw new ArgumentException($" Key size min value is 2048!");
        }

        using RSA rsa = RSA.Create();
        rsa.KeySize = keySize;

        var publicPem = RsaProvider.ToPem(rsa, false, isPKCS8);
        var privatePem = RsaProvider.ToPem(rsa, true, isPKCS8);

        return (publicPem, privatePem);
    }
}

internal class RsaProvider
{
    static private Regex _PEMCode = new Regex(@"--+.+?--+|\s+");
    static private byte[] _SeqOID = new byte[] { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
    static private byte[] _Ver = new byte[] { 0x02, 0x01, 0x00 };

		internal static RSA FromPem(string pem)
    {
        var rsa = RSA.Create();

        var param = new RSAParameters();

        var base64 = _PEMCode.Replace(pem, "");
        var data = Convert.FromBase64String(base64);
        if (data == null)
        {
            throw new Exception("Pem content invalid ");
        }
        var idx = 0;

        Func<byte, int> readLen = (first) =>
        {
            if (data[idx] == first)
            {
                idx++;
                if (data[idx] == 0x81)
                {
                    idx++;
                    return data[idx++];
                }
                else if (data[idx] == 0x82)
                {
                    idx++;
                    return (data[idx++] << 8) + data[idx++];
                }
                else if (data[idx] < 0x80)
                {
                    return data[idx++];
                }
            }
            throw new Exception("Not found any content in pem file");
        };
        Func<byte[]> readBlock = () =>
        {
            var len = readLen(0x02);
            if (data[idx] == 0x00)
            {
                idx++;
                len--;
            }
            var val = data.Sub(idx, len);
            idx += len;
            return val;
        };

        Func<byte[], bool> eq = (byts) =>
        {
            for (var i = 0; i < byts.Length; i++, idx++)
            {
                if (idx >= data.Length)
                {
                    return false;
                }
                if (byts[i] != data[idx])
                {
                    return false;
                }
            }
            return true;
        };

        if (pem.Contains("PUBLIC KEY"))
        {
            /****Use public key****/
            readLen(0x30);
            if (!eq(_SeqOID))
            {
                throw new Exception("Unknown pem format");
            }

            readLen(0x03);
            idx++;
            readLen(0x30);

            param.Modulus = readBlock();

            param.Exponent = readBlock();
        }
        else if (pem.Contains("PRIVATE KEY"))
        {
            /****Use private key****/
            readLen(0x30);

            if (!eq(_Ver))
            {
                throw new Exception("Unknown pem version");
            }

            var idx2 = idx;
            if (eq(_SeqOID))
            {
                readLen(0x04);

                readLen(0x30);

                if (!eq(_Ver))
                {
                    throw new Exception("Pem version invalid");
                }
            }
            else
            {
                idx = idx2;
            }

            param.Modulus = readBlock();
            param.Exponent = readBlock();
            param.D = readBlock();
            param.P = readBlock();
            param.Q = readBlock();
            param.DP = readBlock();
            param.DQ = readBlock();
            param.InverseQ = readBlock();
        }
        else
        {
            throw new Exception("pem need 'BEGIN' and  'END'");
        }

        rsa.ImportParameters(param);
        return rsa;
    }

    internal static string ToPem(RSA rsa, bool includePrivateParameters, bool isPKCS8 = false)
    {
        var ms = new MemoryStream();

        Action<int> writeLenByte = (len) =>
        {
            if (len < 0x80)
            {
                ms.WriteByte((byte)len);
            }
            else if (len <= 0xff)
            {
                ms.WriteByte(0x81);
                ms.WriteByte((byte)len);
            }
            else
            {
                ms.WriteByte(0x82);
                ms.WriteByte((byte)(len >> 8 & 0xff));
                ms.WriteByte((byte)(len & 0xff));
            }
        };
        Action<byte[]> writeBlock = (byts) =>
        {
            var addZero = (byts[0] >> 4) >= 0x8;
            ms.WriteByte(0x02);
            var len = byts.Length + (addZero ? 1 : 0);
            writeLenByte(len);

            if (addZero)
            {
                ms.WriteByte(0x00);
            }
            ms.Write(byts, 0, byts.Length);
        };

        Func<int, byte[], byte[]> writeLen = (index, byts) =>
        {
            var len = byts.Length - index;

            ms.SetLength(0);
            ms.Write(byts, 0, index);
            writeLenByte(len);
            ms.Write(byts, index, len);

            return ms.ToArray();
        };


        if (!includePrivateParameters)
        {
            /****Create public key****/
            var param = rsa.ExportParameters(false);

            ms.WriteByte(0x30);
            var index1 = (int)ms.Length;

            ms.WriteAll(_SeqOID);

            ms.WriteByte(0x03);
            var index2 = (int)ms.Length;
            ms.WriteByte(0x00);

            ms.WriteByte(0x30);
            var index3 = (int)ms.Length;

            writeBlock(param.Modulus);

            writeBlock(param.Exponent);

            var bytes = ms.ToArray();

            bytes = writeLen(index3, bytes);
            bytes = writeLen(index2, bytes);
            bytes = writeLen(index1, bytes);


            return "-----BEGIN PUBLIC KEY-----\n" + TextBreak(Convert.ToBase64String(bytes), 64) + "\n-----END PUBLIC KEY-----";
        }
        else
        {
            /****Create private key****/
            var param = rsa.ExportParameters(true);

            ms.WriteByte(0x30);
            int index1 = (int)ms.Length;

            ms.WriteAll(_Ver);

            int index2 = -1, index3 = -1;
            if (isPKCS8)
            {
                ms.WriteAll(_SeqOID);

                ms.WriteByte(0x04);
                index2 = (int)ms.Length;

                ms.WriteByte(0x30);
                index3 = (int)ms.Length;

                ms.WriteAll(_Ver);
            }

            writeBlock(param.Modulus);
            writeBlock(param.Exponent);
            writeBlock(param.D);
            writeBlock(param.P);
            writeBlock(param.Q);
            writeBlock(param.DP);
            writeBlock(param.DQ);
            writeBlock(param.InverseQ);

            var bytes = ms.ToArray();

            if (index2 != -1)
            {
                bytes = writeLen(index3, bytes);
                bytes = writeLen(index2, bytes);
            }
            bytes = writeLen(index1, bytes);


            var flag = " PRIVATE KEY";
            if (!isPKCS8)
            {
                flag = " RSA" + flag;
            }
            return "-----BEGIN" + flag + "-----\n" + TextBreak(Convert.ToBase64String(bytes), 64) + "\n-----END" + flag + "-----";
        }
    }

		private static string TextBreak(string text, int line)
    {
        var idx = 0;
        var len = text.Length;
        var str = new StringBuilder();
        while (idx < len)
        {
            if (idx > 0)
            {
                str.Append('\n');
            }
            if (idx + line >= len)
            {
                str.Append(text.Substring(idx));
            }
            else
            {
                str.Append(text.Substring(idx, line));
            }
            idx += line;
        }
        return str.ToString();
    }
}
#endregion

#region RsaHelper
public class RSAPaddingPkcs1
{
    private const string PEMDIRECTORY = "pems";
    private const string PRIVATE_PEM = "private_key.pem";
    private const string PUBLIC_PEM = "public_key.pem";
    private static string privateKey;
    private static string publicKey;

    public static void InitRsaPem(bool change = false)
    {
        var pemsDir = Path.Combine(AppContext.BaseDirectory, PEMDIRECTORY);
        if (!Directory.Exists(pemsDir))
        {
            Directory.CreateDirectory(pemsDir);
        }

        var privateFile = Path.Combine(pemsDir, PRIVATE_PEM);
        var publicFile = Path.Combine(pemsDir, PUBLIC_PEM);
        if (change)
        {
            GeneratePems(privateFile, publicFile);
        }
        else if (File.Exists(privateFile) && File.Exists(publicFile))
        {
            privateKey = File.ReadAllText(privateFile)?.Trim();

            publicKey = File.ReadAllText(publicFile)?.Trim();
        }
        else
        {
            GeneratePems(privateFile, publicFile);
        }
    }

    private static void GeneratePems(string privateFile, string publicFile)
    {
        (string publicPem, string privatePem) = RSAToPem(true);

        if (File.Exists(privateFile))
        {
            File.Delete(privateFile);
        }
        using var privateFS = File.Create(privateFile);
        var privateBytes = Encoding.UTF8.GetBytes(publicPem);
        privateFS.Write(privateBytes, 0, privateBytes.Length);

        if (File.Exists(publicFile))
        {
            File.Delete(publicFile);
        }
        using var publicFS = File.Create(publicFile);
        var publicBytes = Encoding.UTF8.GetBytes(publicPem);
        publicFS.Write(publicBytes, 0, publicBytes.Length);

        privateKey = privatePem;

        publicKey = publicPem;
    }

    private static (string publicPem, string privatePem) RSAToPem(bool isPKCS8 = false, int keySize = 2048)
    {
        if (keySize < 2048)
        {
            throw new ArgumentException($"Key size min value is 2048!");
        }

        using var rsa = RSA.Create();
        rsa.KeySize = keySize;

        var publicPem = RsaProvider.ToPem(rsa, false, isPKCS8);
        var privatePem = RsaProvider.ToPem(rsa, true, isPKCS8);

        return (publicPem, privatePem);
    }

    internal static string GetPublicCipher()
    {
        var regex = new Regex(@"--+.+?--+|\s+");
        var base64 = regex.Replace(publicKey, "");
        return WebUtility.UrlEncode(base64);
    }

    #region 解密

    internal static string Decrypt(string cipherText)
    {
        var bytes = Convert.FromBase64String(cipherText);
        var rsa = RsaProvider.FromPem(privateKey);
        return Encoding.UTF8.GetString(rsa.Decrypt(bytes, RSAEncryptionPadding.Pkcs1));
    }

    #endregion

    #region 加密

    internal static string Encrypt(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        var rsa = RsaProvider.FromPem(publicKey);
        var encryptData = rsa.Encrypt(bytes, RSAEncryptionPadding.Pkcs1);
        return Convert.ToBase64String(encryptData);
    }

    #endregion

}
#endregion
