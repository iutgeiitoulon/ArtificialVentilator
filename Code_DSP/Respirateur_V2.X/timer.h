#ifndef TIMER_H
#define	TIMER_H

void InitTimer1(void);
void SetFreqTimer1(float freq);
void __attribute__((interrupt, no_auto_psv)) _T1Interrupt(void);
void InitTimer2(void);
void SetFreqTimer2(float freq);
void __attribute__((interrupt, no_auto_psv)) _T2Interrupt(void);
void InitTimer3(void);
void SetFreqTimer3(float freq);
void __attribute__((interrupt, no_auto_psv)) _T3Interrupt(void);
void InitTimer4(void);
void SetFreqTimer4(float freq);
void __attribute__((interrupt, no_auto_psv)) _T4Interrupt(void);

#endif	/* TIMER_H */