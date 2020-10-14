﻿/*
* PROJECT:          Aura Operating System Development
* CONTENT:          Command Interpreter - Logout
* PROGRAMMER(S):    John Welsh <djlw78@gmail.com>
*/

using System;
using System.IO;

namespace Aura_OS.System.Shell.cmdIntr.Settings
{
    class Logout
    {
        private static string HelpInfo = "";

        /// <summary>
        /// Getter and Setters for Help Info.
        /// </summary>
        public static string HI
        {
            get { return HelpInfo; }
            set { HelpInfo = value; /*PUSHED OUT VALUE (in)*/}
        }

        /// <summary>
        /// Empty constructor. (Good for debug)
        /// </summary>
        public Logout() { }

        /// <summary>
        /// c = command, c_Logout
        /// </summary>
        public static void c_Logout()
        {
            Uszka.Kernel.Logged = false;
            Uszka.Kernel.userLevelLogged = "";
            Uszka.Kernel.userLogged = "";
            Directory.SetCurrentDirectory(Uszka.Kernel.cd);
            Uszka.Kernel.cd = Uszka.Kernel.current_volume;
            Console.Clear();
        }
    }
}
