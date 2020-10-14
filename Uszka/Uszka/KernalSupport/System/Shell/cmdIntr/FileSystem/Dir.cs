/*
* PROJECT:          Aura Operating System Development
* CONTENT:          Command Interpreter - Dir
* PROGRAMMER(S):    John Welsh <djlw78@gmail.com>
*/

using System;
using System.IO;
using L = Aura_OS.System.Translation;

namespace Aura_OS.System.Shell.cmdIntr.FileSystem
{
    class Dir
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
        public Dir() { }

        /// <summary>
        /// c = commnad, c_Dir
        /// </summary>
        public static void c_Dir()
        {
            DirectoryListing.DispDirectories(Uszka.Kernel.cd);
            DirectoryListing.DispFiles(Uszka.Kernel.cd);
            Console.WriteLine();
        }

        /// <summary>
        /// c = command, c_Dir
        /// </summary>
        /// <param name="dir">The directory path that you wish to pass in</param>
        public static void c_Dir(string dir)
        {
            string directory;

            //args commands
            Char cmdargschar = ' ';
            string[] cmdargs = dir.Split(cmdargschar);

            if (!cmdargs[1].StartsWith("-"))
            {
                directory = cmdargs[1];

                if (Directory.Exists(Uszka.Kernel.cd + directory))
                {
                    DirectoryListing.DispDirectories(Uszka.Kernel.cd + directory);
                    DirectoryListing.DispFiles(Uszka.Kernel.cd + directory);
                }
            }

            else
            {
                if (cmdargs[1].Equals("-a"))
                {
                    DirectoryListing.DispDirectories(Uszka.Kernel.cd);
                    DirectoryListing.DispHiddenFiles(Uszka.Kernel.cd);

                    if (cmdargs.Length == 3)
                    {
                        directory = cmdargs[2];

                        DirectoryListing.DispDirectories(Uszka.Kernel.cd + directory);
                        DirectoryListing.DispHiddenFiles(Uszka.Kernel.cd + directory);
                    }
                }
                else
                {
                    L.Text.Display("invalidargument");
                }
            }

            Console.WriteLine();
        }
    }
}
