// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Linq;

namespace IoTCenterCore.ExcelHelper
{
    public class ExportManager : IExportManager
    {
        public void ExportToXlsx<T>(IWorkbook workbook, PropertyByName<T>[] propertyByNames, IEnumerable<T> items, string sheetName, bool xls) where T : class
        {
            var workSheet = workbook.CreateSheet(sheetName);

            var manager = new PropertyManager<T>(propertyByNames.Where(p => !p.Ignore).ToArray());

            manager.WriteCaption(workSheet);

            var row = 1;

            foreach (var item in items)
            {
                manager.CurrentObject = item;

                manager.WriteToXlsx(workSheet, row++);
            }
        }
    }
}
