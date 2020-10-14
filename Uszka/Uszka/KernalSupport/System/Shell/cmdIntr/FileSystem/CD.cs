/*
* PROJECT:          Aura Operating System Development
* CONTENT:          Command Interpreter - CD
* PROGRAMMER(S):    John Welsh <djlw78@gmail.com>
*/

using System.IO;
using L = Aura_OS.System.Translation;
namespace Aura_OS.System.Shell.cmdIntr.FileSystem
{
    class CD
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
        public CD() { }

        /// <summary>
        /// c = commnad, c_CD
        /// </summary>
        /// <param name="cd">The directory you wish to pass in</param>
        /// <param name="startIndex">The start index for remove.</param>
        /// <param name="count">The count index for remove.</param>
        public static void c_CD(string cd, short startIndex = 0, short count = 3)
        {
            string dir = cd.Remove(startIndex, count);
            try
            {
                if(dir == "..")
                {
                    Directory.SetCurrentDirectory(Uszka.Kernel.cd);
                    var root = Uszka.Kernel.fs.GetDirectory(Uszka.Kernel.cd);
                    if (Uszka.Kernel.cd == Uszka.Kernel.current_volume)
                    {
                    }
                    else
                    {
                        Uszka.Kernel.cd = root.mParent.mFullPath;
                    }
                }
                else if (dir == Uszka.Kernel.current_volume)
                { 
                    Uszka.Kernel.cd = Uszka.Kernel.current_volume;
                }
                else
                {
                    if (Directory.Exists(Uszka.Kernel.cd + dir))
                    {
                        Directory.SetCurrentDirectory(Uszka.Kernel.cd);
                        Uszka.Kernel.cd = Uszka.Kernel.cd + dir + @"\";
                    }
                    else if (File.Exists(Uszka.Kernel.cd + dir))
                    {
                        L.Text.Display("errorthisisafile");
                    }
                    else
                    {
                        L.Text.Display("directorydoesntexist");
                    }
                }                
            } catch { }
        }
    }
}
