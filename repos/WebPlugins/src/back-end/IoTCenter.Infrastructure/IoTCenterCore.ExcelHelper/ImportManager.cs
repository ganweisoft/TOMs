// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IoTCenterCore.ExcelHelper
{
    public class ImportManager : IImportManager
    {
        public IWorkbook GetWorkbook(string fileName, Stream stream)
        {
            var fileExtension = Path.GetExtension(fileName).ToLower();

            try
            {
                if (fileExtension.Equals(".xls", StringComparison.OrdinalIgnoreCase))
                {
                    return new HSSFWorkbook(stream);
                }
                else
                {
                    return new XSSFWorkbook(stream);
                }
            }
            catch (FormatException)
            {
                throw new FormatException("请勿修改Excel扩展名格式");
            }
        }

        public List<List<object>> ImportFromXlsx(string fileName, Stream stream, params Type[] types)
        {
            var list = new List<List<object>>(100);

            var workBook = GetWorkbook(fileName, stream);

            if (types.Length <= 0)
            {
                throw new ArgumentNullException(nameof(types));
            }

            if (types.Length != workBook.NumberOfSheets)
            {
                return list;
            }

            for (int i = 0; i < types.Length; i++)
            {
                var worksheet = workBook.GetSheetAt(i);

                var t = Activator.CreateInstance(types[i]);

                var obj = GetSheet(t, worksheet);

                if (obj != null)
                {
                    list.Add(obj);
                }
            }

            return list;

        }

        List<T> GetSheet<T>(T t, ISheet worksheet) where T : class
        {
            var list = new List<T>();

            if (worksheet == null)
            {
                return list;
            }

            var properties = GetPropertiesByExcelCells<T>(worksheet);

            if (properties is null || properties.Count <= 0)
            {
                return list;
            }

            if ((worksheet.FirstRowNum + 1) > worksheet.LastRowNum)
            {
                return list;
            }

            var propertyByNames = new List<PropertyByName<T>>();

            foreach (var propertyInfo in t.GetType().GetProperties())
            {
                var attributes = propertyInfo.GetCustomAttributes(typeof(FieldAttribute));

                if (!attributes.Any())
                {
                    continue;
                }

                if (!(attributes.FirstOrDefault() is FieldAttribute fieldAttribute))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(fieldAttribute.Name) || fieldAttribute.Ignore)
                {
                    continue;
                }

                var propertyName = new PropertyByName<T>(fieldAttribute.Name, d => propertyInfo.Name);

                propertyByNames.Add(propertyName);
            }

            var manager = new PropertyManager<T>(propertyByNames.ToArray());

            for (var iRow = worksheet.FirstRowNum + 1; iRow <= worksheet.LastRowNum; iRow++)
            {
                t = (T)Activator.CreateInstance(t.GetType());
                var type = t.GetType();

                manager.ReadFromXlsx(worksheet, iRow);

                foreach (var property in manager.GetProperties)
                {
                    var propertyInfo = type.GetProperties()
                        .FirstOrDefault(d => ((d.GetCustomAttributes(typeof(FieldAttribute))?.FirstOrDefault()) as FieldAttribute)?.Name == property.PropertyName);

                    if (propertyInfo is null)
                    {
                        continue;
                    }

                    var propertyType = propertyInfo.PropertyType;
                    var typeCode = Type.GetTypeCode(propertyType);

                    switch (typeCode)
                    {
                        case TypeCode.Boolean:
                            propertyInfo.SetValue(t, property.BooleanValue);
                            break;
                        case TypeCode.DateTime:
                            propertyInfo.SetValue(t, property.DateTimeValue);
                            break;
                        case TypeCode.Decimal:
                            propertyInfo.SetValue(t, property.DecimalValue);
                            break;
                        case TypeCode.Double:
                            propertyInfo.SetValue(t, property.DoubleValue);
                            break;
                        case TypeCode.Int32:
                            propertyInfo.SetValue(t, property.IntValue);
                            break;
                        case TypeCode.Int64:
                            propertyInfo.SetValue(t, property.Int64Value);
                            break;
                        case TypeCode.String:
                            propertyInfo.SetValue(t, property.StringValue);
                            break;
                        default:
                            if (propertyType == typeof(int?))
                            {
                                propertyInfo.SetValue(t, property.IntValueNullable);
                            }
                            else if (propertyType == typeof(long?))
                            {
                                propertyInfo.SetValue(t, property.LongValueNullable);
                            }
                            else if (propertyType == typeof(decimal?))
                            {
                                propertyInfo.SetValue(t, property.DecimalValueNullable);
                            }
                            else if (propertyType == typeof(DateTime?))
                            {
                                propertyInfo.SetValue(t, property.DateTimeNullable);
                            }
                            break;
                    }
                }

                list.Add(t);
            }

            return list;
        }

        private IList<PropertyByName<T>> GetPropertiesByExcelCells<T>(ISheet workSheet) where T : class
        {
            var properties = new List<PropertyByName<T>>();

            var poz = 0;

            while (true)
            {
                try
                {
                    var cell = workSheet.GetRow(0).GetCell(poz);

                    if (cell == null || string.IsNullOrEmpty(cell.ToString()))
                        break;

                    poz += 1;

                    properties.Add(new PropertyByName<T>(cell.ToString()));
                }
                catch
                {
                    break;
                }
            }

            return properties;
        }
    }
}
