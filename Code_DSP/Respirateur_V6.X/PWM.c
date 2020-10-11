#include <p33FJ128MC804.h>
#include <stdio.h>
#include "PWM.h"
#include "IO.h"
#include "timer.h"
#include "Toolbox.h"


long timestampSansCollisionDroit = 0;
long timestampSansCollisionGauche = 0;
float moteurCCCurrentSpeed=0;
float moteurPAPCurrentSpeed=0;
float moteurCCSpeedConsigne=0;
float moteurPAPSpeedConsigne=0;
//unsigned char acceleration = 1;
//signed char PWMSpeedConsigne[4] = {0, 0, 0, 0};
//double PWMSpeedCorrectorOutput[4] = {0, 0, 0, 0};

void InitPWM(void)
{
    P1TCONbits.PTMOD = 0b00;
    P1TCONbits.PTCKPS = 0b00;
    P1TCONbits.PTOPS = 0b00;
    P1TPER = 1000;

    PWM1CON1bits.PMOD1 = 1;
    PWM1CON1bits.PMOD2 = 1;
    PWM1CON1bits.PMOD3 = 1;
    PWM1CON2bits.IUE = 1;

    P1OVDCONbits.POVD1H = 1;
    P1OVDCONbits.POVD1L = 1;
    P1OVDCONbits.POVD2H = 1;
    P1OVDCONbits.POVD2L = 1;
    P1OVDCONbits.POVD3H = 1;
    P1OVDCONbits.POVD3L = 1;

    P1TCONbits.PTEN = 1;
}

void InitPWM2(void)
{
    P2TCONbits.PTMOD = 0b00;
    P2TCONbits.PTCKPS = 0b11;                   //FCY/64
    P2TCONbits.PTOPS = 0b00;
    P2TPER = 2500;          //(10pules per sec)

    PWM2CON1bits.PMOD1 = 1;
    PWM2CON2bits.IUE = 1;

    P2OVDCONbits.POVD1H = 1;
    P2OVDCONbits.POVD1L = 1;


    P2TCONbits.PTEN = 1;
}

void PWM2SetSpeedConsigne(double vitesseEnPourcents)
{
    //On borne les valeurs d'entrée des vitesse en pourcentage de la vitesse max.
    vitesseEnPourcents = Min(vitesseEnPourcents, 100);
    vitesseEnPourcents = Max(vitesseEnPourcents, -100);


 
    moteurPAPSpeedConsigne=vitesseEnPourcents;
}

void PWM2UpdatePeriod()
{
    P1DC2 = (unsigned int)(100 - 50);

    //Mise à jour des consignes des hacheurs
    if (moteurPAPSpeedConsigne > 0)
    {
        DIR1=0;
        PWM1CON1bits.PEN2H = 1;
    }
    else
    {
        DIR1=1;
        PWM1CON1bits.PEN2H = 1;
    }
    P2TPER= 2500-(moteurPAPSpeedConsigne*25);
}
    
void PWMSetSpeedConsigne(double vitesseEnPourcents, char moteur)
{
    //On borne les valeurs d'entrée des vitesse en pourcentage de la vitesse max.
    vitesseEnPourcents = Min(vitesseEnPourcents, 100);
    vitesseEnPourcents = Max(vitesseEnPourcents, -100);

    switch(moteur)
    {
        case MOTEUR_CC: moteurCCSpeedConsigne=vitesseEnPourcents;break;
        //case MOTEUR_PAP: moteurPAPSpeedConsigne=vitesseEnPourcents;break;
        default: break;
    }
}

void PWMUpdateSpeed()
{

                //Mise à jour des consignes des hacheurs
                if (moteurCCSpeedConsigne > 0)
                {
                    PWM1CON1bits.PEN1L = 0;

                    PWM1CON1bits.PEN1H = 1;
                }
                else
                {
                    PWM1CON1bits.PEN1H = 0;
                    PWM1CON1bits.PEN1L = 1;
                }
                P1DC1 = (unsigned int)((100 - Abs(moteurCCSpeedConsigne))*20);

//                //Mise à jour des consignes des hacheurs
//                if (moteurPAPSpeedConsigne > 0)
//                {
//                    DIR=0;
//                    PWM1CON1bits.PEN2H = 1;
//                }
//                else
//                {
//                    DIR=1;
//                    PWM1CON1bits.PEN2H = 1;
//                }
//                P1DC2 = (unsigned int)((100 - Abs(moteurPAPSpeedConsigne))*20);

    
}
