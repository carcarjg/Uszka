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
                Aura_OS.System.CustomConsole.WriteLineError("A Error has ocurred");
                Aura_OS.System.CustomConsole.WriteLineError(ex.Message);
            }
            catch (Exception ex1) 
            {
                DisplayDriver.Vcanvas.Clear(System.Drawing.Color.Red);
                Power.ACPIShutdown();
            }
        }
    }
}
