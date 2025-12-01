// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using SkiaSharp;
using System;
using System.IO;
using System.Linq;

namespace IoTCenterCore.Hei.Captcha;

public class SkiaSharpVerificationCodeCreater
{
    private static string fontPath = Path.Combine(AppContext.BaseDirectory, "fonts");

    public SkiaSharpVerificationCodeCreater(int Width, int Height)
    {
        var info = new SKImageInfo(Width, Height);

        Surface = SKSurface.Create(info);

        var fontManager = SKFontManager.Default;
        if (EmojiTypeface == null)
        {
            if (!Directory.Exists(fontPath))
            {
                throw new ArgumentNullException($"绘制验证码字体文件不存在，请将字体文件(.ttf)复制到目录：{fontPath}");

            }
            else
            {
                var fontFile = Directory.GetFiles(fontPath, "*.ttf").FirstOrDefault();
                if (fontFile == default)
                {
                    throw new ArgumentNullException($"绘制验证码字体文件不存在，请将字体文件(.ttf)复制到目录：{fontPath}");
                }
                EmojiTypeface = fontManager.CreateTypeface(fontFile);
            }
        }

        Random1 = new Random();
    }
    SKSurface Surface;
    static SKTypeface EmojiTypeface;
    Random Random1;

    public byte BackColorR;
    public byte BackColorG;
    public byte BackColorB;
    public byte BackOffset;
    int TextSize = 30;

    public (byte begin, byte end, byte length) BoundsR
    {
        get
        {
            return Bounds(BackColorR);
        }
    }

    public (byte begin, byte end, byte length) BoundsG
    {
        get
        {
            return Bounds(BackColorG);
        }
    }

    public (byte begin, byte end, byte length) BoundsB
    {
        get
        {
            return Bounds(BackColorB);
        }
    }

    private (byte begin, byte end, byte length) Bounds(byte Value)
    {
        (byte begin, byte end, byte length) res = (0, 0, 0);

        res.begin = (byte)Math.Max(0, Value - BackOffset);
        res.end = (byte)Math.Min(255, Value + BackOffset);
        res.length = (byte)(res.end - res.begin);

        return res;
    }

    public void Clear(byte r = 41, byte g = 49, byte b = 66, byte a = 0)
    {
        BackColorR = r;
        BackColorG = g;
        BackColorB = b;
        var canvas = Surface.Canvas;
        canvas.Clear(new SKColor(BackColorR, BackColorG, BackColorB, a));
    }
    public void WriteText(string Text)
    {
        var canvas = Surface.Canvas;
        var xPoint = 6;///x点
        var yPoint = (Surface.Canvas.DeviceClipBounds.Height + TextSize) / 2; ///y点
        var paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            TextAlign = SKTextAlign.Left,
            TextSize = TextSize,
            TextEncoding = SKTextEncoding.Utf8,
            Typeface = EmojiTypeface,
            StrokeWidth = 3
        };

        for (int i = 0; i < Text.Length; i++)
        {
            int xOffset = Random1.Next(-TextSize * 2 / 10, 1);///x偏移
            int yOffset = Random1.Next(-3, 3);///y偏移
            int angleOffset = Random1.Next(-15, 15);///角度偏移
            paint.Color = RandColoe();///随机颜色

            canvas.RotateDegrees(angleOffset, xPoint + xOffset, yPoint + yOffset);
            canvas.DrawText(Text[i].ToString(), xPoint + xOffset, yPoint + yOffset, paint);

            canvas.RotateDegrees(-angleOffset, xPoint + xOffset, yPoint + yOffset);
            xPoint = xPoint + TextSize + xOffset;
        }
    }

    public void Confuse(int Count)
    {
        var canvas = Surface.Canvas;

        for (int i = 0; i < Count; i++)
        {
            int x = Random1.Next(0, canvas.DeviceClipBounds.Width);
            int y = Random1.Next(0, canvas.DeviceClipBounds.Height);
            int radius = Random1.Next(TextSize, TextSize * 2);

            var paint = new SKPaint
            {
                Color = RandColoe(),///随机颜色
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                TextAlign = SKTextAlign.Left,
                TextSize = TextSize,
                TextEncoding = SKTextEncoding.Utf8,
                Typeface = EmojiTypeface,
                StrokeWidth = 1
            };
            canvas.DrawCircle(x, y, radius, paint);
        }
    }

    private SKColor RandColoe()
    {

        var r = Random1.Next(0, 255 - BoundsR.length);
        if (r > BoundsR.begin && r < BoundsR.end)
        {
            r = (r + BoundsR.length) % 256;
        }

        var g = (r + Random1.Next(50, 190)) % 256;

        if (g > BoundsG.begin && g < BoundsG.end)
        {
            g = (g + BoundsG.length) % 256;
        }

        var b = (g + Random1.Next(50, 190)) % 256;
        if (b > BoundsB.begin && b < BoundsB.end)
        {
            b = (b + BoundsB.length) % 256;
        }

        return new SKColor((byte)r, (byte)g, (byte)b);
    }

    public byte[] GetImg()
    {
        using (var image = Surface.Snapshot())
        using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
        {
            return data.ToArray();
        }
    }

    public byte[] CreateVerificationImage(string Text)
    {
        Clear();
        WriteText(Text);
        Confuse(Text.Length);
        var stream = GetImg();
        return stream;
    }
}
