
class chip8{

private:
   struct OpCode 
    {
        char OpId;
        u_int16 NNN;
        char NN;
        char X, Y, N;
     };
public:
    std::vector<char> Memory;
    const u_int16* start = 0x200;
    const u_int16* end;
    std::vector<char> V;
    u_int16 I;
    u_int16 PC;
    Stack stack;
        #endregion
 
        #region I/O
        public byte soundTimer;
        public byte delayTimer;
        public ushort keyPressed;
        public ushort[] sprites;

};

