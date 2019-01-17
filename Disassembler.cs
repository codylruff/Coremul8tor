using System;
using System.Collections;
using System.Text;

namespace CHIP_8_Emulator
{
    public static class Disassembler
    {
        public static void DisassembleChip8(byte[] codebuffer, int codeLength)
        {
            for (int i = 0; i < codeLength; i++)
            {
                string codeString = codebuffer[i].ToString() + codebuffer[i+1].ToString();
                
                try
                {
                    ushort opcode = ushort.Parse(codeString);
                    ParseCode(opcode);
                }
                catch(Exception ex)
                {
                    byte opcode = byte.Parse(codeString);
                    ParseCode(opcode);
                    Console.Error.WriteLine($"Error: {ex.Message} {0:X4}",codeString);
                }
                finally
                {
                    i++;
                }
            }   
        }
        private static void ParseCode(ushort opcode)
        {
            // Decode opcode
            switch (opcode & 0xF000)
            {
                case 0x0000:
                    switch (opcode & 0x00FF)
                    {
                        case 0x00E0: // 00E0: Clear screen
                            Console.WriteLine("00E0 - {0:X4}",opcode);
                            break;
                        case 0x00EE: // 00EE: Subroutine return
                            Console.WriteLine("00EE - {0:X4}",opcode);
                            break;
                        default:
                            Console.WriteLine("{0:X4}",opcode);
                            break;
                    }
                    break;
                case 0x1000: // 1NNN: Jump to address NNN
                    Console.WriteLine("1NNN - {0:X4}",opcode);
                    break;
                case 0x2000: // 2NNN: Call subroutine at address NNN
                    Console.WriteLine("2NNN - {0:X4}",opcode);
                    break;
                case 0x3000: // 3XNN: Skip next instruction if VX == NN
                    Console.WriteLine("3XNN - {0:X4}",opcode);
                    break;
                case 0x4000: // 4XNN: Skip next instruction if VX != NN
                    Console.WriteLine("4XNN - {0:X4}",opcode);
                    break;
                case 0x5000: // 5XY0: Skip next instruction if VX == VY
                    Console.WriteLine("5XY0 - {0:X4}",opcode);
                    break;
                case 0x6000: // 6XNN: Set VX to NN
                    Console.WriteLine("6XNN - {0:X4}",opcode);
                    break;
                case 0x7000: // 7XNN: Add NN to VX (no carry flag)
                    Console.WriteLine("7XNN - {0:X4}",opcode);
                    break;
                case 0x8000: 
                    switch (opcode & 0x000F)
                    {
                        case 0x0000: // 8XY0: Set VX to VY
                            Console.WriteLine("8XY0 - {0:X4}",opcode);
                            break;
                        case 0x0001: // 8XY1: Set VX to VX or VY
                            Console.WriteLine("8XY1 - {0:X4}",opcode);
                            break;
                        case 0x0002: // 8XY2: Set VX to VX and VY
                            Console.WriteLine("8XY2 - {0:X4}",opcode);
                            break;
                        case 0x0003: // 8XY3: Set VX to VX xor VY
                            Console.WriteLine("8XY3 - {0:X4}",opcode);
                            break;
                        case 0x0004: // 8XY4: Add VY to VX, VF = 1 when carry, 0 otherwise
                            Console.WriteLine("8XY4 - {0:X4}",opcode);
                            break;
                        case 0x0005: // 8XY5: Subtract VY from VX, VF = 1 when not borrow
                            Console.WriteLine("8XY5 - {0:X4}",opcode);
                            break;
                        case 0x0006: // 8XY6: Set VX = VY >> 1, (if quirk set VX = VX >> 1), VF stores least significant bit
                            Console.WriteLine("8XY6 - {0:X4}",opcode);
                            break;
                        case 0x0007: // 8XY7: Set VX to VY - VX, VF = 1 when not borrow
                            Console.WriteLine("{0:X4}",opcode);
                            break;
                        case 0x000E: // 8XYE: Set VX = VY << 1, (if quirk set VX = VX << 1), VF stores most significant bit
                            Console.WriteLine("8XY7 - {0:X4}",opcode);
                            break;
                        default:
                            break;
                    }
                    break;
                case 0x9000: // 9XY0: Skip next instruction if VX != VY
                    Console.WriteLine("9XY0 - {0:X4}",opcode);
                    break;
                case 0xA000: // ANNN: Set I to NNN
                    Console.WriteLine("ANNN - {0:X4}",opcode);
                    break;
                case 0xB000: // ANNN: Jump to address V0 + NNN
                    Console.WriteLine("ANNN - {0:X4}",opcode);
                    break;
                case 0xC000: // CXNN: Set VX to random number & NN
                    Console.WriteLine("CXNN - {0:X4}",opcode);
                    break;
                case 0xD000: // DXYN: Draw sprite stored in I with height N to coordinates (VX, VY)
                    Console.WriteLine("DXYN - {0:X4}",opcode);
                    break;
                case 0xE000:
                    switch (opcode & 0x00FF)
                    {
                        case 0x009E: // EX9E: Skip next instruction if key in VX is pressed
                            Console.WriteLine("EX9E - {0:X4}",opcode);
                            break;
                        case 0x00A1: // EXA1: Skip next instruction if key in VX is not pressed
                            Console.WriteLine("EXA1 - {0:X4}",opcode);
                            break;
                        default:
                            Console.WriteLine("{0:X4}",opcode);
                            break;
                    }
                    break;
                case 0xF000:
                    switch (opcode & 0x00FF)
                    {
                        case 0x0007: // FX07: Set VX to delay timer
                            Console.WriteLine("FX07 - {0:X4}",opcode);
                            break;
                        case 0x000A: // FX0A: Wait for keypress, store in VX
                            {
                                Console.WriteLine("FX0A - {0:X4}",opcode);
                                break;
                            }
                        case 0x0015: // FX15: Set delay timer to VX
                            Console.WriteLine("FX15 - {0:X4}",opcode);
                            break;
                        case 0x0018: // FX18: Set sound timer to VX
                            Console.WriteLine("FX18 - {0:X4}",opcode);
                            break;
                        case 0x001E: // FX1E: Add VX to I
                            Console.WriteLine("FX1E - {0:X4}",opcode);
                            break;
                        case 0x0029: // FX29: Set I to address for character in VX
                            Console.WriteLine("FX29 - {0:X4}",opcode);
                            break;
                        case 0x0033: // FX33: Store binary coded decimal in VX to memory at I through I+2
                            {
                                Console.WriteLine("FX33 - {0:X4}",opcode);
                            }
                            break;
                        case 0x0055: // FX55: Store V0 through VX into memory at I (if not quirk, also set I = I + X + 1)
                            Console.WriteLine("FX55 - {0:X4}",opcode);
                            break;
                        case 0x0065: // FX65: Fill V0 through VX from memory at I (if not quirk, also set I = I + X + 1)
                            Console.WriteLine("FX65 - {0:X4}",opcode);
                            break;
                        default:
                            Console.WriteLine("{0:X4}",opcode);
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("{0:X4}",opcode);
                    break;
            }
        }
    }
}