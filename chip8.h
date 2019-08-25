
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
public:
   

};

