#include <p33FJ128MC804.h>
#include "timer.h"
#include "oscillator.h"
#include "IO.h"
#include "main.h"
#include "PWM.h"
#include "Toolbox.h"
#include "OutputCompare.h"
/*******************************************************************************
 * TIMER1                            
 *******************************************************************************/
//Initialisation d'un timer 16 bits
int subdiv=200;
void InitTimer1(void)
{
    //Timer1 pour horodater les mesures (1ms)
    T1CONbits.TON = 0; // Disable Timer
    T1CONbits.TSIDL = 0; //continue in idle mode
    T1CONbits.TGATE = 0; //Accumulation disabled
    T1CONbits.TCS = 0; //clock source = internal clock
    SetFreqTimer1(10);

    IFS0bits.T1IF = 0; // Clear Timer Interrupt Flag
    IEC0bits.T1IE = 1; // Enable Timer interrupt
    T1CONbits.TON = 1; // Enable Timer
}

void SetFreqTimer1(float freq)
{
    T1CONbits.TCKPS = 0b00; //00 = 1:1 prescaler value
    if (FCY / freq > 65535)
    {
        T1CONbits.TCKPS = 0b01; //01 = 1:8 prescaler value
        if (FCY / freq / 8 > 65535)
        {
            T1CONbits.TCKPS = 0b10; //10 = 1:64 prescaler value
            if (FCY / freq / 64 > 65535)
            {
                T1CONbits.TCKPS = 0b11; //11 = 1:256 prescaler value
                PR1 = (int) (FCY / freq / 256);
            }
            else
                PR1 = (int) (FCY / freq / 64);
        }
        else
            PR1 = (int) (FCY / freq / 8);
    }
    else
        PR1 = (int) (FCY / freq);
}

//Interruption du timer 1
double vitesseMoteurPAP=0;
double subDiv=0;
int subdiv2=0;
unsigned char sens=0;
void __attribute__((interrupt, no_auto_psv)) _T1Interrupt(void)
{
    IFS0bits.T1IF = 0;
    subdiv2++;
    if(subdiv2>=10)
    {
        subdiv2=0;
    }

}


/*******************************************************************************
 * TIMER2                            
 *******************************************************************************/
//Initialisation d'un timer 16 bits

void InitTimer2(void)
{
    //Timer2
    T2CONbits.TON = 0; // Disable Timer
    T2CONbits.TSIDL = 0; //continue in idle mode
    T2CONbits.TGATE = 0; //Accumulation disabled
    T2CONbits.TCS = 0; //clock source = internal clock
    SetFreqTimer2(1000);

    IFS0bits.T2IF = 0; // Clear Timer Interrupt Flag
    IEC0bits.T2IE = 1; // Enable Timer interrupt
    T2CONbits.TON = 1; // Enable Timer
}

void SetFreqTimer2(float freq)
{
    T2CONbits.TCKPS = 0b00; //00 = 1:1 prescaler value
    if (FCY / freq > 65535)
    {
        T2CONbits.TCKPS = 0b01; //01 = 1:8 prescaler value
        if (FCY / freq / 8 > 65535)
        {
            T2CONbits.TCKPS = 0b10; //10 = 1:64 prescaler value
            if (FCY / freq / 64 > 65535)
            {
                T2CONbits.TCKPS = 0b11; //11 = 1:256 prescaler value
                PR2 = (int) (FCY / freq / 256);
            }
            else
                PR2 = (int) (FCY / freq / 64);
        }
        else
            PR2 = (int) (FCY / freq / 8);
    }
    else
        PR2 = (int) (FCY / freq);
}

int countTimer2 = 0;
//Interruption du timer 2

double cpt=0;
double amplitudeMax=1000;
double amplitudeMin=0;
void __attribute__((interrupt, no_auto_psv)) _T2Interrupt(void)
{
    IFS0bits.T2IF = 0;

    if(sens)
    {
        //Si on es dans le sens positif
        OC1GeneratePulse(); //On genere un pulse
        cpt++;              //On incremente le compteur
        
        if(cpt>=amplitudeMax)
        {
            //Si on a atteint l'amplitude Max, on change le sens
            DIR=!DIR;
            sens=0;
        }
    }
    else
    {
        //Si on es dans le sens negatif
        OC1GeneratePulse(); //On genere un pulse
        cpt--;              //On decremente le compteur
        
        if(cpt<=amplitudeMin)
        {
            //Si on a atteint l'amplitude Min, on change le sens
            DIR=!DIR;
            sens=1;
        }
    }
}


/*******************************************************************************
 * TIMER2                            
 *******************************************************************************/
//Initialisation d'un timer 16 bits

void InitTimer3(void)
{
    //Timer3
    T3CONbits.TON = 0; // Disable Timer
    T3CONbits.TSIDL = 0; //continue in idle mode
    T3CONbits.TGATE = 0; //Accumulation disabled
    T3CONbits.TCS = 0; //clock source = internal clock
    SetFreqTimer3(50);

    IFS0bits.T3IF = 0; // Clear Timer Interrupt Flag
    IEC0bits.T3IE = 1; // Enable Timer interrupt
    T3CONbits.TON = 1; // Enable Timer
}

void SetFreqTimer3(float freq)
{
    T3CONbits.TCKPS = 0b00; //00 = 1:1 prescaler value
    if (FCY / freq > 65535)
    {
        T3CONbits.TCKPS = 0b01; //01 = 1:8 prescaler value
        if (FCY / freq / 8 > 65535)
        {
            T3CONbits.TCKPS = 0b10; //10 = 1:64 prescaler value
            if (FCY / freq / 64 > 65535)
            {
                T3CONbits.TCKPS = 0b11; //11 = 1:256 prescaler value
                PR3 = (int) (FCY / freq / 256);
            }
            else
                PR3 = (int) (FCY / freq / 64);
        }
        else
            PR3 = (int) (FCY / freq / 8);
    }
    else
        PR3 = (int) (FCY / freq);
}

//Interruption du timer 1

void __attribute__((interrupt, no_auto_psv)) _T3Interrupt(void)
{
    IFS0bits.T3IF = 0;
}


/*******************************************************************************
 * TIMER 4                            
 *******************************************************************************/
//Initialisation d'un timer 16 bits

void InitTimer4(void)
{
    //Timer1 pour horodater les mesures (1ms)
    T4CONbits.TON = 0; // Disable Timer
    T4CONbits.TSIDL = 0; //continue in idle mode
    T4CONbits.TGATE = 0; //Accumulation disabled
    T4CONbits.TCS = 0; //clock source = internal clock
    SetFreqTimer4(1000);

    IFS1bits.T4IF = 0; // Clear Timer Interrupt Flag
    IEC1bits.T4IE = 1; // Enable Timer interrupt
    T4CONbits.TON = 1; // Enable Timer
}

void SetFreqTimer4(float freq)
{
    T4CONbits.TCKPS = 0b00; //00 = 1:1 prescaler value
    if (FCY / freq > 65535)
    {
        T4CONbits.TCKPS = 0b01; //01 = 1:8 prescaler value
        if (FCY / freq / 8 > 65535)
        {
            T4CONbits.TCKPS = 0b10; //10 = 1:64 prescaler value
            if (FCY / freq / 64 > 65535)
            {
                T4CONbits.TCKPS = 0b11; //11 = 1:256 prescaler value
                PR4 = (int) (FCY / freq / 256);
            }
            else
                PR4 = (int) (FCY / freq / 64);
        }
        else
            PR4 = (int) (FCY / freq / 8);
    }
    else
        PR4 = (int) (FCY / freq);
}

unsigned long timestamp;
//Interruption du timer 4

void __attribute__((interrupt, no_auto_psv)) _T4Interrupt(void)
{
    IFS1bits.T4IF = 0;
    timestamp++;

}
