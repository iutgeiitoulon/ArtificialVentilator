#ifndef IO_H
#define	IO_H

/*******************************************************************************
 * LED PORT MAPPING
 ******************************************************************************/
#define LED_BLANCHE _LATA7                         //Led Blanche
#define LED_ROUGE _LATA10                        //Led Orange
#define LED_EMETTEUR _LATC7
#define STEP _LATC6
#define DIR _LATA9
#define ELECTROVANNE _LATB5
#define SERVO1 _LATC4
#define SERVO2 _LATC3
#define FIN_COURSE1 PORTBbits.RB3
#define FIN_COURSE2 PORTBbits.RB2

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