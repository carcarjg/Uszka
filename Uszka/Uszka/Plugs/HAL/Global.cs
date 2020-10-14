﻿/*
* PROJECT:          Aura Operating System Development
* CONTENT:          Plug of Cosmos.HAL.Global
* PROGRAMMERS:      Valentin Charbonnier <valentinbreiz@gmail.com>
*/

using System;
using IL2CPU.API.Attribs;
using Cosmos.HAL;
using Cosmos.Core;

namespace Aura_Plugs.HAL
{

    [Plug(Target = typeof(Cosmos.HAL.Global))]
    public static class Global
    {

        static public void Init(TextScreenBase textScreen)
        {

            Console.WriteLine("[Uszka Operating System v" + Uszka.Kernel.version + " - Made by ST.Gloriana Development]");
            Aura_OS.System.CustomConsole.WriteLineInfo("Starting Cosmos kernel...");

            PCI.Setup();
            Aura_OS.System.CustomConsole.WriteLineOK("PCI Devices Scan");

            ACPI.Start();
            Aura_OS.System.CustomConsole.WriteLineOK("ACPI Initialization");

            /*Cosmos.HAL.BlockDevice.IDE.InitDriver();
            Aura_OS.System.CustomConsole.WriteLineOK("IDE Driver Initialization");

            Cosmos.HAL.BlockDevice.AHCI.InitDriver();
            Aura_OS.System.CustomConsole.WriteLineOK("AHCI Driver Initialization");*/

            Cosmos.HAL.Global.PS2Controller.Initialize();
            Aura_OS.System.CustomConsole.WriteLineOK("PS/2 Controller Initialization");

            //Cosmos.Core.Processing.ProcessorScheduler.Initialize();
            //Aura_OS.System.CustomConsole.WriteLineOK("Processor Scheduler Initialization");

            Aura_OS.System.CustomConsole.WriteLineOK("Kernel successfully initialized!");

        }
    }
}
