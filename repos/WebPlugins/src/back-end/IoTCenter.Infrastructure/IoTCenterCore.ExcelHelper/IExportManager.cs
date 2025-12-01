// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace IoTCenterCore.ExcelHelper
{
    public interface IExportManager
    {
        void ExportToXlsx<T>(IWorkbook workbook, PropertyByName<T>[] propertyByNames, IEnumerable<T> items, string sheetName, bool xls = true) where T : class;
    }
}
