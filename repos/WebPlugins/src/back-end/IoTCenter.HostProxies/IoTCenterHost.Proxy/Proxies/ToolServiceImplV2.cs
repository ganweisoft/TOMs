// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction.Interfaces.Services;
using static IoTCenterHost.Proto.Tool;

namespace IoTCenterHost.Proxy.Proxies
{
    public class ToolServiceImplV2 : IToolService
    {
        private readonly ToolClient _toolClient;
        public ToolServiceImplV2(ToolClient toolClient)
        {
            _toolClient = toolClient;
        }

        public string DecryptOld(string str)
        {
            return _toolClient.Decrypt(new IoTCenterHost.Proto.StringResult { Result = str }).Result;
        }

        public string EncryptOld(string str)
        {
            return _toolClient.Encrypt(new IoTCenterHost.Proto.StringResult { Result = str }).Result;

        }


        public string Decrypt(string str, string purpose = "")
        {
            var inputStringWithSalt = new InputStringWithSalt { PlainText = str, SecurityStamp = purpose }.ToJson();

            var input = _toolClient.DecryptWithSalt(new IoTCenterHost.Proto.StringResult { Result = inputStringWithSalt }).Result;
            return input;
        }

        public string Encrypt(string str, string purpose = "")
        {
            var inputStringWithSalt = new InputStringWithSalt { PlainText = str, SecurityStamp = purpose }.ToJson();
            var input = _toolClient.EncryptWithSalt(new IoTCenterHost.Proto.StringResult { Result = inputStringWithSalt }).Result;
            return input;
        }

        public BatchInputStringWithSalt BatchDecrypt(BatchInputStringWithSalt batchInput)
        {
            if (string.IsNullOrWhiteSpace(batchInput.SecurityStamp)) batchInput.SecurityStamp = "";
            var result = _toolClient.BatchDecryptWithSalt(new IoTCenterHost.Proto.StringResult { Result = batchInput.ToJson() })
                .Result.FromJson<BatchInputStringWithSalt>();
            return result;
        }

        public BatchInputStringWithSalt BatchEncrypt(BatchInputStringWithSalt batchInput)
        {
            if (string.IsNullOrWhiteSpace(batchInput.SecurityStamp)) batchInput.SecurityStamp = "";
            var result = _toolClient.BatchEncryptWithSalt(new IoTCenterHost.Proto.StringResult { Result = batchInput.ToJson() })
                .Result.FromJson<BatchInputStringWithSalt>();
            return result;
        }
    }
    public class InputStringWithSalt
    {
        public string PlainText { get; set; }
        public string SecurityStamp { get; set; }
    }
}
