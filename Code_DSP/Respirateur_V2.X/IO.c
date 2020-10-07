#include "IO.h"
#include <p33FJ128MC804.h>

void InitIO() {
    // IMPORTANT : désactive les entrées analogiques, sinon on perd les entrées numériques (Pin remapable)
    AD1PCFGL=0xFFFF;

    //*************************************************************************/
    // Configuration des sorties
    //*************************************************************************/

    //LED
    _TRISA7 = 0; //Led 1 sur RA7
    _TRISC4=0;      //SERVO1
    
    
    _TRISA9=0;      //DIR1
    _TRISC6=0;      //STEP
    _TRISB5=0;      //DIR2
    _TRISC3=0;      //STEP2
    _TRISB15=0;     //DIR3
    _TRISB14=0;     //STEP3
    _TRISA10=0;     //Led 3 sur RA10
    _TRISA1=0;      //nRESET
    
    
    //*************************************************************************/
    // Configuration des entrées
    //*************************************************************************/
    _TRISB2=1;      //FIN_COURSE2
    _TRISB3=1;      //FIN_COURSE1
    _TRISC1=1;      //FIN_COURSE3
    _TRISC0=1;      //FIN_COURSE4
    _TRISB4=1;      //FIN_COURSE5
    _TRISC2=1;      //FIN_COURSE6
    
    

    //*************************************************************************/
    //Configuration des pins remappables
    //*************************************************************************/
    UnlockIO();
    
    RPOR11bits.RP22R=0b10010;           //Output compare 1 on RP22 (STEP1)
    RPOR9bits.RP19R=0b10011;           //Output compare 2 on RP19 (STEP2)
    RPOR7bits.RP14R=0b10100;           //Output compare 3 on RP14 (STEP3)
    
    RPOR11bits.RP23R=0b00011;           //UART1 TX on SPI_MISO (RP23)
    RPINR18bits.U1RXR=24;               //UART1 RX on SPI_MOSI (RP24) 
    
    
    LockIO();
}

void LockIO()
{
    asm volatile ("mov #OSCCON,w1 \n"
                  "mov #0x46, w2 \n"
                  "mov #0x57, w3 \n"
                  "mov.b w2,[w1] \n"
                  "mov.b w3,[w1] \n"
                  "bset OSCCON, #6":::"w1","w2","w3");
}

void UnlockIO()
{
    asm volatile ("mov #OSCCON,w1 \n"
                  "mov #0x46, w2 \n"
                  "mov #0x57, w3 \n"
                  "mov.b w2,[w1] \n"
                  "mov.b w3,[w1] \n"
                  "bclr OSCCON, #6":::"w1","w2","w3");
}

void InitCN(void)
{
    //Fin de course 1
    CNEN1bits.CN1IE = 0; //Disable interrupt for CN pin
    CNPU1bits.CN1PUE = 1; //Enable Pull-up for CN pin
    CNEN1bits.CN6IE = 0; //Disable interrupt for CN pin
    CNPU1bits.CN6PUE = 1; //Enable Pull-up for CN pin
    CNEN1bits.CN7IE = 0; //Disable interrupt for CN pin
    CNPU1bits.CN7PUE = 1; //Enable Pull-up for CN pin
    CNEN1bits.CN8IE = 0; //Disable interrupt for CN pin
    CNPU1bits.CN8PUE = 1; //Enable Pull-up for CN pin
    CNEN1bits.CN9IE = 0; //Disable interrupt for CN pin
    CNPU1bits.CN9PUE = 1; //Enable Pull-up for CN pin
    CNEN1bits.CN10IE = 0; //Disable interrupt for CN pin
    CNPU1bits.CN10PUE = 1; //Enable Pull-up for CN pin
}