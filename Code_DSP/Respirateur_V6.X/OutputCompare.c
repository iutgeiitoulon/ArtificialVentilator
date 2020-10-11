
#include <p33FJ128MC804.h>


void InitOC1(void)
{
    OC1CONbits.OCTSEL=0;            //Select Timer2 Clock
    OC1CONbits.OCM=0b000;           //Disable OC Module
    OC1R=10;
}
void InitOC2(void)
{
    OC2CONbits.OCTSEL=0;            //Select Timer2 Clock
    OC2CONbits.OCM=0b000;           //Disable OC Module
    OC2R=10;
}
void InitOC3(void)
{
    OC3CONbits.OCTSEL=0;            //Select Timer2 Clock
    OC3CONbits.OCM=0b000;           //Disable OC Module
    OC3R=10;
}

void OC1GeneratePulse()
{
    OC1CONbits.OCM=0b010;           //Active high one shot
}
void OC2GeneratePulse()
{
    OC2CONbits.OCM=0b010;           //Active high one shot
}
void OC3GeneratePulse()
{
    OC3CONbits.OCM=0b010;           //Active high one shot
}

