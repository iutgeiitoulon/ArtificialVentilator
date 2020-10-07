
#include <p33FJ128MC804.h>


void InitOC1(void)
{
    OC1CONbits.OCTSEL=0;            //Select Timer2 Clock
    OC1CONbits.OCM=0b000;           //Disable OC Module
    OC1R=10;
}

void OC1GeneratePulse()
{
    OC1CONbits.OCM=0b010;           //Active high one shot
}

