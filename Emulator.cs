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
        public ushort[] Memory;
        public byte[] Registers;
        public byte[] V = {0x0,0x1,0x2,0x3,0x4,0x5,0x6,0x7,0x8,0x9,0xA,0xB,0xC,0xD,0xE,0xF};
        public ushort I;
        public ushort PC;
        public Stack Stack;
        #endregion

        #region I/O
        public ushort keyPressed;
        public ushort[] sprites;
        #endregion

        #region Execution
        public byte soundTimer;
        public byte delayTimer;
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
            LoadProgram(romData);
            var Stack = new Stack();
            soundTimer = 0;
            delayTimer = 0;
            PC = 0;
            
        }
        #endregion

        #region Methods
        public void RunProgram()
        {
            ushort nextInstruction;
            for (ushort i = 0x200; i<0xFFF;i++)
            {
                PC = i;
                nextInstruction = Memory[PC];
                ExecuteOpCode(nextInstruction);
            }
        }
        private void BuildDictionary()
        {
            opcodes.Add(0x0,Execute_0x0);
            opcodes.Add(0x1,Execute_0x1);
            opcodes.Add(0x2,Execute_0x2);
            opcodes.Add(0x3,Execute_0x3);
            opcodes.Add(0x4,Execute_0x4);
            opcodes.Add(0x5,Execute_0x5);
            opcodes.Add(0x6,Execute_0x6);
            opcodes.Add(0x7,Execute_0x7);
            opcodes.Add(0x8,Execute_0x8);
            opcodes.Add(0x9,Execute_0x9);
            opcodes.Add(0xA,Execute_0xA);
            opcodes.Add(0xB,Execute_0xB);
            opcodes.Add(0xC,Execute_0xC);
            opcodes.Add(0xD,Execute_0xD);
            opcodes.Add(0xE,Execute_0xE);
            opcodes.Add(0xF,Execute_0xF);
        }
        private void LoadProgram(byte[] romData)
        {
            // Load romdata into memory before execution
            for (ushort i = 0x200; i<0xFFF;i++)
            {
                Memory[i] = romData[i];
                i++;
            }
            Console.WriteLine("Rom loaded succesfully.");
        }
        private void ExecuteOpCode(ushort opCode)
        {
            OpCodeData data;
            data.OpCode = opCode;
            data.OpId = (byte)(opCode & 0xF000 >> 3);
            data.NNN = (UInt16)(opCode & 0x0FFF);
            data.NN = (byte)(opCode & 0x0FF0 >> 1);
            data.X = (byte)(opCode & 0x0F00 >> 2);
            data.Y = (byte)(opCode & 0x00F0 >> 1);
            data.N = (byte)(opCode & 0x000F);
            opcodes[data.OpId].DynamicInvoke(data);
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
            ExecuteOpCode(Memory[PC]);
        }
        private void Execute_0x0(OpCodeData data)
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
        private void Execute_0x1(OpCodeData data)
        {
            // 1NNN	Jump to address NNN
            PC = data.NNN;
        }
        private void Execute_0x2(OpCodeData data)
        {
            // 2NNN	Execute subroutine starting at address NNN
            Execute_Subroutine(Memory[data.NNN]);
        }
        private void Execute_0x3(OpCodeData data)
        {
            // 3XNN	Skip the following instruction 
            // if the value of register VX equals NN
            
            if (V[data.X] == data.NN)
            {
                // TODO: Skip next instruction
            }
        }
        private void Execute_0x4(OpCodeData data)
        {
            // 4XNN	Skip the following instruction if the value of 
            // register VX is not equal to NN

            if (V[data.X] == data.NN)
            {
                // TODO: Skip next instruction
            }
        }
        private void Execute_0x5(OpCodeData data)
        {
            // 5XY0	Skip the following instruction if the 
            // value of register VX is equal to the value of register VY

            if (V[data.X] == V[data.Y])
            {
                // TODO: Skip next instruction
            }
        }
        private void Execute_0x6(OpCodeData data)
        {
            // 6XNN	Store number NN in register VX
            V[data.X] = data.NN;
        }
        private void Execute_0x7(OpCodeData data)
        {
            // 7XNN	Add the value NN to register VX
            V[data.X] += data.NN;
        }
        private void Execute_0x8(OpCodeData data)
        {
            ushort sum, diff;
            // 8XY0	Store the value of register VY in register VX
            switch(data.N)
            {
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
                case 4:
                    sum = (byte)(V[data.X] + V[data.Y]);
                    V[data.X] += V[data.Y];
                    if(sum > ushort.MaxValue)
                    {
                        V[0xF] = 0x1;
                        break;
                    }else
                    {
                        V[0xF] = 0x0;
                        break;
                    }
                // 8XY5	Subtract the value of register VY from register VX
                // Set VF to 00 if a borrow occurs
                // Set VF to 01 if a borrow does not occur
                case 5:
                    diff = (byte)(V[data.X] - V[data.Y]);
                    V[data.X] -= V[data.Y];
                    if(diff < ushort.MinValue)
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
                    byte leastSig = 0x0; // TODO: determine least significant bit
                    V[data.X] = (byte)(V[data.Y] >> 1);
                    V[0xF] = leastSig;
                    break;
                // 8XY7 Set register VX to the value of VY minus VX
                // Set VF to 00 if a borrow occurs
                // Set VF to 01 if a borrow does not occur
                case 7:
                    diff = (byte)(V[data.X] - (V[data.Y] - V[data.X]));
                    V[data.X] -= (byte)(V[data.Y] - V[data.X]);
                    if(diff < ushort.MinValue)
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
                    byte mostSig = 0x1; // TODO: determine most significant bit
                    V[data.X] = (byte)(V[data.Y] << 1);
                    V[0xF] = mostSig;
                    break;
                default:
                    // TODO: Implement default functionality
                    break;
            }
        }
        private void Execute_0x9(OpCodeData data)
        {
            
            // 9XY0	Skip the following instruction if the value of register VX is
            // not equal to the value of register VY
            if(V[data.X] != V[data.Y])
            {
                // TODO: Skip next instruction
            }

        }
        private void Execute_0xA(OpCodeData data)
        {
            // ANNN	Store memory address NNN in register I
            I = data.NNN;
        }
        private void Execute_0xB(OpCodeData data)
        {
            // BNNN	Jump to address NNN + V0
            PC = (ushort)(data.NNN + V[0x0]);
        }
        private void Execute_0xC(OpCodeData data)
        {
            // CXNN	Set VX to a random number with a mask of NN

            // TODO: Create random number and mask with NN
            ushort rand = 0x100; // place holder
            V[data.X] = (byte)(rand & 0xFF0); // not masked correctly
        }
        private void Execute_0xD(OpCodeData data)
        {
            // DXYN	Draw a sprite at position VX, VY 
            // with N bytes of sprite data starting at the address stored in I
            // Set VF to 01 if any set pixels are changed to unset, and 00 otherwise

            // TODO: Draw sprites to screen
        }
        private void Execute_0xE(OpCodeData data)
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