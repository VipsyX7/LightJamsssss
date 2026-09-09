using UnityEngine;

namespace LightJam
{
    /// <summary>
    /// 运行时生成像素风占位图，方便空项目直接开玩。之后可换成自己的 Sprite。
    /// </summary>
    public static class SpriteFactory
    {
        public const float PixelsPerUnit = 32f;

        public static Sprite Solid(Color color, int width, int height)
        {
            var pixels = new Color32[width * height];
            var fill = (Color32)color;
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = fill;
            return FromPixels(pixels, width, height);
        }

        public static Sprite Rect(Color fill, Color border, int width, int height)
        {
            var pixels = new Color32[width * height];
            Fill(pixels, width, height, 0, 0, width, height, border);
            Fill(pixels, width, height, 1, 1, width - 2, height - 2, fill);
            return FromPixels(pixels, width, height);
        }

        public static Sprite Player()
        {
            const int w = 16;
            const int h = 24;
            var pixels = new Color32[w * h];
            Clear(pixels);

            var hair = C(42, 36, 56);
            var skin = C(232, 196, 168);
            var eye = C(28, 24, 36);
            var shirt = C(72, 168, 176);
            var shade = C(40, 112, 124);
            var pants = C(48, 52, 78);
            var shoe = C(32, 30, 40);

            Fill(pixels, w, h, 3, 16, 10, 7, hair);
            Fill(pixels, w, h, 4, 14, 8, 5, skin);
            Plot(pixels, w, h, 6, 16, eye);
            Plot(pixels, w, h, 9, 16, eye);
            Fill(pixels, w, h, 3, 7, 10, 8, shirt);
            Fill(pixels, w, h, 3, 7, 3, 8, shade);
            Fill(pixels, w, h, 4, 2, 3, 6, pants);
            Fill(pixels, w, h, 9, 2, 3, 6, pants);
            Fill(pixels, w, h, 3, 0, 4, 2, shoe);
            Fill(pixels, w, h, 9, 0, 4, 2, shoe);
            return FromPixels(pixels, w, h, new Vector2(0.5f, 0f));
        }

        public static Sprite Ground()
        {
            return Brick(C(92, 78, 68), C(62, 50, 44), C(130, 108, 90), 64, 16);
        }

        public static Sprite Wall()
        {
            return Brick(C(48, 52, 72), C(34, 36, 52), C(70, 76, 98), 32, 64);
        }

        public static Sprite Note()
        {
            const int w = 14;
            const int h = 16;
            var pixels = new Color32[w * h];
            Fill(pixels, w, h, 0, 0, w, h, C(58, 42, 28));
            Fill(pixels, w, h, 1, 1, w - 2, h - 2, C(232, 214, 168));
            var ink = C(92, 72, 48);
            Fill(pixels, w, h, 3, 11, 8, 1, ink);
            Fill(pixels, w, h, 3, 8, 7, 1, ink);
            Fill(pixels, w, h, 3, 5, 8, 1, ink);
            Fill(pixels, w, h, 3, 2, 5, 1, ink);
            return FromPixels(pixels, w, h, new Vector2(0.5f, 0f));
        }

        public static Sprite Lamp()
        {
            const int w = 16;
            const int h = 28;
            var pixels = new Color32[w * h];
            Clear(pixels);
            Fill(pixels, w, h, 6, 0, 4, 8, C(64, 56, 48));
            Fill(pixels, w, h, 7, 8, 2, 6, C(80, 72, 60));
            Fill(pixels, w, h, 3, 14, 10, 10, C(240, 188, 92));
            Fill(pixels, w, h, 4, 15, 8, 8, C(255, 226, 140));
            Fill(pixels, w, h, 2, 24, 12, 3, C(72, 64, 52));
            return FromPixels(pixels, w, h, new Vector2(0.5f, 0f));
        }

        public static Sprite Door(bool open)
        {
            const int w = 28;
            const int h = 48;
            var pixels = new Color32[w * h];
            var frame = C(46, 32, 28);
            var wood = open ? C(28, 22, 26) : C(92, 48, 42);
            var panel = open ? C(18, 16, 20) : C(120, 64, 54);
            Fill(pixels, w, h, 0, 0, w, h, frame);
            Fill(pixels, w, h, 2, 2, w - 4, h - 4, wood);
            Fill(pixels, w, h, 5, 8, 8, 14, panel);
            Fill(pixels, w, h, 15, 8, 8, 14, panel);
            Fill(pixels, w, h, 5, 26, 8, 14, panel);
            Fill(pixels, w, h, 15, 26, 8, 14, panel);
            if (!open)
                Fill(pixels, w, h, 20, 22, 3, 5, C(212, 168, 72));
            return FromPixels(pixels, w, h, new Vector2(0.5f, 0f));
        }

        public static Sprite Painting()
        {
            const int w = 22;
            const int h = 18;
            var pixels = new Color32[w * h];
            Fill(pixels, w, h, 0, 0, w, h, C(86, 62, 40));
            Fill(pixels, w, h, 2, 2, w - 4, h - 4, C(48, 78, 96));
            Fill(pixels, w, h, 2, 2, w - 4, 6, C(168, 132, 96));
            Fill(pixels, w, h, 8, 5, 6, 8, C(62, 50, 58));
            return FromPixels(pixels, w, h);
        }

        public static Sprite Window()
        {
            const int w = 20;
            const int h = 24;
            var pixels = new Color32[w * h];
            Fill(pixels, w, h, 0, 0, w, h, C(70, 78, 96));
            Fill(pixels, w, h, 2, 2, 7, 9, C(36, 64, 88));
            Fill(pixels, w, h, 11, 2, 7, 9, C(32, 58, 82));
            Fill(pixels, w, h, 2, 13, 7, 9, C(28, 52, 76));
            Fill(pixels, w, h, 11, 13, 7, 9, C(30, 56, 80));
            Fill(pixels, w, h, 9, 0, 2, h, C(88, 96, 114));
            Fill(pixels, w, h, 0, 11, w, 2, C(88, 96, 114));
            return FromPixels(pixels, w, h);
        }

        public static Sprite Backdrop()
        {
            return Solid(C(22, 24, 38), 16, 16);
        }

        static Sprite Brick(Color fill, Color grout, Color highlight, int width, int height)
        {
            var pixels = new Color32[width * height];
            Fill(pixels, width, height, 0, 0, width, height, grout);
            int brickH = 7;
            for (int y = 1; y < height; y += brickH + 1)
            {
                int offset = ((y / (brickH + 1)) % 2) * 8;
                for (int x = 1 - offset; x < width; x += 16)
                    Fill(pixels, width, height, x, y, 14, brickH, fill);
            }

            for (int x = 0; x < width; x += 11)
                Plot(pixels, width, height, x % width, height - 2, highlight);

            return FromPixels(pixels, width, height, new Vector2(0.5f, 1f));
        }

        static void Clear(Color32[] pixels)
        {
            var empty = new Color32(0, 0, 0, 0);
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = empty;
        }

        static void Fill(Color32[] pixels, int w, int h, int x, int y, int fw, int fh, Color32 color)
        {
            for (int py = y; py < y + fh; py++)
            {
                if (py < 0 || py >= h)
                    continue;
                for (int px = x; px < x + fw; px++)
                {
                    if (px < 0 || px >= w)
                        continue;
                    pixels[py * w + px] = color;
                }
            }
        }

        static void Plot(Color32[] pixels, int w, int h, int x, int y, Color32 color)
        {
            if (x < 0 || y < 0 || x >= w || y >= h)
                return;
            pixels[y * w + x] = color;
        }

        static Color32 C(byte r, byte g, byte b, byte a = 255)
        {
            return new Color32(r, g, b, a);
        }

        static Sprite FromPixels(Color32[] pixels, int width, int height)
        {
            return FromPixels(pixels, width, height, new Vector2(0.5f, 0.5f));
        }

        static Sprite FromPixels(Color32[] pixels, int width, int height, Vector2 pivot)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = "GeneratedSprite"
            };
            texture.SetPixels32(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, width, height), pivot, PixelsPerUnit);
        }
    }
}
