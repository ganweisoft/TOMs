//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace IoTCenterHost.Core.Abstraction.Interfaces.Services
{
    public class BatchInputStringWithSalt
    {
        public List<InputStringItem> PlainTexts { get; set; }

        public string SecurityStamp { get; set; }
    }
}
