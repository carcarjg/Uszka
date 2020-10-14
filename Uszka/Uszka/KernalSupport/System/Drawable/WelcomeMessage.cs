/*
* PROJECT:          Aura Operating System Development
* CONTENT:          Welcome Message
* PROGRAMMERS:      Alexy DA CRUZ <dacruzalexy@gmail.com>
*                   Valentin Charbonnier <valentinbreiz@gmail.com>
*/

using System;

namespace Aura_OS.System
{
    class WelcomeMessage
    {

        /// <summary>
        /// Display the welcome message
        /// </summary>
        public static void Display()
        {
            Logo.Print();
            Console.ForegroundColor = ConsoleColor.Green;
            switch (Uszka.Kernel.langSelected)
            {
                case "fr_FR":
                    Console.WriteLine(" * Documentation not available");
                    break;

                case "en_US":
                    Console.WriteLine(" * Documentation not available");
                    break;

                case "nl_NL":
                    Console.WriteLine(" * Documentatie not available");
                    break;

                case "it_IT":
                    Console.WriteLine(" * Documentazione not available");
                    break;
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ");
        }

    }
}
