#ifndef IO_H
#define	IO_H

/*******************************************************************************
 * LED PORT MAPPING
 ******************************************************************************/
#define LED_BLANCHE _LATA7                         //Led Blanche
#define LED_ROUGE _LATA10                        //Led Orange
#define nRESET _LATA1
#define STEP1 _LATC6
#define DIR1 _LATA9
#define STEP2 _LATC3
#define DIR2 _LATB5
#define STEP3 _LATB14
#define DIR3 _LATB15
#define SERVO1 _LATC4
#define FIN_COURSE1 PORTBbits.RB3
#define FIN_COURSE2 PORTBbits.RB2
#define FIN_COURSE3 PORTCbits.RC1
#define FIN_COURSE4 PORTCbits.RC0
#define FIN_COURSE5 PORTBbits.RB4
#define FIN_COURSE6 PORTCbits.RC2

#define BUTTON_START FIN_COURSE2        //A remplacer par 2 pour le respi 1 moteur
#define BUTTON_RESET FIN_COURSE4

#define SW1 1
#define SW2 2

#define I2C_SCLtris TRISBbits.TRISB8
#define I2C_SDAtris TRISBbits.TRISB9
#define _I2C_SCL _LATB8
#define _I2C_SDA _LATB9
#define I2C_SCLpin 8
#define I2C_SDApin 9

/*******************************************************************************
 * Prototypes fonctions
 ******************************************************************************/
void InitIO();
void LockIO();
void UnlockIO();
void InitCN(void);
#endif	/* IO_H */