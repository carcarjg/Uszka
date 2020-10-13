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
using Uszka.Drivers.Font;

namespace Uszka.Drivers
{
    public struct Cursor
    {
        public int x;
        public int y;
        public int prevx;
        public int prevy;
        public bool changedMouse;
    }

    public struct Window
    {
        public int x;
        public int y;
        public int W;
        public int H;

        public int prevx;
        public int prevy;
        public bool changedWindow;
    }

    class GraphicsManager
    {
        private Cursor cursor;
        public static VGA canvas = new VGA();
        private bool changedBuffer = false;
        List<WindowContainer> WindowContainers = new List<WindowContainer>();
        public GraphicsManager()
        {
            Sys.MouseManager.ScreenHeight = (uint)canvas.Mode.Rows;
            Sys.MouseManager.ScreenWidth = (uint)canvas.Mode.Columns;

            cursor.x = (int)(canvas.Mode.Rows / 2);
            cursor.y = (int)(canvas.Mode.Columns / 2);

            Sys.MouseManager.X = (uint)(canvas.Mode.Rows / 2);
            Sys.MouseManager.Y = (uint)(canvas.Mode.Columns / 2);

            //canvas.CleanScreen(Color.Blue);
            //canvas.PutPixel(cursor.x, cursor.y, Color.BrightGreen, true);

            changedBuffer = true;
        }

        public void AddNewWindowContainer(int BaseX, int BaseY, int width, int height, Color? color = null)
        {
            Color color1 = color ?? Color.Red;
            WindowContainer windowContainer = new WindowContainer(BaseX, BaseY, width, height, color1);
            WindowContainers.Add(windowContainer);
        }

        public bool CheckMousePos()
        {
            cursor.prevx = cursor.x;
            cursor.prevy = cursor.y;
            if ((int)Sys.MouseManager.X != cursor.x || (int)Sys.MouseManager.Y != cursor.y)
            {
                if (Sys.MouseManager.X >= (uint)canvas.Mode.Columns)
                    Sys.MouseManager.X = (uint)canvas.Mode.Columns;
                else if (Sys.MouseManager.Y >= (uint)canvas.Mode.Rows)
                    Sys.MouseManager.Y = (uint)canvas.Mode.Rows;
                cursor.x = (int)Sys.MouseManager.X;
                cursor.y = (int)Sys.MouseManager.Y;
                cursor.changedMouse = true;
            }
            else
                cursor.changedMouse = false;

            if (cursor.changedMouse == true)
                return true;
            //else
            return false;
        }

        private void renderMouse()
        {
            foreach (WindowContainer container in WindowContainers)
            {
                if (container.IsPixelInArray(cursor.prevx, cursor.prevy))
                {
                    canvas.PutPixel(cursor.prevx, cursor.prevy, container.GetPixelColor(cursor.prevx, cursor.prevy), true);
                }
            }
            canvas.PutPixel(cursor.x, cursor.y, Color.Gray, true);
        }

        public void start()
        {

            AddNewWindowContainer(0, 0, 320, 200, Color.Purple);
            AddNewWindowContainer(3, 3, 90, 35, Color.Green);
            changedBuffer = true;

            for (; ; )  // Make infinite loop
            {
                //Do some stuff
                if (changedBuffer == true)
                {
                    canvas.Render();
                    changedBuffer = false;
                }

                if (CheckMousePos())
                {
                    renderMouse();
                }
            }
        }

    }
}
