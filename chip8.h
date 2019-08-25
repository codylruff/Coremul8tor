
class chip8{

private:
   struct OpCode 
    {
        char OpId;
        u_int16 NNN;
        char NN;
        char X, Y, N;
     };
    std::map<char,void (*op)> operations;
    void LoadOperations()
    {
                operations[0x0] = ClearOrReturn
                operations[0x1] = Jump
                operations[0x2] = CallSubroutine
                operations[0x3] = SkipIfXEqual
                operations[0x4] = SkipIfXNotEqual
                operations[0x5] = SkipIfXEqualY
                operations[0x6] = SetX
                operations[0x7] = AddX
                operations[0x8] = Arithmetic
                operations[0x9] = SkipIfXNotEqualY
                operations[0xA] = SetI
                operations[0xB] = JumpWithOffSet
                operations[0xC] =Rnd},
                {0xD,DrawSprite},
                {0xE,SkipOnKey},
                {0xF,Execute_0xF}
    };
public:
   

};

