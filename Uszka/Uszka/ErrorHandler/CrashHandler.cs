using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.FileSystem;
using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.HAL.Drivers;
using Uszka.Drivers;
using expack = Cosmos.Core.IOGroup.ExtPack;

namespace Uszka_OS
{
    public class CrashHandler
    {
        public void Whoops(Exception ex) 
        {
            try
            {
                DisplayDriver.Vcanvas.Clear(System.Drawing.Color.Blue);
                FontDrawer.WriteText("A Error has ocurred", 400, 300, expack.Color.Black);
                FontDrawer.WriteText(ex.Message, 400, 200, expack.Color.Black);
            }
            catch (Exception ex1) 
            {
                Delay D = new Delay();
                int i = 0;
                do {
                    PCSpeaker.Beep(1050);
                    D.DelayInMS(100);
                    PCSpeaker.Beep(900);
                    D.DelayInMS(100);
                    PCSpeaker.Beep(1050);
                    D.DelayInMS(100);
                    PCSpeaker.Beep(900);
                    D.DelayInMS(100);
                    PCSpeaker.Beep(1050);
                    D.DelayInMS(100);
                    PCSpeaker.Beep(900);
                    D.DelayInMS(100);
                    i++;
                } while (i != 5);
                //Power.ACPIShutdown();
            }
        }
    }
}
