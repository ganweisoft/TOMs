// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Linq;

namespace IoTCenterCore.ExcelHelper
{
    public class PropertyManager<T>
    {
        private readonly Dictionary<string, PropertyByName<T>> _properties;

        public PropertyManager(PropertyByName<T>[] properties)
        {
            _properties = new Dictionary<string, PropertyByName<T>>();

            var poz = 0;
            foreach (var propertyByName in properties)
            {
                if (_properties.ContainsKey(propertyByName.PropertyName))
                {
                    continue;
                }
                propertyByName.PropertyOrderPosition = poz;
                poz++;
                _properties.Add(propertyByName.PropertyName, propertyByName);
            }
        }

        public T CurrentObject { get; set; }

        public int GetIndex(string propertyName)
        {
            if (!_properties.ContainsKey(propertyName))
                return -1;

            return _properties[propertyName].PropertyOrderPosition;
        }

        public object this[string propertyName]
        {
            get
            {
                return _properties.ContainsKey(propertyName) && CurrentObject != null
                    ? _properties[propertyName].GetProperty(CurrentObject)
                    : null;
            }
        }

        public void WriteToXlsx(ISheet workSheet, int row, int cellOffset = 0)
        {
            var cellRow = workSheet.CreateRow(row);

            if (CurrentObject == null)
                return;

            foreach (var prop in _properties.Values)
            {
                var cell = cellRow.CreateCell(prop.PropertyOrderPosition + cellOffset);

                if (prop.IsDropDownCell)
                {
                    cell.SetCellValue(prop.GetItemText(prop.GetProperty(CurrentObject)));
                }
                else
                {
                    object propertyValue = null;

                    if (CurrentObject is IDictionary<string, object> dynamic)
                    {
                        var cloumnName = prop.GetProperty(CurrentObject)?.ToString();

                        if (string.IsNullOrEmpty(cloumnName))
                        {
                            continue;
                        }

                        dynamic.TryGetValue(cloumnName, out var value);

                        propertyValue = value;
                    }
                    else
                    {
                        propertyValue = prop.GetProperty(CurrentObject);
                    }

                    if (propertyValue is null)
                    {
                        cell.SetCellType(CellType.Blank);
                    }
                    else
                    {
                        cell.SetCellType(CellType.String);
                        cell.SetCellValue(propertyValue.ToString());
                    }
                }
            }
        }

        public void ReadFromXlsx(ISheet workSheet, int row, int cellOffset = 0)
        {
            if (workSheet == null || workSheet.GetRow(row) == null)
            {
                return;
            }

            for (var i = 0; i < workSheet.GetRow(0).Cells.Count; i++)
            {
                var cell = workSheet.GetRow(0).Cells[i];

                var cellName = cell?.ToString();

                if (string.IsNullOrEmpty(cellName))
                {
                    continue;
                }

                if (!_properties.ContainsKey(cellName))
                {
                    continue;
                }

                _properties[cellName].PropertyValue = workSheet.GetRow(row).GetCell(i + cellOffset)?.ToString()?.Trim();
            }
        }

        public void WriteCaption(ISheet sheet, int row = 0, int cellOffset = 0)
        {
            var cellRow = sheet.CreateRow(row);

            foreach (var caption in _properties.Values)
            {
                var cell = cellRow.CreateCell(caption.PropertyOrderPosition + cellOffset);
                cell.SetCellValue(caption.PropertyName);
            }
        }

        public int Count
        {
            get { return _properties.Count; }
        }

        public PropertyByName<T> GetProperty(string propertyName)
        {
            return _properties.ContainsKey(propertyName) ? _properties[propertyName] : null;
        }

        public PropertyByName<T>[] GetProperties
        {
            get { return _properties.Values.ToArray(); }
        }


        public void SetSelectList(string propertyName, SelectList list)
        {
            var tempProperty = GetProperty(propertyName);
            if (tempProperty != null)
                tempProperty.DropDownElements = list;
        }

        public bool IsCaption
        {
            get { return _properties.Values.All(p => p.IsCaption); }
        }
    }
}
