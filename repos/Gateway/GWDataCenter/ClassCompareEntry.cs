﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using System;
using System.Text;
namespace GWDataCenter
{
    public class CompareResult
    {
        public bool IsChange { get; set; }
        public string ChangeContent { get; set; }
    }
    public class ClassCompareEntry
    {
        public CompareResult CompareDTO(object AfterDTO, object BeforeDTO)
        {
            CompareResult result = new CompareResult();
            bool b = false;
            if (BeforeDTO == null && AfterDTO != null)
            {
                b = true;
            }
            else if (BeforeDTO != null && AfterDTO == null)
            {
                b = true;
            }
            else if (BeforeDTO.Equals(DBNull.Value) && !AfterDTO.Equals(DBNull.Value))
            {
                b = true;
            }
            else if (!BeforeDTO.Equals(DBNull.Value) && AfterDTO.Equals(DBNull.Value))
            {
                b = true;
            }
            else if (BeforeDTO.GetType() != AfterDTO.GetType())
            {
                result.IsChange = true;
                return result;
            }
            else if (BeforeDTO is int || BeforeDTO is short || BeforeDTO is long || BeforeDTO is float || BeforeDTO is double || BeforeDTO is decimal)
            {
                if (BeforeDTO is int)
                {
                    if (Convert.ToInt32(BeforeDTO) != Convert.ToInt32(AfterDTO))
                    {
                        b = true;
                    }
                }
                else if (BeforeDTO is short)
                {
                    if (Convert.ToInt16(BeforeDTO) != Convert.ToInt16(AfterDTO))
                    {
                        b = true;
                    }
                }
                else if (BeforeDTO is long)
                {
                    if (Convert.ToInt64(BeforeDTO) != Convert.ToInt64(AfterDTO))
                    {
                        b = true;
                    }
                }
                else if (BeforeDTO is float)
                {
                    if (Convert.ToSingle(BeforeDTO) != Convert.ToSingle(AfterDTO))
                    {
                        b = true;
                    }
                }
                else if (BeforeDTO is double)
                {
                    if (Convert.ToDouble(BeforeDTO) != Convert.ToDouble(AfterDTO))
                    {
                        b = true;
                    }
                }
                else if (BeforeDTO is decimal)
                {
                    if (Convert.ToDecimal(BeforeDTO) == Convert.ToDecimal(AfterDTO))
                    {
                        b = true;
                    }
                }
            }
            else
            {
                StringBuilder content = new StringBuilder();
                content.Append(GetTableRowInfo(BeforeDTO));
                var beforeMembers = BeforeDTO.GetType().GetProperties();
                var afterMembers = AfterDTO.GetType().GetProperties();
                for (int i = 0; i < beforeMembers.Length; i++)
                {
                    var beforeVal = beforeMembers[i].GetValue(BeforeDTO, null);
                    var afterVal = afterMembers[i].GetValue(AfterDTO, null);
                    var beforeValue = beforeVal == null ? null : beforeVal.ToString();
                    var afterValue = afterVal == null ? null : afterVal.ToString();
                    if (beforeValue != afterValue)
                    {
                        b = true;
                        content.Append(beforeMembers[i].Name + "(" + beforeValue + "->" + afterValue + ")");
                    }
                }
                result.IsChange = b;
                result.ChangeContent = content.ToString();
            }
            if (result.IsChange)
                GWDataCenter.DataCenter.WriteLogFile(result.ChangeContent, LogType.Config);
            return result;
        }
        string GetTableRowInfo(object obj)
        {
            StringBuilder content = new StringBuilder();
            if (obj.GetType() == typeof(EquipTableRow))
            {
                EquipTableRow row = (EquipTableRow)obj;
                content.Append($"Equip表设备号为[{row.equip_no}],设备名称为[{row.equip_nm}]的修改信息:\n");
            }
            if (obj.GetType() == typeof(YcpTableRow))
            {
                YcpTableRow row = (YcpTableRow)obj;
                content.Append($"Ycp表设备号为[{row.equip_no}],测点号为[{row.yc_no}],测点名称为[{row.yc_nm}]的修改信息:\n");
            }
            if (obj.GetType() == typeof(YxpTableRow))
            {
                YxpTableRow row = (YxpTableRow)obj;
                content.Append($"Yxp表设备号为[{row.equip_no}],测点号为[{row.yx_no}],测点名称为[{row.yx_nm}]的修改信息:\n");
            }
            if (obj.GetType() == typeof(SetParmTableRow))
            {
                SetParmTableRow row = (SetParmTableRow)obj;
                content.Append($"SetParm表设备号为[{row.equip_no}],测点号为[{row.set_no}],设置名称为[{row.set_nm}]的修改信息:\n");
            }
            return content.ToString();
        }
    }
}
