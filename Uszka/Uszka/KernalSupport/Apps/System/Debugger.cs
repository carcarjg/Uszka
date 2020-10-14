﻿/*
* PROJECT:          Aura Operating System Development
* CONTENT:          Debugger using TCP!
* PROGRAMMERS:      Valentin Charbonnier <valentinbreiz@gmail.com>
*/

using Aura_OS.System.Network.IPV4;
using System;
using System.Text;
using Aura_OS.System.Network.IPV4.TCP;

namespace Aura_OS.Apps.System
{
    public class Debugger
    {

        public static Cosmos.Debug.Kernel.Debugger debugger = new Cosmos.Debug.Kernel.Debugger("aura", "debugger");

        TCPClient xClient;

        public bool enabled = false;

        public int port;
        public Address ip;

        public Debugger(Address IP, int Port)
        {
            ip = IP;
            port = Port;
        }

        public void Start()
        {
            xClient = new TCPClient(port);
            xClient.Connect(ip, port);
            enabled = true;

            Send("--- Aura Debugger v0.2 ---");
            Send("Connected!");
            debugger.Send("Debugger started!");
        }

        public void Send(string message)
        {
            debugger.Send(message);
            if (enabled)
            {
                xClient.Send(Encoding.ASCII.GetBytes("[" + Aura_OS.System.Time.TimeString(true, true, true) + "] - " + message));
            }
        }

        internal void Stop()
        {
            if (enabled)
            {
                xClient.Send(Encoding.ASCII.GetBytes("[" + Aura_OS.System.Time.TimeString(true, true, true) + "] - Properly disconnected by the operating system!"));
                xClient.Close();
                Uszka.Kernel.debugger.enabled = false;
            }
            else
            {
                Console.WriteLine("Debugger already disabled!");
            }
        }
    }

    public class DebuggerSettings
    {

        /// <summary>
        /// Settings of the debugger
        /// </summary>
        public static void RegisterSetting()
        {

            //HAL.SaveScreen.SaveCurrentScreen();

            string result;

            if (Uszka.Kernel.debugger.enabled)
            {
                result = DispSettingsDialog(true);
            }
            else
            {
                result = DispSettingsDialog(false);
            }

            //HAL.SaveScreen.PushLastScreen();

            if (result.Equals("on"))
            {
                if (Uszka.Kernel.debugger == null)
                {
                    Uszka.Kernel.debugger = new Debugger(new Address(192, 168, 1, 73), 4224);
                }
                Console.WriteLine("Starting debugger at: " + Uszka.Kernel.debugger.ip.ToString() + ":" + Uszka.Kernel.debugger.port);
                Uszka.Kernel.debugger.Start();
                Console.WriteLine("Debugger started!");
            }
            else if (result.Equals("off"))
            {
                if (Uszka.Kernel.debugger != null)
                {
                    if (!Uszka.Kernel.debugger.enabled)
                    {
                        Console.WriteLine("Debugger already disabled!");
                    }
                    else
                    {
                        Uszka.Kernel.debugger.Stop();
                        Console.WriteLine("Debugger disabled!");
                    }
                }
            }
            else if (result.Equals("changeip"))
            {

                string ip = VOIDIP();

                if (Aura_OS.System.Utils.Misc.IsIpv4Address(ip))
                {
                    if (Uszka.Kernel.debugger != null)
                    {
                        if (Uszka.Kernel.debugger.enabled)
                        {
                            Uszka.Kernel.debugger.Stop();
                        }
                    }
                    Uszka.Kernel.debugger = new Debugger(Address.Parse(ip), 4224);
                }
                else
                {
                    Aura_OS.System.Drawable.Menu.DispErrorDialog("It is not an IP address!");
                    RegisterSetting();
                }
            }
        }

        private static string VOIDIP()
        {
            if (Uszka.Kernel.debugger != null)
            {
                return Aura_OS.System.Drawable.Menu.DispDialogOneArg("Change IP address (currently " + Uszka.Kernel.debugger.ip.ToString() + ")", "IP Address: ");
            }
            else
            {
                return Aura_OS.System.Drawable.Menu.DispDialogOneArg("Change IP address", "IP Address: ");
            }
        }

        public static int x_;
        public static int y_;

        /// <summary>
        /// Display settings dialog
        /// </summary>
        public static string DispSettingsDialog(bool enabled)
        {
            int x = (Uszka.Kernel.AConsole.Width / 2) - (64 / 2);
            int y = (Uszka.Kernel.AConsole.Height / 2) - (10 / 2);
            x_ = x;
            y_ = y;
            SettingMenu(x, y, enabled);
            string[] item = { "Enable", "Disable", "Change IP address" };
            int settings = Aura_OS.System.Drawable.Menu.GenericMenu(item, Settings, x, y);
            if (settings == 0)
            {
                return "on";
            }
            else if (settings == 1)
            {
                return "off";
            }
            else if (settings == 2)
            {
                return "changeip";
            }
            else
            {
                return "off";
            }
        }

        static int x_lang = Console.CursorLeft;
        static int y_lang = Console.CursorTop;

        static void SettingMenu(int x, int y, bool enabled)
        {
            Console.Clear();

            Console.BackgroundColor = ConsoleColor.DarkBlue;

            Console.SetCursorPosition(x, y);
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.SetCursorPosition(x_lang, y_lang);
            Console.SetCursorPosition(x, y + 1);
            if (enabled)
            {
                Console.WriteLine("║ Enable or disable TCP debugger: (currently enabled)          ║");
            }
            else
            {
                Console.WriteLine("║ Enable or disable TCP debugger: (currently disabled)         ║");
            }
            Console.SetCursorPosition(x_lang, y_lang);
            Console.SetCursorPosition(x, y + 2);
            Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");

            Console.SetCursorPosition(x, y + 3);
            Console.WriteLine("║                                                              ║");
            Console.SetCursorPosition(x_lang, y_lang);

            Console.SetCursorPosition(x, y + 4);
            Console.WriteLine("║                                                              ║");
            Console.SetCursorPosition(x_lang, y_lang);

            Console.SetCursorPosition(x, y + 5);
            Console.WriteLine("║                                                              ║");
            Console.SetCursorPosition(x_lang, y_lang);

            Console.SetCursorPosition(x, y + 6);
            Console.WriteLine("║                                                              ║");
            Console.SetCursorPosition(x_lang, y_lang);

            Console.SetCursorPosition(x, y + 7);
            Console.WriteLine("║                                                              ║");
            Console.SetCursorPosition(x_lang, y_lang);

            Console.SetCursorPosition(x, y + 8);
            Console.WriteLine("║                                                              ║");
            Console.SetCursorPosition(x_lang, y_lang);

            Console.SetCursorPosition(x, y + 9);
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.SetCursorPosition(x_lang, y_lang);

            Console.BackgroundColor = ConsoleColor.Black;
        }

        static void Settings()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;

            Console.SetCursorPosition(x_ + 2, y_ + 3);
            Console.WriteLine(" ");
            Console.SetCursorPosition(x_lang, y_lang);

            Console.SetCursorPosition(x_ + 2, y_ + 4);
            Console.WriteLine(" ");
            Console.SetCursorPosition(x_lang, y_lang);

            Console.SetCursorPosition(x_ + 2, y_ + 5);
            Console.WriteLine(" ");
            Console.SetCursorPosition(x_lang, y_lang);

            Console.SetCursorPosition(x_ + 2, y_ + 6);
            Console.WriteLine(" ");
            Console.SetCursorPosition(x_lang, y_lang);

            Console.SetCursorPosition(x_ + 2, y_ + 7);
            Console.WriteLine(" ");
            Console.SetCursorPosition(x_lang, y_lang);
        }
    }

    public class DebugConsole
    {
        public static void WriteLine(string text)
        {
            Console.WriteLine(text);
            if (Uszka.Kernel.debugger != null)
            {
                if (Uszka.Kernel.debugger.enabled)
                {
                    Uszka.Kernel.debugger.Send(text);
                }
            }
        }

        public static void WriteLine()
        {
            Console.WriteLine();
        }
    } 
}
