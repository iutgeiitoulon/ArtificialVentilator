/* 
 * File:   ADC.h
 * Author: E105-POSTE2
 *
 * Created on 8 septembre 2015, 15:36
 */

#ifndef ADC_H
#define	ADC_H

void InitADC1(void);

void __attribute__((interrupt, no_auto_psv)) _ADC1Interrupt(void);

void ADC1StartConversionSequence();

unsigned int * ADCGetResult(void);

unsigned char ADCIsConversionFinished(void);

void ADCClearConversionFinishedFlag(void);


#endif	/* ADC_H */

