using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.FileSystem;
using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.HAL.Drivers;
using System.IO;
using Jajka_OS;

namespace Uszka.Installer
{
    class Installer
    {
        public void start_inst(CosmosVFS fs) 
        {
            try
            {
                //This is in the if.. statement. Remove the Kernel.Stop();
                Console.Clear();
                Console.WriteLine("-----Uszka Installer-----");
                /*
                Console.WriteLine("Create a username and password:");
                Console.Write("Username: ");
                string username = Console.ReadLine();
                Console.Write("Password: ");
                string cPassword = Console.ReadLine();*/
                Console.WriteLine("Starting Installation Process...");
                Console.WriteLine("Creating System Directory...");
                fs.CreateDirectory("0:\\SYSTEM\\");
                Console.WriteLine("Creating System Files");
                fs.CreateFile("0:\\SYSTEM\\System.cx");
                fs.CreateFile("0:\\SYSTEM\\users.db");
                fs.CreateFile("0:\\SYSTEM\\readme.txt");
                fs.CreateFile("0:\\SYSTEM\\sysinfo.txt");
                Console.WriteLine("Setting User Preferences...");
                File.WriteAllText("0:\\SYSTEM\\System.cs", "using tutOS.System;" +
                    "namespace System { " +
                    "class System" +
                    "{" +
                    "public void Main()" +
                    "{" +
                    "Console.WriteLine(\"System File. Do not edit.\"" +
                    ") } } }");
                File.WriteAllText("0:\\SYSTEM\\users.db", "No Users need run afterinstall");
                File.WriteAllText("0:\\SYSTEM\\readme.txt", "Note: Put the license here!");
                File.WriteAllText("0:\\SYSTEM\\sysinfo.txt", "Uszka Version 0.0.1 [8 GB]");
                fs.CreateDirectory("0:\\Documents\\");
                Console.WriteLine("Deleting Preinstalled CosmosVFS Files");
                //Directory.Delete("0:\\Dir Testing\\");
                //Directory.Delete("0:\\TEST\\");
                //fs.DeleteDirectory("0:\\Dir Testing\\");
                //fs.DeleteDirectory("0:\\TEST\\");
                File.Delete("0:\\Kudzu.txt");
                File.Delete("0:\\Root.txt");

                Console.WriteLine("Setting Flags");
                if (!File.Exists("0:\\Flags.flg"))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText("0:\\Flags.flg"))
                    {
                        sw.Write("2");
                    }
                }
                Console.Write("Press any key to reboot...");
                Console.ReadKey();
                Power.ACPIReboot();
            }
            catch(Exception ex)
            {
                CrashHandler CH = new CrashHandler();
                CH.Whoops(ex);
            }
        }
    }
}
