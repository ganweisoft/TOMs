// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.IO;

namespace IoTCenterCore.ExcelHelper
{
    public interface IImportManager
    {
        List<List<object>> ImportFromXlsx(string fileName, Stream stream, params Type[] types);
    }
}
