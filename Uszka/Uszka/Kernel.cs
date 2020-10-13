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
using Uszka.Drivers;
using System.IO;

namespace Uszka
{
    public class Kernel : Sys.Kernel
    {
        Sys.FileSystem.CosmosVFS fs;
        Drivers.DisplayDriver DD = new DisplayDriver();
        string current_directory = "0:\\";
        char[] Flags;
        GraphicsManager GM;
       
        protected override void BeforeRun()
        {
            ACPI.Enable();
            ACPI.Start();
            fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            Console.Clear();

            if (File.Exists("0:\\Flags.flg")) 
            {
                Flags = new char[99];
                Flags = File.ReadAllText("0:\\Flags.flg").ToCharArray();
                Flags[1] = '0';//Force GFX mode
                if (Flags[0] == '1')
                {
                    Uszka.Installer.Installer inst = new Installer.Installer();
                    inst.start_inst(fs);
                }
                else if (Flags[0] == '2') 
                {
                    Console.ReadKey();
                }
                if (Flags[1] == '0')//GUI Mode
                {
                    Console.WriteLine("Vga Driver Booting");
                    try
                    {
                        GM = new GraphicsManager();
                    }
                    catch 
                    {
                        
                    }
                    DD.Startup();
                }
                else if (Flags[1] == '1')//Text Mode/Console Mode
                {
                    
                }
            }
            if (!File.Exists("0:\\Uszaka32\\System.cx") && !File.Exists("0:\\Flags.flg"))
            {
                // There is no SYSTEM directory yet, so we just shut the computer down there after setting flags
                if (!File.Exists("0:\\Flags.flg"))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText("0:\\Flags.flg"))
                    {
                        sw.Write("1");
                    }
                }
                //Power.ACPIReboot();
            }
            else
            {
                File.ReadAllText("0:\\Uszaka32\\System.cx");
            }
        }
        protected override void Run()
        {
            GM.start();
        }
    }
    public class UIstartup 
    {
        void Start() 
        {
            //Add task to draw taskbar
        }
    }
}
