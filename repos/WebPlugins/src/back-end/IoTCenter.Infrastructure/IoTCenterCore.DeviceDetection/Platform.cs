// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterCore.DeviceDetection
{
    public enum PlatformType
    {
        Unknown = 0,
        Windows,
        MacOS,
        ChromeOS,
        Ubuntu,
        MintLinux,
        Fedora,
        Gentoo,
        FreeBSD,
        OpenBSD,
        NetBSD,
        Linux,
        iPad,
        iOS,
        Android,
        WindowsPhone
    }

    public static class PlatformConstant
    {
        public const string WindowsXP = "Windows XP";
        public const string WindowsVista = "Windows Vista";
        public const string Windows7 = "Windows 7";
        public const string Windows8 = "Windows 8";
        public const string Windows8_1 = "Windows 8.1";
        public const string Windows10 = "Windows 10";
        public const string WindowsPhone = "Windows Phone";
        public const string Windows = "Windows";

        public const string MacOS10_5 = "Mac OS X v10.5 Leopard";
        public const string MacOS10_6 = "Mac OS X v10.6 Snow Leopard";
        public const string MacOS10_7 = "Mac OS X v10.7 Lion";
        public const string MacOS10_8 = "OS X v10.8 Mountain Lion";
        public const string MacOS10_9 = "OS X v10.9 Mavericks";
        public const string MacOS10_10 = "OS X v10.10 Yosemite";
        public const string MacOS10_11 = "OS X v10.11 El Capitan";
        public const string MacOS10_12 = "macOS v10.12 Sierra";
        public const string MacOS10_13 = "macOS v10.13 High Sierra";
        public const string MacOS10_14 = "macOS v10.14 Mojave";
        public const string MacOS10_15 = "macOS v10.15 Catalina";
        public const string MacOS11_7 = "macOS v11.7 Big Sur";
        public const string MacOS12_6 = "macOS v12.6 Monterey";
        public const string MacOS = "Mac OS";

        public const string ChromeOS = "Chrome OS";
        public const string Ubuntu = "Ubuntu";
        public const string MintLinux = "Mint Linux";
        public const string Fedora = "Fedora";
        public const string Gentoo = "Gentoo";
        public const string FreeBSD = "FreeBSD";
        public const string OpenBSD = "OpenBSD";
        public const string NetBSD = "NetBSD";
        public const string Linux = "Linux";

        public const string iOS = "iOS";
        public const string iPad = "iPad";
        public const string Android = "Android";

        public const string Unknown = "Unknown";
    }

    public class DeviceInfo
    {
        public PlatformType? PlatformType { get; set; }
        public string BrowserDetail { get; set; }
        public BrowserType? BrowserType { get; set; }
    }
}
