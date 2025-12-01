// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IoTCenter.Utilities
{
    public class ImageTypeUtil
    {
        private static Dictionary<int, ImageType> pairs = new Dictionary<int, ImageType>()
        {
            {0, ImageType.None },
            {0x4D42, ImageType.PCX},
            {0xD8FF, ImageType.JPG},
            {0x4947, ImageType.GIF},
            {0x050A, ImageType.PCX},
            {0x5089, ImageType.PNG},
            {0x4238, ImageType.PSD},
            {0xA659, ImageType.RAS},
            {0xDA01, ImageType.SGI},
            {0x4949, ImageType.TIFF}
        };

        public static ImageType CheckImageType(Stream stream)
        {
            var buffer = new byte[2];
            try
            {
                using var streamReader = new StreamReader(stream, Encoding.UTF8);
                var i = streamReader.BaseStream.Read(buffer, 0, buffer.Length);
                if (i != buffer.Length)
                {
                    return ImageType.None;
                }
            }
            catch
            {
                return ImageType.None;
            }

            if (buffer.Length < 2)
            {
                return ImageType.None;
            }

            var key = (buffer[1] << 8) + buffer[0];

            if (pairs.TryGetValue(key, out ImageType type))
            {
                return type;
            }
            return ImageType.None;
        }

        public enum ImageType
        {
            None = 0,
            BMP = 0x4D42,
            JPG = 0xD8FF,
            GIF = 0x4947,
            PCX = 0x050A,
            PNG = 0x5089,
            PSD = 0x4238,
            RAS = 0xA659,
            SGI = 0xDA01,
            TIFF = 0x4949
        }
    }
}
