#include "oscillator.h"
#include "p33Fxxxx.h"
#include <stdlib.h>
#include "p33Fxxxx.h"
#include <libpic30.h>

void InitOscillator()
{
    //Fosc = Fin * M / (N1 * N2)
    //Fosc = Fin * (PLLDIV+2) / ((PLLPRE+2)*2*(PLLPOST+1)) 
    //avec une condition : 0.8MHz < Fin / (PLLPRE+2) < 8MHz
    
    //80 = 32 * (18+2) / ((2+2)*2*(0+1))
    
    // Configure PLL prescaler, PLL postscaler, PLL divisor
    PLLFBD=18; //PLLFBD+2 = 20
    CLKDIVbits.PLLPRE = 2; // PLLPRE+2 = 4
    CLKDIVbits.PLLPOST = 0;// 2*(PLLPOST+1) = 2
    
    // Initiate Clock Switch to Primary Oscillator with PLL (NOSC = 0b011)
    __builtin_write_OSCCONH(0x03);
    __builtin_write_OSCCONL(0x01);

    // Wait for Clock switch to occur
    while (OSCCONbits.COSC != 0b011);

    // Wait for PLL to lock
    while(OSCCONbits.LOCK != 1) {};    
}
