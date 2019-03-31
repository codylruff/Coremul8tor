using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CHIP_8_Emulator
{
    class Program
    {
        static void Main(string[] args)
        {
            
            try
            {
                // Parse args
                Emulator chip8;
                bool DisassemblerEnabled = false;
                string romFile = @"C:\Users\codyl\source\repos\codylruff\Coremul8tor\Roms\Invaders.ch8";

                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i].ToLower())
                    {
                        case "-d":
                            DisassemblerEnabled = true;
                            break;
                        default:
                            romFile = args[i];
                            break;
                    }
                }

                if (string.IsNullOrWhiteSpace(romFile)) Usage();

                byte[] romData = File.ReadAllBytes(romFile);
                chip8 = new Emulator(romData);

                if (DisassemblerEnabled)
                {
                    Disassembler.DisassembleChip8(romData, romData.Length);
                }else
                {
                    Console.WriteLine("Starting Emulator . . . ");
                    chip8.RunProgram();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
        }
        private static void Usage()
        {
                Console.WriteLine("Help:");
                Console.WriteLine();
                Console.WriteLine("CHIP_8_Emulator.exe [options] RomFile");
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine("-d   Run the disassembler on the RomFile");
                Console.WriteLine();
        }
    }
}
