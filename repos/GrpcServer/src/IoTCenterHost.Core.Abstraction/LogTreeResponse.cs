//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;

namespace IoTCenterHost.Core.Abstraction
{
    public class LogTreeResponse
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public long Size { get; set; }
        public DateTime ModifyTime { get; set; }
        public bool IsDirectory { get; set; }
        public List<LogTreeResponse> Childs { get; set; }
    }
}
