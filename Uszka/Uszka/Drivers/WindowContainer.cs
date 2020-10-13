using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.FileSystem;
using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.HAL.Drivers;
using Cosmos.Core.IOGroup.ExtPack;
using Cosmos.Core.IOGroup;
using Uszka.Drivers.Font;

namespace Uszka.Drivers
{
    public struct Pixel
    {
        public Color color;
        public int x;
        public int y;
    }
    public class WindowContainer
    {
        public List<Pixel> pixels = new List<Pixel>();
        public int BaseX;
        public int BaseY;
        public int W;
        public int H;
        public Color color;
        public bool changed = false;

        public WindowContainer(int X, int Y, int width, int height, Color? color = null)
        {
            this.color = color ?? Color.Red;
            BaseX = X;
            BaseY = Y;
            W = width;
            H = height;

            Clear();


        }

        public void Clear()
        {
            int endX = W;
            int endY = H;

            for (int y = 0; y < endY; y++)
            {
                for (int x = 0; x < endX; x++)
                {
                    DrawPoint(x, y, color);
                }
            }

            changed = true;
        }

        public void DrawPoint(int x, int y, Color color)
        {
            GraphicsManager.canvas.PutPixel(x + BaseX, y + BaseY, color);
            Pixel px = new Pixel();
            px.x = x;
            px.y = y;
            px.color = color;
            pixels.Add(px);
        }

        public bool IsPixelInArray(int x, int y)
        {
            foreach (Pixel pixel in pixels)
            {
                if (pixel.x == x - BaseX && pixel.y == y - BaseY)
                    return true;
            }

            return false;
        }

        public Color GetPixelColor(int x, int y)
        {
            foreach (Pixel px in pixels)
                if (px.x == x - BaseX && px.y == y - BaseY) return px.color;
            return color;
        }
    }
}
