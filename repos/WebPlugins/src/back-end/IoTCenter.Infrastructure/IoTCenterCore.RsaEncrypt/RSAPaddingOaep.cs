// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Security.Cryptography;
using System.Text;

namespace IoTCenterCore.RsaEncrypt;

internal static class RSAPaddingOaep
{
    internal static string PublicKey { get; set; }
    internal static RSA RSA { get; set; }

    static RSAPaddingOaep()
    {
        RSA = RSA.Create();

        var publicKeyParameters = RSA.ExportParameters(false);

        PublicKey = WebEncoders.Base64UrlEncode(publicKeyParameters.Modulus);
    }
    internal static string Decrypt(string decryptedText)
    {
        var decryptBytes = RSA.Decrypt(Convert.FromBase64String(decryptedText), RSAEncryptionPadding.OaepSHA256);

        return Encoding.UTF8.GetString(decryptBytes);
    }

    internal static string Encrypt(string encryptedText)
    {
        var encryptBytes = RSA.Encrypt(Encoding.UTF8.GetBytes(encryptedText), RSAEncryptionPadding.OaepSHA256);

        return Convert.ToBase64String(encryptBytes);
    }
}
