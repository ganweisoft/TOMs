// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterCore.DeviceDetection
{
    public enum BrowserType
    {
        Unknown = 0,
        InternetExplorer,
        Edge,
        Chrome,
        SoGou,
        _360_,
        FireFox,
        Safari,
        Opera
    }

    public static class BrowserConstant
    {
        public const string InternetExplorer = "Internet Explorer";
        public const string Edge = "Microsoft Edge";
        public const string Chrome = "Chrome";
        public const string FireFox = "FireFox";
        public const string Safari = "Safari";
        public const string Opera = "Opera";
        public const string SoGou = "SoGou";
        public const string _360_ = "360";
        public const string Unknown = "Unknown";
    }

    public static class BrowserTypeExtension
    {
        public static string ToName(this BrowserType browserType)
        {
            return browserType switch
            {
                BrowserType.InternetExplorer => BrowserConstant.InternetExplorer,
                BrowserType.Edge => BrowserConstant.Edge,
                BrowserType.SoGou => BrowserConstant.SoGou,
                BrowserType._360_ => BrowserConstant._360_,
                BrowserType.Chrome => BrowserConstant.Chrome,
                BrowserType.FireFox => BrowserConstant.FireFox,
                BrowserType.Safari => BrowserConstant.Safari,
                BrowserType.Opera => BrowserConstant.Opera,
                _ => BrowserConstant.Unknown
            };
        }
    }
}
