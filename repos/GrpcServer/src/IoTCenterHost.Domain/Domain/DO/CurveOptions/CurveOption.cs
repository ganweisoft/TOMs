//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Extension;
using System.Reflection;

namespace IoTCenterHost.AppServices.Domain.CurveOptions;

public class CurveOption
{
    public bool CurveStoreInDb { get; set; }

    public int CurveStoreDays { get; set; }

    public string CurveStorePath { get; set; }

    public bool CurveCompress { get; set; }

    public bool EventCompress { get; set; }

    public int EventStoreDays { get; set; }

    public CurveCompressType CurveCompressType { get; set; }

    public static CurveOption Create()
    {
        var directoryInfo = new DirectoryInfo(Assembly.GetEntryAssembly().Location).Parent.Parent.FullName;

        string configurationXml =
            Path.Combine(directoryInfo, "data", "AlarmCenter", "AlarmCenterProperties.xml");

        var curveStoreStr =
            configurationXml.ReadXml("AlarmCenter.Gui.OptionPanels.CurveOptions", "CurveStoreInDB");
        var curveParse = bool.TryParse(curveStoreStr, out var curveStoreValue);
        if (!curveParse)
        {
        }

        var curveStoreDayStr = configurationXml.ReadXml("AlarmCenter.Gui.OptionPanels.CurveOptions",
            "History_CurveStoreTime");
        var curveStoreParse = int.TryParse(curveStoreDayStr, out var curveStoreDayValue);
        if (!curveStoreParse)
        {
        }

        var curveStorePath = configurationXml.ReadXml("AlarmCenter.Gui.OptionPanels.CurveOptions",
            "Hostory_CurveStorePath");

        var curveCompress = configurationXml.ReadXml("AlarmCenter.Gui.OptionPanels.CurveOptions",
            "CurveCompress");
        var curveCompressParse = bool.TryParse(curveCompress, out var curveCompressParseValue);
        if (!curveCompressParse)
        {
        }

        var curveCompressType = configurationXml.ReadXml("AlarmCenter.Gui.OptionPanels.CurveOptions",
            "CurveCompressType");
        var curveCompressTypeParse = Enum.TryParse<CurveCompressType>(curveCompressType, out var curveCompressTypeParseValue);
        if (!curveCompressTypeParse)
        {
        }

        var evtStoreStr =
            configurationXml.ReadXml("AlarmCenter.Gui.OptionPanels.CurveOptions", "Compress");
        var evtStoreParse = bool.TryParse(evtStoreStr, out var evtStoreValue);
        if (!evtStoreParse)
        {
        }

        var evtStoreDayStr = configurationXml.ReadXml("AlarmCenter.Gui.OptionPanels.CurveOptions",
            "History_StoreTime");
        var evtStoreDayParse = int.TryParse(evtStoreDayStr, out var evtStoreDayValue);
        if (!evtStoreDayParse)
        {
        }

        return new CurveOption
        {
            CurveStoreInDb = curveParse && curveStoreValue,
            CurveStoreDays = curveStoreParse ? curveStoreDayValue : 365,
            CurveStorePath = curveStorePath,
            CurveCompress = !curveCompressParse || curveCompressParseValue,
            CurveCompressType = curveCompressTypeParse ? curveCompressTypeParseValue : CurveCompressType.Fastest,
            EventCompress = !evtStoreParse || evtStoreValue,
            EventStoreDays = evtStoreDayParse ? evtStoreDayValue : 365
        };
    }

    public void Save()
    {
        GWDataCenter.DataCenter.SetPropertyToPropertyService(
            "AlarmCenter.Gui.OptionPanels.CurveOptions", "CurveStoreInDB", this.CurveStoreInDb.ToString());

        GWDataCenter.DataCenter.SetPropertyToPropertyService("AlarmCenter.Gui.OptionPanels.CurveOptions",
            "History_CurveStoreTime",
            this.CurveStoreDays.ToString());

        GWDataCenter.DataCenter.SetPropertyToPropertyService(
            "AlarmCenter.Gui.OptionPanels.CurveOptions", "Hostory_CurveStorePath", this.CurveStorePath);

        GWDataCenter.DataCenter.SetPropertyToPropertyService(
            "AlarmCenter.Gui.OptionPanels.CurveOptions", "CurveCompress", this.CurveCompress.ToString());

        GWDataCenter.DataCenter.SetPropertyToPropertyService(
            "AlarmCenter.Gui.OptionPanels.CurveOptions", "CurveCompressType", Enum.GetName(typeof(CurveCompressType), this.CurveCompressType));

        GWDataCenter.DataCenter.SetPropertyToPropertyService(
            "AlarmCenter.Gui.OptionPanels.CurveOptions", "Compress", this.EventCompress.ToString());

        GWDataCenter.DataCenter.SetPropertyToPropertyService(
            "AlarmCenter.Gui.OptionPanels.CurveOptions", "History_StoreTime", this.EventStoreDays.ToString());

    }
}

public class CurveOptionCompare : IEqualityComparer<CurveOption>
{
    public bool Equals(CurveOption x, CurveOption y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.CurveStoreInDb == y.CurveStoreInDb &&
               x.CurveStoreDays == y.CurveStoreDays &&
               x.CurveStorePath == y.CurveStorePath &&
               x.CurveCompress == y.CurveCompress &&
               x.EventCompress == y.EventCompress &&
               x.EventStoreDays == y.EventStoreDays &&
               x.CurveCompressType == y.CurveCompressType;
    }

    public int GetHashCode(CurveOption obj)
    {
        return HashCode.Combine(obj.CurveStoreInDb, obj.CurveStoreDays, obj.CurveStorePath, obj.CurveCompress,
            (int)obj.CurveCompressType, obj.EventCompress, obj.EventStoreDays);
    }
}

public enum CurveCompressType
{
    Fastest,
    SmallestSize
}