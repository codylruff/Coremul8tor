fn main() {
    let mut memory[u8; 4096];
    let cpu = Processor{pc: 0x200, opcode: 0, i: 0, sp: 0};

    println!("{}", cpu.pc);
    println!("{}", cpu.opcode);
    println!("{}", cpu.i);
    println!("{}", cpu.sp);
}

struct Processor {
    pc: u16,
    opcode: u16,
    i: u16,
    sp: u16
}

fn fetch(pc: u16) -> u16 {
   
}

fn decode(opcode: u16){

}