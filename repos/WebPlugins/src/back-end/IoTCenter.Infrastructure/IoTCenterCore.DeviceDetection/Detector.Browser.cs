// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.Net.Http.Headers;
using System;
using System.Diagnostics.CodeAnalysis;

namespace IoTCenterCore.DeviceDetection
{
    public static partial class Detector
    {
        public static bool IsPC(PlatformType platformType)
        {
            return platformType == PlatformType.Windows ||
                platformType == PlatformType.Linux ||
                platformType == PlatformType.MacOS ||
                platformType == PlatformType.ChromeOS;
        }

        public static bool IsMobile(PlatformType platformType)
        {
            return platformType == PlatformType.Android ||
                platformType == PlatformType.iOS ||
                 platformType == PlatformType.iPad;
        }

        public static DeviceInfo TryParseUserAgent(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return null;
            }

            if (!userAgent.TryDetectBrowser(out BrowserType browserType, out string browserDetail))
            {
                return null;
            }

            if (!userAgent.TryDetectPlatform(out PlatformType platformType, out string platformDetail))
            {
                return null;
            }

            return new DeviceInfo()
            {
                PlatformType = platformType,
                BrowserType = browserType,
                BrowserDetail = browserDetail
            };
        }

        private static bool TryDetectBrowser(this string userAgent, out BrowserType browserType, [NotNullWhen(true)] out string? browserDetail)
        {
            if (userAgent.TryDetectBrowser(out browserType, out int? version))
            {
                if (version is int nonNullVersion)
                {
                    browserDetail = $"{browserType.ToName()} {nonNullVersion}";
                }
                else
                {
                    browserDetail = browserType.ToName();
                }
                return true;
            }

            browserType = default;
            browserDetail = default;
            return false;
        }

        private static bool TryDetectBrowser(this string userAgent, out BrowserType browserType, out int? version)
        {
            if (userAgent.TryDetectInternetExplore(out browserType, out version))
            {
                return true;
            }

            if (userAgent.TryDetectEdge(out browserType, out version))
            {
                return true;
            }

            if (userAgent.TryDetectSoGou(out browserType, out version))
            {
                return true;
            }

            if (userAgent.TryDetect360(out browserType, out version))
            {
                return true;
            }

            if (userAgent.TryDetectChrome(out browserType, out version))
            {
                return true;
            }

            if (userAgent.TryDetectFireFox(out browserType, out version))
            {
                return true;
            }

            if (userAgent.TryDetectOpera(out browserType, out version))
            {
                return true;
            }

            if (userAgent.TryDetectSafari(out browserType, out version))
            {
                return true;
            }

            browserType = default;
            version = default;
            return false;
        }

        private static bool TryDetectInternetExplore(this string userAgent, out BrowserType browserType, out int? version)
        {
            if (userAgent.Contains("MSIE"))
            {
                browserType = BrowserType.InternetExplorer;
                userAgent.SearchBrowserVersion("MSIE", 5, out version);
                return true;
            }
            else if (userAgent.Contains("Trident/"))
            {
                browserType = BrowserType.InternetExplorer;

                if (userAgent.Contains("Trident/7"))
                {
                    version = 11;
                }
                else
                {
                    version = default;
                }

                return true;
            }

            browserType = default;
            version = default;
            return false;
        }

        private static bool TryDetectSoGou(this string userAgent, out BrowserType browserType, out int? version)
        {
            if (userAgent.Contains("SE") && userAgent.Contains("MetaSr"))
            {
                browserType = BrowserType.SoGou;
                userAgent.SearchBrowserVersion("SE ", 3, out version);
                return true;
            }

            browserType = default;
            version = default;
            return false;
        }

        private static bool TryDetect360(this string userAgent, out BrowserType browserType, out int? version)
        {
            if (userAgent.Contains("360SE"))
            {
                browserType = BrowserType.SoGou;
                userAgent.SearchBrowserVersion("360SE/", 4, out version);
                return true;
            }

            browserType = default;
            version = default;
            return false;
        }

        private static bool TryDetectEdge(this string userAgent, out BrowserType browserType, out int? version)
        {
            if (userAgent.Contains("Edg/"))
            {
                browserType = BrowserType.Edge;
                userAgent.SearchBrowserVersion("Edg/", 5, out version);
                return true;
            }

            browserType = default;
            version = default;
            return false;
        }

        private static bool TryDetectChrome(this string userAgent, out BrowserType browserType, out int? version)
        {
            if (userAgent.Contains("Chrome/"))
            {
                browserType = BrowserType.Chrome;
                userAgent.SearchBrowserVersion("Chrome/", 5, out version);
                return true;
            }

            if (userAgent.Contains("CriOS/"))
            {
                browserType = BrowserType.Chrome;
                userAgent.SearchBrowserVersion("CriOS/", 5, out version);
                return true;
            }

            browserType = default;
            version = default;
            return false;
        }

        private static bool TryDetectFireFox(this string userAgent, out BrowserType browserType, out int? version)
        {
            if (userAgent.Contains("Firefox/"))
            {
                browserType = BrowserType.FireFox;
                userAgent.SearchBrowserVersion("Firefox/", 5, out version);
                return true;
            }

            browserType = default;
            version = default;
            return false;
        }

        private static bool TryDetectSafari(this string userAgent, out BrowserType browserType, out int? version)
        {
            if ((userAgent.Contains("Safari/") && userAgent.Contains("Version/")) || userAgent.Contains("Mozilla/"))
            {
                browserType = BrowserType.Safari;
                userAgent.SearchBrowserVersion("Version/", 5, out version);
                return true;
            }
            else if (userAgent.Contains("iPhone"))
            {
                browserType = BrowserType.Safari;
                userAgent.SearchBrowserVersion("OS", 5, out version);
                return true;
            }

            browserType = default;
            version = default;
            return false;
        }

        private static bool TryDetectOpera(this string userAgent, out BrowserType browserType, out int? version)
        {
            if (userAgent.Contains("Opera"))
            {
                browserType = BrowserType.Opera;
                userAgent.SearchBrowserVersion("Opera", 5, out version);
                return true;
            }
            if (userAgent.Contains("OPR/"))
            {
                browserType = BrowserType.Opera;
                userAgent.SearchBrowserVersion("OPR/", 5, out version);
                return true;
            }

            browserType = default;
            version = default;
            return false;
        }

        private static void SearchBrowserVersion(this string userAgent, string browserSearchWord, int searchLength, out int? version)
        {
            var versionSearchIndex = userAgent.IndexOf(browserSearchWord) + browserSearchWord.Length;

            if (userAgent.TryGetInt(versionSearchIndex, searchLength, out int foundVersion))
            {
                version = foundVersion;
            }
            else
            {
                version = default;
            }
        }
    }
}
