using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.FileSystem;
using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.HAL.Drivers;
using System.Drawing;
using Jajka_OS;

namespace Uszka.Drivers
{
    public class DisplayDriver
    {
        //Bitmap bootmap = new Bitmap(Properties.Resources.BootLogo);
        public static Canvas Vcanvas;
        public void Startup() 
        {
            try
            {
                Vcanvas = FullScreenCanvas.GetFullScreenCanvas();
                Vcanvas.Clear(Color.Black);
                //DisplayDriver.Vcanvas.DrawImage(bootmap, 0, 0); //CRASHES!!! WTF

                //Display text at the bottom of the screen or right below the logo displaying words
            }
            catch (Exception ex) 
            {
                CrashHandler CH = new CrashHandler();
                CH.Whoops(ex);
            }
        }
    }
}
