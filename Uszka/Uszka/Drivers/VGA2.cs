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

namespace Cosmos.Core.IOGroup.ExtPack
{
    public enum Color
    {
        Black,
        Blue,
        Green,
        LightBlue,
        Red,
        Purple,
        Orange,
        Gray,
        DarkGray,
        UnNamed1,
        BrightGreen,
        BrightLightBlue
    };

    /// <summary>
    /// This is the VGA driver, use this to initialize VGA video mode and draw stuff to screen.
    /// <para><see cref="VGA.Render"/> to render the buffer on the Video mem</para>
    /// For graphics drawing see: <seealso cref="VGA.PutPixel(int, int, Color, bool)"/>, 
    /// <seealso cref="VGA.DrawFilledRectangle(int, int, int, int, Color, bool)"/>, 
    /// <seealso cref="VGA.DrawFilledRectangle(int, int, int, int, Color[], bool)"/>, 
    /// <seealso cref="VGA.GetPixelColor(int, int)"/>, 
    /// <seealso cref="VGA.GetBufferColor(int, int, int, int)"/>, 
    /// <seealso cref="VGA.CleanScreen(Color)"/>.
    /// </summary>
    unsafe class VGA
    {

        private readonly IOPortWrite AttributeController_Index = new IOPortWrite(0x3C0);
        private readonly IOPortWrite AttributeController_Write = new IOPortWrite(0x3C0);
        private readonly IOPortRead AttributeController_Read = new IOPortRead(0x3C1);
        private readonly IOPortWrite MiscellaneousOutput_Write = new IOPortWrite(0x3C2);
        private readonly IOPortWrite Sequencer_Index = new IOPortWrite(0x3C4);
        private readonly IOPort Sequencer_Data = new IOPort(0x3C5);
        private readonly IOPortRead DACIndex_Read = new IOPortRead(0x3C7);
        private readonly IOPortWrite DACIndex_Write = new IOPortWrite(0x3C8);
        private readonly IOPortWrite DAC_Data = new IOPortWrite(0x3C9);
        private readonly IOPortWrite GraphicsController_Index = new IOPortWrite(0x3CE);
        private readonly IOPort GraphicsController_Data = new IOPort(0x3CF);
        private readonly IOPortWrite CRTController_Index = new IOPortWrite(0x3D4);
        private readonly IOPort CRTController_Data = new IOPort(0x3D5);
        private readonly IOPortRead Instat_Read = new IOPortRead(0x3DA);
        private byte* BackBuffer;
        int bufsize;
        private bool writeOnVGAMem = false;
        private Color[] colorsInBuffer;
        public Cosmos.System.Graphics.Mode Mode;
        private byte PixelStride;
        private ushort ScrW, ScrH;
        private int Pitch;
        private byte BitsPerPixel = 8;

        /// <summary>
        /// <returns>Returns the <i>address pointer</i>(<see cref="byte*"/>) in which the <b>Video Memory</b> is located.</returns>
        /// <para>
        /// <see cref="byte*"/> to know more about <i>address pointers</i>
        /// <br/>
        /// <see cref="Cosmos.System.memorymanager"/> to know more about how the swap between <b>buffer</b> and <b>video memory</b> works
        /// </para>
        /// 
        /// <para>
        /// See also: <seealso cref="Cosmos.System.memorymanager.MemAlloc(byte*, byte, uint)"/> and <seealso cref="Cosmos.System.memorymanager.MemSwap(byte*, byte*, int)"/>
        /// </para>
        /// </summary>
        private byte* GetFrameBufferSegment()
        {
            GraphicsController_Index.Byte = 0x06;
            byte segmentNumber = (byte)(GraphicsController_Data.Byte & (3 << 2));
            switch (segmentNumber)
            {
                default:
                case 0 << 2: return (byte*)0x00000;
                case 1 << 2: return (byte*)0xA0000;
                case 2 << 2: return (byte*)0xB0000;
                case 3 << 2: return (byte*)0xB8000;
            }
        }

        private void WriteRegs(byte* regs)
        {
            /*MISC*/
            MiscellaneousOutput_Write.Byte = *(regs++);

            /*SEQUENCER*/
            for (byte i = 0; i < 5; i++)
            {
                Sequencer_Index.Byte = i;
                Sequencer_Data.Byte = *(regs++);
            }

            /*CRTC*/
            CRTController_Index.Byte = 0x03;
            CRTController_Data.Byte = (byte)(CRTController_Data.Byte | 0x80);

            CRTController_Index.Byte = 0x11;
            CRTController_Data.Byte = (byte)(CRTController_Data.Byte & ~0x80);

            regs[0x03] = (byte)(regs[0x03] | 0x80);
            regs[0x11] = (byte)(regs[0x11] & ~0x80);

            for (byte i = 0; i < 25; i++)
            {
                CRTController_Index.Byte = i;
                CRTController_Data.Byte = *(regs++);
            }

            /*GC*/
            for (byte i = 0; i < 9; i++)
            {
                GraphicsController_Index.Byte = i;
                GraphicsController_Data.Byte = *(regs++);
            }

            /*ATTRIBUTE CONTROLLER*/
            for (byte i = 0; i < 21; i++)
            {
                byte _foo = Instat_Read.Byte;
                AttributeController_Index.Byte = i;
                AttributeController_Write.Byte = *(regs++);
            }

            byte foo = Instat_Read.Byte;
            AttributeController_Index.Byte = 0x20;
        }

        public Color GetPixelColor(int x, int y)
        {
            return colorsInBuffer[x * y];
        }

        public Color[] GetBufferColor(int x, int y, int bufW, int bufH)
        {

            int endx = x + bufW;
            int endy = y + bufH;

            Color[] colors = new Color[endx * endy];

            for (int Y = y; Y < endy; Y++)
                for (int X = x; X < endx; X++)
                    colors[X * Y] = GetPixelColor(X, Y);

            return colors;
        }

        public void PutPixel(int x, int y, Color colorIndex, bool direct = false)
        {
            if (x < 0 || 320 <= x
            || y < 0 || 200 <= y)
                return;

            if (!writeOnVGAMem && !direct)
            {
                byte* pixelAddress = BackBuffer + 320 * y + x;
                *pixelAddress = (byte)colorIndex;
                colorsInBuffer[x * y] = colorIndex;
            }
            else
            {
                byte* pixelAddress = GetFrameBufferSegment() + 320 * y + x;
                *pixelAddress = (byte)colorIndex;
            }
        }

        public void DrawFilledRectangle(int x, int y, int width, int height, Color color = Color.Black, bool direct = false)
        {
            int endx = x + width;
            int endy = y + height;

            for (int Y = y; Y < endy; Y++)
                for (int X = x; X < endx; X++)
                    PutPixel(X, Y, color, direct);

        }

        public void DrawFilledRectangle(int x, int y, int width, int height, Color[] color, bool direct = false)
        {
            int endx = x + width;
            int endy = y + height;

            for (int Y = y; Y < endy; Y++)
                for (int X = x; X < endx; X++)
                    PutPixel(X, Y, color[X * Y], direct);

        }

        public void Render()
        {
            if (writeOnVGAMem)
                return;

            Sys.memorymanager.MemSwap(GetFrameBufferSegment(), BackBuffer, bufsize);
        }

        public void CleanScreen(Color color = Color.Black)
        {
            for (int y = 0; y < 200; y++)
                for (int x = 0; x < 320; x++)
                    PutPixel(x, y, color);
        }

        public VGA(bool buffered = true)
        {
            byte[] g_320x200x256 =
            {
                /* MISC */
                    0x63,
                /* SEQ */
                    0x03, 0x01, 0x0F, 0x00, 0x0E,
                /* CRTC */
                    0x5F, 0x4F, 0x50, 0x82, 0x54, 0x80, 0xBF, 0x1F,
                    0x00, 0x41, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x9C, 0x0E, 0x8F, 0x28, 0x40, 0x96, 0xB9, 0xA3,
                    0xFF,
                /* GC */
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x05, 0x0F,
                    0xFF,
                /* AC */
                    0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
                    0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                    0x41, 0x00, 0x0F, 0x00, 0x00
            };

            fixed (byte* fixedPTR = g_320x200x256)
            {
                WriteRegs(fixedPTR);
            }

            if (!buffered)
            {
                writeOnVGAMem = true;
                return;
            }



            ScrW = 320;
            ScrH = 200;

            PixelStride = (byte)((BitsPerPixel | 7) >> 3);
            Pitch = ScrW * PixelStride;
            BackBuffer = (byte*)(ScrH * Pitch);
            bufsize = ScrH * Pitch;

            Mode = new Sys.Graphics.Mode(ScrW, ScrH, Sys.Graphics.ColorDepth.ColorDepth8);

            colorsInBuffer = new Color[(ScrH * ScrW)];

        }
    }
}
