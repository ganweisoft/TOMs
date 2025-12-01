// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.Extensions.Configuration;

namespace IoTCenterCore.RsaEncrypt;

public class RSAAlgorithm : IRSAAlgorithm
{
    private readonly IConfiguration _configuration;

    public RSAAlgorithm(IConfiguration Configuration)
    {
        _configuration = Configuration;
    }

    public string Decrypt(string decryptedText)
    {
        if (_configuration["WebApi:RSAPadding"] == "oaep")
        {
            return RSAPaddingOaep.Decrypt(decryptedText);
        }
        else if (_configuration["WebApi:RSAPadding"] == "pkcs1")
        {
            return RSAPaddingPkcs1.Decrypt(decryptedText);
        }
        else
        {
            return RSAPaddingPkcs1.Decrypt(decryptedText);
        }
    }

    public string Encrypt(string encryptedText)
    {
        if (_configuration["WebApi:RSAPadding"] == "oaep")
        {
            return RSAPaddingOaep.Encrypt(encryptedText);
        }
        else if (_configuration["WebApi:RSAPadding"] == "pkcs1")
        {
            return RSAPaddingPkcs1.Encrypt(encryptedText);
        }
        else
        {
            return RSAPaddingPkcs1.Encrypt(encryptedText);
        }
    }

    public string GetPublicCipher()
    {
        if (_configuration["WebApi:RSAPadding"] == "oaep")
        {
            return RSAPaddingOaep.PublicKey;
        }
        else if (_configuration["WebApi:RSAPadding"] == "pkcs1")
        {
            return RSAPaddingPkcs1.GetPublicCipher();
        }
        else
        {
            return RSAPaddingPkcs1.GetPublicCipher();
        }
    }
}
