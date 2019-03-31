using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CHIP_8_Emulator
{
    public class Emulator 
    {
        #region Memory
        // Memory Map:
        // +---------------+= 0xFFF (4095) End of Chip-8 RAM
        // |               |
        // |               |
        // |               |
        // |               |
        // |               |
        // | 0x200 to 0xFFF|
        // |     Chip-8    |
        // | Program / Data|
        // |     Space     |
        // |               |
        // |               |
        // |               |
        // +- - - - - - - -+= 0x600 (1536) Start of ETI 660 Chip-8 programs
        // |               |
        // |               |
        // |               |
        // +---------------+= 0x200 (512) Start of most Chip-8 programs
        // | 0x000 to 0x1FF|
        // | Reserved for  |
        // |  interpreter  |
        // +---------------+= 0x000 (0) Start of Chip-8 RAM
        public byte[] Memory;
        public const ushort startAddress = 0x200;
        public ushort endAddress;
        public byte[] V;
        public ushort I;
        public ushort PC;
        public Stack Stack;
        #endregion

        #region I/O
        public byte soundTimer;
        public byte delayTimer;
        public ushort keyPressed;
        public ushort[] sprites;
        #endregion

        #region Execution
        struct OpCodeData
        {
            public ushort OpCode; 
            public byte OpId; // Ex. Line = 45A2, opCode = 4
            public ushort NNN; // NNN = 5A2
            public byte NN; // NN = 5A
            public byte X, Y, N; // X = 5, Y = A, N = 2
        }
        Dictionary<byte,Action<OpCodeData>> opcodes;
        #endregion

        #region Constructor
        public Emulator(byte[] romData)
        {
            BuildDictionary();
            Memory = new byte[0xFFF];
            V = new byte[0xF];
            LoadProgram(romData);
            Stack = new Stack();
            soundTimer = 0;
            delayTimer = 0;
        }
        #endregion

        #region Operations
        private void LoadProgram(byte[] romData)
        {
            PC = startAddress;
            endAddress = (ushort)romData.Length;
            // Load romdata into memory before execution
            for (ushort i = startAddress; i < endAddress; i++)
            {
                Console.WriteLine(romData[i].ToString());
                Memory[i] = romData[i];
            }
            Console.WriteLine("Rom loaded succesfully.");
        }

        public void RunProgram()
        {
            ushort nextInstruction;
	    delayTimer = 60;
	    soundTimer = 60;
            for (ushort i = startAddress; i < endAddress; i += 2)
            {
                nextInstruction = (ushort)((Memory[PC] << 8) | Memory[PC+1]);
                ExecuteOpCode(nextInstruction);
		delayTimer--;
		soundTimer--;
                PC += 2;
            }
        }
        private void BuildDictionary()
        {
           opcodes = new Dictionary<byte, Action<OpCodeData>>
           {
                {0x0,ClearOrReturn},
                {0x1,Jump},
                {0x2,CallSubroutine},
                {0x3,SkipIfXEqual},
                {0x4,SkipIfXNotEqual},
                {0x5,SkipIfXEqualY},
                {0x6,SetX},
                {0x7,AddX},
                {0x8,Arithmetic},
                {0x9,SkipIfXNotEqualY},
                {0xA,SetI},
                {0xB,JumpWithOffSet},
                {0xC,Rnd},
                {0xD,DrawSprite},
                {0xE,SkipOnKey},
                {0xF,Execute_0xF}
            };
        }
        
        private void ExecuteOpCode(ushort opCode)
        {
            var data = new OpCodeData()
            {
                OpCode = opCode,
                OpId = (byte)(opCode & 0xF000 >> 0xC),
                NNN = (UInt16)(opCode & 0x0FFF),
                NN = (byte)(opCode & 0x00FF),
                N = (byte)(opCode & 0x000F),
                X = (byte)(opCode & 0x0F00 >> 8),
                Y = (byte)(opCode & 0x00F0 >> 4)
            };
            // Loop up the OpCode using the first nibble and execute.
	        opcodes[data.OpId](data);
        }
        private void Execute_Subroutine(ushort address)
        {
            // TODO: Implement subroutine functionality
            // push current address on stack
            Stack.Push(PC);
            ExecuteOpCode(Memory[address]);
        }
        private void Execute_ClearScreen()
        {
            // TODO: Implement clear screen function
        }
        private void Execute_ReturnFromSubroutine()
        {
            // 00EE	Return from a subroutine
            // TODO: Execute address at top of stack
            PC = (ushort)Stack.Pop();
        }
        private void ClearOrReturn(OpCodeData data)
        {
            switch(data.OpCode)
            {
                case 0x00E0:
                    // 00E0: Clear the screen
                    Execute_ClearScreen();
                    break;
                case 0x00EE:
                    Execute_ReturnFromSubroutine();
                    break;
                default:
                    // 0NNN: Execute machine language subroutine at address NNN
                    Execute_Subroutine(Memory[data.NNN]);
                    break;
            }          
        }
        private void Jump(OpCodeData data)
        {
            // 1NNN	Jump to address NNN
            PC = data.NNN;
        }
        private void CallSubroutine(OpCodeData data)
        {
            // 2NNN	Execute subroutine starting at address NNN
            Execute_Subroutine(Memory[data.NNN]);
        }
        private void SkipIfXEqual(OpCodeData data)
        {
            // 3XNN	Skip the following instruction 
            // if the value of register VX equals NN
            
            if (V[data.X] == data.NN)
            {
                PC += 2;
            }
        }
        private void SkipIfXNotEqual(OpCodeData data)
        {
            // 4XNN	Skip the following instruction if the value of 
            // register VX is not equal to NN

            if (V[data.X] != data.NN)
            {
                PC += 2;
            }
        }
        private void SkipIfXEqualY(OpCodeData data)
        {
            // 5XY0	Skip the following instruction if the 
            // value of register VX is equal to the value of register VY

            if (V[data.X] == V[data.Y])
            {
                PC += 2;
            }
        }
        private void SetX(OpCodeData data)
        {
            // 6XNN	Store number NN in register VX
            V[data.X] = data.NN;
        }
        private void AddX(OpCodeData data)
        {
            // 7XNN	Add the value NN to register VX
            V[data.X] += data.NN;
        }
        private void Arithmetic(OpCodeData data)
        {
            ushort sum, diff;
            
            switch(data.N)
            {
                // 8XY0	Store the value of register VY in register VX
                case 0:
                    V[data.X] = V[data.Y];
                    break;
                // 8XY1	Set VX to VX OR VY
                case 1:
                    V[data.X] = (byte)(V[data.X] | V[data.Y]);
                    break;
                // 8XY2	Set VX to VX AND VY
                case 2:
                    V[data.X] = (byte)(V[data.X] & V[data.Y]);
                    break;
                // 8XY3	Set VX to VX XOR VY
                case 3:
                    V[data.X] = (byte)(V[data.X] ^ V[data.Y]);
                    break;
                // 8XY4 Add the value of register VY to register VX
                // Set VF to 01 if a carry occurs
                // Set VF to 00 if a carry does not occur
                case 4:
                    sum = (byte)(V[data.X] + V[data.Y]);
                    
                    if(sum > byte.MaxValue)
                    {
                        V[data.X] = 0xFF; 
                        V[0xF] = 0x1;
                        break;
                    }else
                    {
                        V[data.X] += V[data.Y];
                        V[0xF] = 0x0;
                        break;
                    }
                // 8XY5	Subtract the value of register VY from register VX
                // Set VF to 00 if a borrow occurs
                // Set VF to 01 if a borrow does not occur
                case 5:
                    diff = (byte)(V[data.X] - V[data.Y]);
                    V[data.X] -= V[data.Y];
                    if(diff < byte.MinValue)
                    {
                        V[0xF] = 0x0;
                        break;
                    }else
                    {
                        V[0xF] = 0x1;
                        break;
                    }
                // 8XY6	Store the value of register VY shifted right one bit in register VX
                // Set register VF to the least significant bit prior to the shift
                case 6:
                    byte leastSig = (byte)(V[data.Y] & 1);
                    V[data.X] = (byte)(V[data.Y] >> 1);
                    V[0xF] = leastSig;
                    break;
                // 8XY7 Set register VX to the value of VY minus VX
                // Set VF to 00 if a borrow occurs
                // Set VF to 01 if a borrow does not occur
                case 7:
                    diff = (byte)(V[data.X] - (V[data.Y] - V[data.X]));
                    V[data.X] -= (byte)(V[data.Y] - V[data.X]);
                    if(diff < byte.MinValue)
                    {
                        V[0xF] = 0x0;
                        break;
                    }else
                    {
                        V[0xF] = 0x1;
                        break;
                    }
                // 8XYE Store the value of register VY shifted left one bit in register VX
                // Set register VF to the most significant bit prior to the shift
                case 8:
                    byte mostSig = (byte)(V[data.Y] & 0x8); // TODO: determine most significant bit
                    V[data.X] = (byte)(V[data.Y] << 1);
                    V[0xF] = mostSig;
                    break;
                default:
                    // TODO: Implement default functionality
                    break;
            }
        }
        private void SkipIfXNotEqualY(OpCodeData data)
        {
            
            // 9XY0	Skip the following instruction if the value of register VX is
            // not equal to the value of register VY
            if(V[data.X] != V[data.Y])
            {
                PC += 2;
            }

        }
        private void SetI(OpCodeData data)
        {
            // ANNN	Store memory address NNN in register I
            I = data.NNN;
        }
        private void JumpWithOffSet(OpCodeData data)
        {
            // BNNN	Jump to address NNN + V0
            PC = (ushort)(data.NNN + V[0x0]);
        }
        private void Rnd(OpCodeData data)
        {
            // CXNN	Set VX to a random number with a mask of NN

            // Create random number and mask with NN
            Random rand = new Random();
            byte[] b = new byte[10];
            rand.NextBytes(b);
            V[data.X] = (byte)(b[1] & data.NN); // this may not be right
        }
        private void DrawSprite(OpCodeData data)
        {
            // DXYN	Draw a sprite at position VX, VY 
            // with N bytes of sprite data starting at the address stored in I
            // Set VF to 01 if any set pixels are changed to unset, and 00 otherwise

            // TODO: Draw sprites to screen
        }
        private void SkipOnKey(OpCodeData data)
        {
            // EX9E	Skip the following instruction if the key corresponding
            // to the hex value currently stored in register VX is pressed

            // TODO: Implement input functionality
            if(keyPressed == V[data.X])
            {
                // Skip next instruction
            }
        }
        private void Execute_0xF(OpCodeData data)
        {
            switch(data.NN)
            {
            // FX07	Store the current value of the delay timer in register VX
            case 0x07:
                V[data.X] = (byte)delayTimer;
                break;
            // FX0A	Wait for a keypress and store the result in register VX
            case 0x0A:
                // TODO: no idea how to do this yet.
                break;
            // FX15	Set the delay timer to the value of register VX
            case 0x15:
                delayTimer = V[data.X];
                break;
            // FX18	Set the sound timer to the value of register VX
            case 0x18:
                soundTimer = V[data.X];
                break;
            // FX1E	Add the value stored in register VX to register I
            case 0x1E:
                I += V[data.X];
                break;
            // FX29	Set I to the memory address of the sprite data 
            // corresponding to the hexadecimal digit stored in register VX
            case 0x29:
                I = sprites[V[data.X]];
                break;
            // FX33	Store the binary-coded decimal equivalent of the value
            // stored in register VX at addresses I, I+1, and I+2
            case 0x33:
                Memory[I] = (byte)V[data.X]; 
                Memory[I+1] = (byte)V[data.X];
                Memory[I+2] = (byte)V[data.X];
                break;
            // FX55	Store the values of registers V0 to VX inclusive in memory
            // starting at address I, I is set to I + X + 1 after operation
            case 0x55:
                // TODO: ????
                break;
            // FX65	Fill registers V0 to VX inclusive with the values stored in 
            // memory starting at address I, I is set to I + X + 1 after operation
            case 0x65:
                // TODO: ????
                break;
            default:
                // TODO: implement default case
                break;
            }
        }
        #endregion
    }
}
