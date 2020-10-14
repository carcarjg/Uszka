﻿/*
* PROJECT:          Aura Operating System Development
* CONTENT:          VBE VESA Console
* PROGRAMMERS:      Valentin Charbonnier <valentinbreiz@gmail.com>
*/


using System;
using System.Drawing;
using Cosmos.Debug.Kernel;
using Cosmos.System.Graphics;

namespace Aura_OS.System.AConsole.VESAVBE
{
    public class VESAVBEConsole : Console
    {

        public Graphics.Graphics graphics;
        private const byte LineFeed = (byte)'\n';
        private const byte CarriageReturn = (byte)'\r';
        private const byte Tab = (byte)'\t';
        private const byte Space = (byte)' ';

        public Debugger debugger = new Debugger("", "");

        public VESAVBEConsole()
        {
            Name = "VESA";
            graphics = new Graphics.Graphics();
            mWidth = Graphics.Graphics.canvas.Mode.Columns / graphics.font.Width;
            mHeight = Graphics.Graphics.canvas.Mode.Rows / graphics.font.Height;

            mCols = mWidth;
            mRows = mHeight;

            debugger.Send("Height=" + Graphics.Graphics.canvas.Mode.Rows);
            debugger.Send("rows=" + mRows);

            debugger.Send("Width=" + Graphics.Graphics.canvas.Mode.Columns);
            debugger.Send("mCols=" + mCols);
        }

        protected int mX = 0;
        public override int X
        {
            get { return mX; }
            set
            {
                mX = value;
            }
        }


        protected int mY = 0;
        public override int Y
        {
            get { return mY; }
            set
            {
                mY = value;
            }
        }

        public static int mWidth;
        public override int Width
        {
            get { return mWidth; }
        }

        public static int mHeight;
        public override int Height
        {
            get { return mHeight; }
        }

        public static int mCols;
        public override int Cols
        {
            get { return mCols; }
        }

        public static int mRows;
        public override int Rows
        {
            get { return mRows; }
        }

        public static uint foreground = (byte)ConsoleColor.White;
        public override ConsoleColor Foreground
        {
            get { return (ConsoleColor)foreground; }
            set
            {
                foreground = (byte)global::System.Console.ForegroundColor;
                graphics.ChangeForegroundPen(foreground);
            }
        }

        public static uint background = (byte)ConsoleColor.Black;

        public override ConsoleColor Background
        {
            get { return (ConsoleColor)background; }
            set
            {
                background = (byte)global::System.Console.BackgroundColor;
                graphics.ChangeBackgroundPen(background);
            }
        }

        public override int CursorSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool CursorVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Clear()
        {
            Graphics.Graphics.canvas.Clear();
            mX = 0;
            mY = 0;
        }

        public override void Clear(uint color)
        {
            Graphics.Graphics.canvas.Clear(Color.FromArgb((int)color));
            mX = 0;
            mY = 0;
        }

        /// <summary>
        /// Scroll the console up and move crusor to the start of the line.
        /// </summary>
        private void DoLineFeed()
        {
            mY++;
            mX = 0;
            if (mY == mRows)
            {
                Aura_OS.System.Graphics.Graphics g = new Graphics.Graphics();
                g.ScrollUp();
                mY--;
            }
            //UpdateCursor();
        }

        private void DoCarriageReturn()
        {
            mX = 0;
            //UpdateCursor();
        }

        /// <summary>
        /// Write char to the console.
        /// </summary>
        /// <param name="aChar">A char to write</param>
        public void Write(byte aChar)
        {
            if (aChar == 0)
                return;

            graphics.WriteByte(aChar);
            mX++;
            if (mX == mCols)
            {
                DoLineFeed();
            }
            //UpdateCursor();
        }

        public override void Write(byte[] aText)
        {
            for (int i = 0; i < aText.Length; i++)
            {
                switch (aText[i])
                {
                    case LineFeed:
                        DoLineFeed();
                        break;

                    case CarriageReturn:
                        DoCarriageReturn();
                        break;

                    case Tab:
                        DoTab();
                        break;

                    /* Normal characters, simply write them */
                    default:
                        Write(aText[i]);
                        break;
                }
            }
        }

        private void DoTab()
        {
            graphics.WriteByte(Space);
            graphics.WriteByte(Space);
            graphics.WriteByte(Space);
            graphics.WriteByte(Space);
        }

        public override void DrawImage(ushort X, ushort Y, Bitmap image)
        {
            Graphics.Graphics.canvas.DrawImage(image, X, Y);
        }

    }
}
