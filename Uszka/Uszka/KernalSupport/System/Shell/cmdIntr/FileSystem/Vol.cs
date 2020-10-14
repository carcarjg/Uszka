﻿/*
* PROJECT:          Aura Operating System Development
* CONTENT:          Command Interpreter - Vol
* PROGRAMMER(S):    John Welsh <djlw78@gmail.com>
*/

using System;
using L = Aura_OS.System.Translation;

namespace Aura_OS.System.Shell.cmdIntr.FileSystem
{
    class Vol
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
        public Vol() { }

        /// <summary>
        /// c = command, c_Vol
        /// </summary>
        public static void c_Vol()
        {
            var vols = Uszka.Kernel.fs.GetVolumes();

            L.Text.Display("volCommand");

            foreach (var vol in vols)
            {
                if (vol.mName == Uszka.Kernel.current_volume && vols.Count > 1)
                {
                    Console.WriteLine(" >" + vol.mName + "\t   \t" + Uszka.Kernel.fs.GetFileSystemType(vol.mName) + " \t" + vol.mSize + " MB\t" + vol.mParent);
                }
                else
                {
                    Console.WriteLine("  " + vol.mName + "\t   \t" + Uszka.Kernel.fs.GetFileSystemType(vol.mName) + " \t" + vol.mSize + " MB\t" + vol.mParent);
                }
            }

        }
    }
}
