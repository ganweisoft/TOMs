// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTCenterCore.RsaEncrypt;

public interface IRSAAlgorithm
{
    string Encrypt(string encryptedText);
    string Decrypt(string decryptedText);
    string GetPublicCipher();
}
