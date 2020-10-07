#include <p33FJ128MC804.h>
#include "adc.h"

unsigned char ADCResultIndex = 0;
static volatile unsigned int ADCResult[5];
unsigned char ADCConversionFinishedFlag;

//Configuration ADC

void InitADC1(void) {
    //AD1CON1
    AD1CON1bits.ADON = 0;
    AD1CON1bits.AD12B = 1;
    AD1CON1bits.FORM = 0b00;
    AD1CON1bits.ASAM = 0;
    AD1CON1bits.SSRC = 0b111;

    //AD1CON2
    AD1CON2bits.VCFG = 0b000;
    AD1CON2bits.CSCNA = 1;      //Scan inputs
    AD1CON2bits.SMPI = 4;

    //AD1CON3
    AD1CON3bits.ADRC = 0;
    AD1CON3bits.SAMC = 0b11111;
    AD1CON3bits.ADCS = 0b0000000;

    //Configuration des ports
    //AD1PCFGLbits.PCFG0 = 0;
    //AD1PCFGLbits.PCFG1 = 0;
    AD1PCFGLbits.PCFG6 = 0;
    AD1PCFGLbits.PCFG7 = 0;
    AD1PCFGLbits.PCFG8 = 0;

    //AD1CSSLbits.CSS0 = 1;
    //AD1CSSLbits.CSS1 = 1;
    AD1CSSLbits.CSS6 = 1;
    AD1CSSLbits.CSS7 = 1;
    AD1CSSLbits.CSS8 = 1;

    IFS0bits.AD1IF = 0;
    IEC0bits.AD1IE = 1;
    AD1CON1bits.ADON = 1;
}

//ADC Interrupt Routine
void __attribute__((interrupt, no_auto_psv)) _ADC1Interrupt(void)
{
    IFS0bits.AD1IF=0;
    ADCResult[ADCResultIndex]=ADC1BUF0;
    if(ADCResultIndex<4)
        ADCResultIndex++;
    else
    {
        ADCResultIndex=0;
        ADCConversionFinishedFlag=1;
    }
}

void ADC1StartConversionSequence()
{
    AD1CON1bits.SAMP=1;
}

unsigned int * ADCGetResult(void)
{
    return ADCResult;
}

unsigned char ADCIsConversionFinished(void)
{
    return ADCConversionFinishedFlag;
}

void ADCClearConversionFinishedFlag(void)
{
    ADCConversionFinishedFlag=0;
}
