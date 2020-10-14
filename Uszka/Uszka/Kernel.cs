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
using Uszka_OS;
using Aura_OS.System;
using Aura_OS.System.Users;
using Aura_OS.System.Computer;
using Aura_OS.System.Utils;
using Aura_OS.Apps.System;
using Aura_OS.System.Network.IPV4;
using Aura_OS.System.Graphics;
using Aura_OS.System.Shell.cmdIntr;
using Cosmos.System.ExtendedASCII;
using Lang = Aura_OS.System.Translation;

namespace Uszka
{
    public class Kernel : Sys.Kernel
    {
        public static Sys.FileSystem.CosmosVFS fs = new Sys.FileSystem.CosmosVFS();
        Drivers.DisplayDriver DD = new DisplayDriver();
        char[] Flags;
        GraphicsManager GM;
        public static Boolean graphicsMode = false;
        private static Boolean fsMode = false;
        public static Boolean newGraphics = false;
        public static Boolean enableFs = false;
        public static String cd = @"0:\";
        public static bool running;
        public static string version = "0.0.1";
        public static string revision = "000000002601";
        public static string langSelected = "en_US";
        public static string userLogged;
        public static Aura_OS.HAL.PCSpeaker speaker = new Aura_OS.HAL.PCSpeaker();
        public static string userLevelLogged;
        public static bool Logged = false;
        public static string ComputerName = "Uszka-Private-Alpha";
        public static string UserDir = @"0:\Users\" + userLogged + "\\";
        public static bool SystemExists = false;
        public static bool JustInstalled = false;
        public static Config LocalNetworkConfig;
        public static Aura_OS.System.AConsole.Console AConsole;
        public static string current_volume = @"0:\";
        public static Debugger debugger;
        public static Dictionary<string, string> environmentvariables = new Dictionary<string, string>();

        protected override void BeforeRun()
        {
            CommandManager.RegisterAllCommands();
            //AConsole = new System.Shell.VGA.VGAConsole(null);
            Encoding.RegisterProvider(CosmosEncodingProvider.Instance);

            Console.InputEncoding = Encoding.GetEncoding(437);
            Console.OutputEncoding = Encoding.GetEncoding(437);
            Console.Clear();
            enableFs = true;
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            printLogoConsole();
            Console.Write("Detected Drives: ");
            Console.WriteLine(DriveInfo.GetDrives().Length);
            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                Console.WriteLine(" - " + d.Name + " (" + d.GetType() + ") " + (d.TotalSize / 1048576) + "MB");
            }
            WaitSeconds(5);

            PCSpeaker.Beep(850);
            startupchecks();


            
        }
        protected override void Run()
        {
            try
            {
                //Sys.Thread TBAR = new Sys.Thread(TaskBar);
                while (running)
                {
                    if (Logged) //If logged
                    {
                        //TBAR.Start();
                        BeforeCommand();

                        AConsole.writecommand = true;

                        var cmd = Console.ReadLine();
                        //TBAR.Stop();
                        CommandManager._CommandManger(cmd);
                        //Console.WriteLine();

                    }
                    else
                    {
                        Login.LoginForm();
                    }
                }
            }
            catch (Exception ex)
            {
                CrashHandler CH = new CrashHandler();
                CH.Whoops(ex);
            }
        }

        public static void BeforeCommand()
        {
            if (cd == Kernel.current_volume)
            {

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(UserLevel.TypeUser());

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(userLogged);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("@");

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(ComputerName);

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("> ");

                Console.ForegroundColor = ConsoleColor.White;

            }
            else
            {

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(UserLevel.TypeUser());

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(userLogged);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("@");

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(ComputerName);

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("> ");

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(cd + "~ ");

                Console.ForegroundColor = ConsoleColor.White;

            }
        }

        public static bool ContainsVolumes()
        {
            var vols = fs.GetVolumes();
            foreach (var vol in vols)
            {
                return true;
            }
            return false;
        }
        void startupchecks()
        {
            try
            {
                //New stuff
                Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
                if (ContainsVolumes())
                {
                    Aura_OS.System.CustomConsole.WriteLineOK("FileSystem Registration");
                }
                else
                {
                    Aura_OS.System.CustomConsole.WriteLineError("FileSystem Registration");
                }

                NetworkInit.Init();

                Aura_OS.System.CustomConsole.WriteLineOK("Uszka-OS successfully started!");
                Aura_OS.System.Setup SU = new Setup();
                SU.InitSetup();

                if (SystemExists)
                {
                    if (!JustInstalled)
                    {

                        Settings config = new Settings(@"0:\System\settings.conf");
                        langSelected = config.GetValue("language");

                        #region Language

                        Lang.Keyboard.Init();

                        #endregion

                        Info.getComputerName();

                        Aura_OS.System.Network.NetworkInterfaces.Init();

                        running = true;

                    }
                }
                else
                {
                    running = true;
                }
            }
            catch (Exception ex) 
            {
                CrashHandler CH = new CrashHandler();
                CH.Whoops(ex);
            }

            #region Obsolete
            /*if (!File.Exists(@"0:\fs.cfg"))
            {
                printLogoConsole();

                Console.WriteLine(@"The filesystem was not formatted with Uszka OS, so it cannot be used.");
                Console.WriteLine(@"Would you like to format it? (y/n)");
                Console.WriteLine("WARNING: THIS WILL DELETE ALL DATA.\n");
                if (Console.ReadLine() == "y")
                {
                    Console.WriteLine("\nFormatting...");
                    try
                    {
                        fs.Format(@"0:\", "FAT32", true);
                        FileStream writeStream = File.Create(@"0:\fs.cfg");
                        byte[] toWrite = Encoding.ASCII.GetBytes("true");
                        writeStream.Write(toWrite, 0, toWrite.Length);
                        writeStream.Close();
                    }
                    catch
                    {
                        deathScreen("0x0100 No Hard Drive to format!");
                    }
                }
                else
                {
                    Console.WriteLine("\nThe filesystem is being disabled as it was not formatted with Uszka OS.");
                    enableFs = false;
                    WaitSeconds(2);
                }

                if (File.Exists("0:\\Flags.flg"))
                {
                    Flags = new char[99];
                    Flags = File.ReadAllText("0:\\Flags.flg").ToCharArray();
                    Flags[1] = '1';//Force GFX mode
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
                        catch (Exception ex)
                        {
                            CrashHandler CH = new CrashHandler();
                            CH.Whoops(ex);
                        }
                        DD.Startup();
                        //GM.start();
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
                    Power.ACPIReboot();
                }
                else
                {
                    File.ReadAllText("0:\\Uszaka32\\System.cx");
                }
            }*/
            #endregion
        }

        private static void printLogoConsole()
        {
            Console.WriteLine(@"  _    _  _____ _______  __                ____   _____ ");
            Console.WriteLine(@" | |  | |/ ____|___  / |/ /    /\         / __ \ / ____|");
            Console.WriteLine(@" | |  | | (___    / /| ' /    /  \ ______| |  | | (___  ");
            Console.WriteLine(@" | |  | |\___ \  / / |  <    / /\ \______| |  | |\___ \ ");
            Console.WriteLine(@" | |__| |____) |/ /__| . \  / ____ \     | |__| |____) |");
            Console.WriteLine(@"  \____/|_____//_____|_|\_\/_/    \_\     \____/|_____/ ");
            Console.WriteLine(@"                                                        ");
            Console.WriteLine(@"               ST. Gloriana Development                 ");
            Console.WriteLine(@"                     V0.0.1 Alpha                       ");
        }
        public static void shutdown(Boolean reboot)
        {
            if (reboot) Cosmos.System.Power.Reboot();
            else Cosmos.System.Power.Shutdown();
        }
        private static void processFSConsole(String input)
        {
            if (input == "exit")
            {
                fsMode = false;
                return;
            }

            var directory_list = fs.GetDirectoryListing(cd);
            if (input == "ls")
            {
                foreach (var directoryEntry in directory_list)
                {
                    if (directoryEntry.mEntryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.File)
                    {
                        Console.WriteLine(" - " + directoryEntry.mName);
                    }
                    else
                    {
                        Console.WriteLine(directoryEntry.mName);
                    }
                }
            }
            else if (input.Split(" ")[0] == "cd")
            {
                String[] inputSplit = input.Split(" ");
                if (inputSplit.Length > 1)
                {
                    Console.WriteLine("go");
                    String[] pieces = input.Split(new[] { ',' }, 2);
                    Console.WriteLine("go 2");
                    if (Directory.Exists(pieces[1]))
                    {
                        Console.WriteLine("Exists");
                    }
                    else
                    {
                        Console.WriteLine("Not Exists");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid syntax! cd [dir] ");
                }
            }
            else if (input.Split(" ")[0] == "mkdir")
            {
                String[] inputSplit = input.Split(" ");
                if (inputSplit.Length > 1)
                {
                    String dir = "";
                    for (int i = 1; i < inputSplit.Length; i++)
                    {
                        dir = dir + inputSplit[i];
                    }
                    fs.CreateDirectory(cd + dir);
                    Console.WriteLine("Success!");
                }
                else
                {
                    Console.WriteLine("Invalid syntax! mkdir [name] ");
                }
            }
            else if (input.Split(" ")[0] == "delfile")
            {
                String[] inputSplit = input.Split(" ");
                if (inputSplit.Length == 2)
                {
                    File.Delete(@"0:\" + inputSplit[1]);
                    Console.WriteLine("Success!");
                }
                else
                {
                    Console.WriteLine("Invalid Syntax!");
                }
            }
            else if (input.Split(" ")[0] == "rdfile")
            {
                String[] inputSplit = input.Split(" ");
                if (inputSplit.Length == 2)
                {
                    FileStream s = File.OpenRead(inputSplit[1]);
                    byte[] a = new byte[s.Length];
                    s.Read(a, 0, a.Length);
                    foreach (Byte b in a)
                    {
                        Console.Write((char)b);
                    }
                    Console.Write("\n");
                }
                else
                {
                    Console.WriteLine("Invalid Syntax!");
                }
            }
            else
            {
                Console.WriteLine("Unknown");
            }
        }
        public static void WaitSeconds(int secNum)
        {
            int StartSec = Cosmos.HAL.RTC.Second;
            int EndSec;
            if (StartSec + secNum > 59)
            {
                EndSec = 0;
            }
            else
            {
                EndSec = StartSec + secNum;
            }
            while (Cosmos.HAL.RTC.Second != EndSec) { }
        }
        public static void deathScreen(String error)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Clear();
            int sec = 20;
            while (sec > 0)
            {
                Console.WriteLine("Your system was disabled due to an internal error. Please see the error message below and contact us on GitHub if you don't know what happened.");
                Console.WriteLine();
                Console.WriteLine(error);
                Console.WriteLine();
                Console.WriteLine("Your system will automatically reboot in " + sec + " seconds, or press any key to restart now.");
                sec--;
                WaitSeconds(1);
                if (System.Console.KeyAvailable == true)
                {
                    shutdown(true);
                }
                Console.Clear();
            }
            shutdown(true);
        }
        private void processConsole(String input)
        {
            if (input == "gui")
            {
                return;
            }
            else if (input == "cpu")
            {
                return;
            }
            else if (input == "mem")
            {
                Console.WriteLine("RAM: " + (Cosmos.Core.CPU.GetAmountOfRAM() < 1024 ? Cosmos.Core.CPU.GetAmountOfRAM() + " MB" : Cosmos.Core.CPU.GetAmountOfRAM() / 1024.00 + " GB"));
                return;
            }
            else if (input == "time")
            {
                Console.WriteLine("Current Date: " + Cosmos.HAL.RTC.Month + "/" + Cosmos.HAL.RTC.DayOfTheMonth + "/" + Cosmos.HAL.RTC.Year + " " + Cosmos.HAL.RTC.Hour + ":" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second.ToString().PadLeft(2, '0'));
                return;
            }
            else if (input == "help")
            {
                Console.WriteLine("Help:");
                Console.WriteLine("    mem: Returns Memory size.");
                Console.WriteLine("    time: Returns current time according to the BIOS.");
                Console.WriteLine("    help: Returns this message.");
                return;
            }
            else if (input == "miv")
            {
            }
            else if (input == "fs")
            {
                if (enableFs)
                {
                    fsMode = true;
                }
                else
                {
                    Console.WriteLine("FS is not enabled! Please restart and select 'y' when prompted!");
                }

                return;
            }
            else if (input == "shutdown")
            {
                shutdown(false);
            }
            else if (input == "restart")
            {
                shutdown(true);
            }
            else
            {
                Console.WriteLine("Unknown command! Use 'help' for help!");
                return;
            }
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
