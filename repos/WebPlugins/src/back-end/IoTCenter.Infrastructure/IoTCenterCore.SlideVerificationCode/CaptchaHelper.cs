// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Newtonsoft.Json.Linq;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;

namespace IoTCenterCore.SlideVerificationCode
{
    public class CaptchaHelper
    {
        #region 参数
        private const int _shearSize = 40;
        private const int _imgNum = 30;
        private readonly int _imgWidth = 300;
        private readonly int _imgHeight = 300;
        private readonly int _minRangeX = 30;
        private readonly int _maxRangeX = 240;
        private readonly int _minRangeY = 30;
        private readonly int _maxRangeY = 140;
        private readonly int _cutX = 30;
        private readonly int _cutY = 150;
        public int _PositionX { get; set; }

        public const int _deviationPx = 2;
        public const int _MaxErrorNum = 4;
        #endregion

        public string GetVerificationCode(string path)
        {
            int _positionY;
            var rd = new Random();

            _PositionX = rd.Next(_minRangeX, _maxRangeX);

            _positionY = rd.Next(_minRangeY, _maxRangeY);

            int[] numbers = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };

            int[] array = numbers.OrderBy(x => Guid.NewGuid()).ToArray();

            path = Path.Combine(path, $"{(new Random()).Next(1, _imgNum)}.jpg");

            using var input = File.OpenRead(path);
            
            using var inputStream = new SKManagedStream(input);
            
            using var original = SKBitmap.Decode(inputStream);
            var ls_small2 = "data:image/jpg;base64," + TOBase64String(CutImage2(original, _shearSize, _shearSize, _PositionX, _positionY).Encode(SKEncodedImageFormat.Jpeg, 100));
            
            var lb_normal2 = GetNewBitMap2(original, _shearSize, _shearSize, _PositionX, _positionY);
            
            var ls_confusion2 = "data:image/jpg;base64," + TOBase64String(ConfusionImage2(array, lb_normal2).Encode(SKEncodedImageFormat.Jpeg, 100));

            JObject jObject2 = new JObject
            {
                ["errcode"] = 0,
                ["y"] = _positionY,
                ["array"] = string.Join(",", array),
                ["imgx"] = _imgWidth,
                ["imgy"] = _imgHeight,
                ["small"] = ls_small2,
                ["normal"] = ls_confusion2
            };
            /* errcode: 状态值 成功为0
             * y:裁剪图片y轴位置
             * small：小图字符串
             * normal：剪切小图后的原图并按无序数组重新排列后的图
             * array：无序数组
             * imgx：原图宽
             * imgy：原图高
             */
            return jObject2.ToString();
        }

        private SKBitmap CutImage2(SKBitmap original, int cutWidth, int cutHeight, int x, int y)
        {
            using (var sur = SKSurface.Create(new SKImageInfo(_imgWidth, _imgHeight)))
            {
                using (var canvas = sur.Canvas)
                {
                    var skrec = SKRect.Create(x, y, cutWidth, cutHeight);
                    canvas.ClipRect(skrec);
                    canvas.DrawBitmap(original, 0, 0);
                }
                
                using var s = sur.Snapshot();
                
                using var s2 = s.Subset(new SKRectI(x, y, x + cutWidth, y + cutHeight));

                return SKBitmap.FromImage(s2);
            }
        }

        private SKBitmap ConfusionImage2(int[] a, SKBitmap cutbmp)
        {
            SKBitmap[] bmp = new SKBitmap[20];
            
            for (int i = 0; i < 20; i++)
            {
                int x, y;
                x = a[i] > 9 ? (a[i] - 10) * _cutX : a[i] * _cutX;
                y = a[i] > 9 ? _cutY : 0;
                bmp[i] = CutImage2(cutbmp, _cutX, _cutY, x, y);
            }

            using (var skSurface = SKSurface.Create(new SKImageInfo(_imgWidth, _imgHeight)))
            {
                using (var canvas = skSurface.Canvas)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        canvas.DrawBitmap(bmp[i], new SKPoint(i > 9 ? (i - 10) * _cutX : i * _cutX, i > 9 ? _cutY : 0));
                    }
                    using var s = skSurface.Snapshot();
                    return SKBitmap.FromImage(s);
                }
            }
        }

        private SKBitmap GetNewBitMap2(SKBitmap original, int cutWidth, int cutHeight, int x, int y)
        {
            using (var sur = SKSurface.Create(new SKImageInfo(_imgWidth, _imgHeight)))
            {
                using (var canvas = sur.Canvas)
                {
                    canvas.DrawBitmap(original, 0, 0);
                    SKPaint pa = new SKPaint();
                    pa.Color = new SKColor(0, 0, 0, 120);
                    canvas.DrawRegion(new SKRegion(new SKRectI(x, y, x + cutWidth, y + cutHeight)), pa);

                }
                using var s = sur.Snapshot();
                return SKBitmap.FromImage(s);
            }
        }
        private string TOBase64String(SKData data)
        {
            Stream ms = data.AsStream();
            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            return Convert.ToBase64String(arr);
        }
    }
}