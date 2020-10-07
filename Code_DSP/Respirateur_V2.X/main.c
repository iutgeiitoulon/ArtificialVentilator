#include <stdio.h>
#include <stdlib.h>
#include "p33Fxxxx.h"
#include <libpic30.h>
#include <math.h>
#include "pragma_config.h"
#include "main.h"
#include "oscillator.h"
#include "IO.h"
#include "ADC.h"
#include "UTLN_Typedefs.h"
#include "UTLN_Timers.h"
#include "PWM.h"
#include "OutputCompare.h"
#include "UTLN_uart.h"
#include "UTLN_Communication.h"
#include "RespiratorState.h"
#include "Define.h"
#include "ustv_i2c_interrupt.h"
#include "UTLN_D6F-PH.h"
#include "timer.h"
#include "UTLN_Button.h"

#define SENS_MONTEE 0
#define SENS_DESCENTE 1
volatile unsigned long g_longTimeStamp=0;

BOOL boardVersion3Moteur=true;
extern BOOL startRespirator;
typedef enum{
        INIT_0,
    ARRET,

    SOUFFLE, 
    SOUFFLE_EN_COURS,
    VIDAGE, //for bavu only
    VIDAGE_MAINTIEN_BAS,
    VIDAGE_EN_COURS,
    MAINTIEN_PRESSION,
    MAINTIEN_EN_COURS,
    MANUAL
}ETAT;

typedef enum{
    INIT,
    INIT_EN_COURS,
    DESCENTE,        
    DESCENTE_EN_COURS,
    MONTEE,
    MONTEE_EN_COURS,
    ATTENTE,
    ATTENTE_EN_COURS,
}ETATMoteur;

void StateMachineRespirateur3Moteurs(void);
void StateMachineRespirateurBavu(void);
void StateMachineMoteur1(void);
void SetStateMoteur1(ETATMoteur _etat);
void StateMachineMoteur2(void);
void SetStateMoteur2(ETATMoteur _etat);
void StateMachineMoteur3(void);
void SetStateMoteur3(ETATMoteur _etat);
void SetMotorTargetPos(unsigned char motorID, double target, double speed);
void SetMotorTarget(unsigned char motorID, unsigned int target);
void StopMotor(unsigned char motorID);
void InitMachine(void);
void StateMachineButton(void);

void Timer1CallBack(void);
void Timer2CallBack(void);
void Timer3CallBack(void);
void Timer4CallBack(void);
void Timer5CallBack(void);
void ButtonStartCallback(void);
void ButtonResetCallback(void);
void CalculateRespiratorParameters(void);

double surface=0;


volatile ETAT etat=INIT_0;
volatile unsigned long timeStampDebut=0;
volatile unsigned long timeStampHighSpeed=0;
unsigned char flagFinMontee=0;
unsigned char flagFinDescente=0;
unsigned long deadTime=50000;
double freqTimer=0;
unsigned char initMoteur1Done=0;
unsigned char initMoteur2Done=0;
unsigned char initMoteur3Done=0;
double vitesseMaxMoteur=2200;
int main(void)
{
    InitOscillator();

    /****************************************************************************************************/
    // Configuration des entrées sorties
    /****************************************************************************************************/
    InitIO();
    InitCN();           //Initialisation des pullUP (Change Notification)
    
    //InitADC1();
    

    
    RegisterTimerWithCallBack(TIMER1_ID, 100.0, Timer1CallBack, true, 4, 0);//Gestion de la vitesse des pas a pas
    RegisterTimerWithCallBack(TIMER2_ID, 100.0, Timer2CallBack, true, 4, 0);   //Gestion de la vitesse des pas a pas
    RegisterTimerWithCallBack(TIMER3_ID, 1200.0, Timer3CallBack, true, 4, 0);//Gestion de la vitesse des pas a pas
    RegisterTimerWithCallBack(TIMER4_ID, 1000.0, Timer4CallBack, true, 6, 1);//TimeStamp
    RegisterTimerWithCallBack(TIMER5_ID, 50.0, Timer5CallBack, true, 3, 1);//Timer Send values
    InitOC1();
    InitOC2();
    InitOC3();
    initUART1();
                                      
    InitI2C1();
    //D6FHarwareReset();
    __delay_ms(10);
    //D6F_PHInitialize();
    __delay_ms(100);
    LED_BLANCHE = 1;
    LED_ROUGE = 1;
    nRESET=0;       //Desactivation moteurs
    //Parametres par defaut
    respiratorState.amplitude=600;
    respiratorState.attenteBas=1000;
    respiratorState.attenteHaut=1000;
    respiratorState.stepsOffsetDown=0;
    respiratorState.stepsOffsetUp=0;
    respiratorState.positionMoteur1=0;
    respiratorState.positionMoteur2=0;
    respiratorState.positionMoteur3=0;
    respiratorState.vitesse=1300;
    respiratorState.pLimite=2500;      //en Pascals
    respiratorState.vLimite=0.6;        //en L
    respiratorState.periode=2000000;    //(en us)
    respiratorState.cyclesPerMinute=12;
    respiratorState.isAssistanceMode=0;     //Par defaut on est en mode reanimation (cycles auto)
    respiratorState.seuilAssistance=-30;
    
    /****************************************************************************************************/
    // Determination du type de respirateur (1 moteur VS 3 Moteurs)
    /****************************************************************************************************/
    if(FIN_COURSE5==0 && FIN_COURSE6==0)
    {
        boardVersion3Moteur=false;
        surface=7.26/100000;
        CalculateRespiratorParameters();
      //  RegisterTimerWithCallBack(TIMER2_ID, 1000000.0, Timer2CallBack, true, 3, 1);   //Time base pulse OC
        respiratorState.amplitude=600;
        vitesseMaxMoteur=1300;
        ButtonRegisterWithCallBack(SW1, &PORTB, 2, ButtonStartCallback);
        ButtonRegisterWithCallBack(SW2, &PORTC, 0, ButtonResetCallback);
    }
    else
    {
        surface=9.4/100000;
        CalculateRespiratorParameters();
        respiratorState.amplitude=1250;
        ButtonRegisterWithCallBack(SW1, &PORTC, 2, ButtonStartCallback);

        ButtonRegisterWithCallBack(SW2, &PORTC, 0, ButtonResetCallback);

    }
    /****************************************************************************************************/
    // Boucle Principale
    /****************************************************************************************************/
    for (;;)
    {
        //Detection fin course
        if(FIN_COURSE1==0)
        {
             respiratorState.positionMoteur1=0;  //On reset le compteur(et donc la position 0 du moteur)
             LED_BLANCHE=1;
        }
        
        if(ADCIsConversionFinished())
        {
            ADCClearConversionFinishedFlag();
            unsigned int ADCVals[5];
            unsigned int * pValues=ADCGetResult();
            ADCVals[0]=pValues[0];
            ADCVals[1]=pValues[1];
            ADCVals[2]=pValues[2];
            ADCVals[3]=pValues[3];
            ADCVals[4]=pValues[4];
            //respiratorState.pressure1=(ADCVals[0]*(3.3/4096));
            respiratorState.pressure2=(ADCVals[0]*(3.3/4096));
            
            double rho = 1.23;
            //double diametre = 0.023;        //en M
            double diffPression = respiratorState.pressure2-0.08;
            int sign=1;
            if (diffPression < 0)
                sign = -1;
            else
                sign = 1;
            double vitesse=sqrt(2*ABS(diffPression)/rho)*sign;
            respiratorState.debitCourant=vitesse*0.0009;
        }
        

        StateMachineRespirateur3Moteurs();
        StateMachineMoteur1();
        StateMachineMoteur2();
        StateMachineMoteur3();
        StateMachineButton();
    } 
}// fin main


//<editor-fold defaultstate="collapsed" desc="State Machines">
void InitMachine(void)
{
    initMoteur1Done=initMoteur2Done=initMoteur3Done=0;
    etat=INIT_0;
    SetStateMoteur1(INIT);
    SetStateMoteur2(INIT);
    SetStateMoteur3(INIT);
}

unsigned char respiratorStarted=0;
void StateMachineRespirateur3Moteurs(void)
{
    switch(etat)
        {
        case INIT_0:
            nRESET=1;
            if(boardVersion3Moteur)
            {
                if(initMoteur1Done && initMoteur2Done && initMoteur3Done)
                {
                    
                    //nRESET=0;       //Desactivation moteurs
                    etat=ARRET;
                }
            }
            else
            {
                if(initMoteur1Done)
                {
                    SetStateMoteur1(DESCENTE);
                    etat=ARRET;
                }
                    
            }
            break;
            case ARRET:
                if(startRespirator)
                {
                    nRESET=1;
                    respiratorStarted=true;
                    etat=SOUFFLE;                   
                    timeStampDebut=g_longTimeStamp;
                }
                else
                {
                    respiratorStarted=false;
                }
                //Si on a un ordre de deplacement manuel (depuis interface)
                if(respiratorState.flagDoStepsCMD)
                {
                    etat=MANUAL;
                    TurnOnOffTimer(TIMER1_ID,1);

                    nRESET=1;
                }
                break;
            
            //Montee en pression  
            case SOUFFLE:
                if(boardVersion3Moteur)
                    SetStateMoteur1(DESCENTE);
                else
                    SetStateMoteur1(MONTEE);
                SetStateMoteur2(DESCENTE);
                SetStateMoteur3(DESCENTE);
                timeStampDebut=g_longTimeStamp;
                etat=SOUFFLE_EN_COURS;
            break;
            
            case SOUFFLE_EN_COURS:
                if( g_longTimeStamp>=(timeStampDebut+(unsigned long)respiratorState.tempsMontee))
                {
                    LED_BLANCHE=0;
                    LED_ROUGE=0;
                    etat=MAINTIEN_PRESSION;
                }
                if(respiratorState.pressure2>=respiratorState.pLimite || respiratorState.volumeCourant>=respiratorState.vLimite)
                {
                    etat=MAINTIEN_PRESSION;
                }
                if(!startRespirator)
                {
                    etat=ARRET;
                    nRESET=0;       //Desactivation moteurs
                }
                break;
            //Attente en pression
            case MAINTIEN_PRESSION:
                SetStateMoteur1(ATTENTE);
                SetStateMoteur2(ATTENTE);
                SetStateMoteur3(ATTENTE);
                timeStampDebut=g_longTimeStamp;
                etat=MAINTIEN_EN_COURS;
                break;
            case MAINTIEN_EN_COURS:
                if(g_longTimeStamp>=(timeStampDebut+(unsigned long)respiratorState.attenteHaut))
                {
                    //On commence la descente
                    respiratorState.volumeCourant=0;
                    
                    etat=VIDAGE_MAINTIEN_BAS;
                }
                if(!startRespirator)
                {
                    etat=ARRET;
                    nRESET=0;       //Desactivation moteurs
                }
                break;
                
        case VIDAGE_MAINTIEN_BAS:
            if(boardVersion3Moteur)
                SetStateMoteur1(MONTEE);
            else
                SetStateMoteur1(DESCENTE);
            
                SetStateMoteur2(MONTEE);
                SetStateMoteur3(MONTEE);
                timeStampDebut=g_longTimeStamp;
                etat=VIDAGE_EN_COURS;
            break;
            case VIDAGE_EN_COURS:
                if(respiratorState.isAssistanceMode)
                {
                    if((g_longTimeStamp>=timeStampDebut+(unsigned long)respiratorState.attenteBas/2)&&(respiratorState.pressure2<respiratorState.seuilAssistance))
                    {
                        timeStampDebut=g_longTimeStamp;
                        etat=SOUFFLE;
                    }
                }
                else
                {
                    if(g_longTimeStamp>=(timeStampDebut+(unsigned long)respiratorState.attenteBas))
                    {
                        LED_ROUGE=1;
                        //On commence la montee
                        timeStampDebut=g_longTimeStamp;
                        etat=SOUFFLE;
                    }
                }
                if(!startRespirator)
                {
                    etat=ARRET;
                    nRESET=0;       //Desactivation moteurs
                }
                break;                
            case MANUAL:
                if(respiratorState.flagDoStepsCMD==0)
                {
                    etat=ARRET;
                    nRESET=0;       //Desactivation moteurs
                }
                break;
            
            default:break;
        }
}



ETATMoteur etat1=INIT;
void StateMachineMoteur1(void)
{
    switch(etat1)
    {
        //Init
        case INIT:
            SetMotorTargetPos(1, -100000, vitesseMaxMoteur-1000);            //On descend
            etat1=INIT_EN_COURS;
            break;
        //Descente d'init    
        case INIT_EN_COURS:
            if(FIN_COURSE1==0)
            {
                StopMotor(1);       //On arrete le moteur 1
                respiratorState.positionMoteur1=0;           //On reset la position du moteur 1
                etat1=MONTEE;            //On remonte
            }
            break;
        //DESCENTE    
        case DESCENTE:
            SetMotorTargetPos(1, -50, respiratorState.vitesse); //-50 pour descendre au dela du fin de course
            etat1=DESCENTE_EN_COURS;
            break;
        case DESCENTE_EN_COURS:
            if(FIN_COURSE1==0) //Si on touche le fin de course bas
            {
                respiratorState.positionMoteur1=0; //On réinit la position 
                etat1=ATTENTE; //On passe en attente pour ne pas tout casser
            }
            break;
        case MONTEE:
            //respiratorState.positionMoteur1=0;
            SetMotorTargetPos(1, respiratorState.amplitude, vitesseMaxMoteur);            //On monte
            etat1=MONTEE_EN_COURS;
            break;
        case MONTEE_EN_COURS:
            if( FIN_COURSE2==0 || respiratorState.positionMoteur1>= respiratorState.targetMoteur1)
            {
                etat1=ATTENTE; //On passe en attente pour ne pas tout casser
                initMoteur1Done=1;
            }
            break;
        //ATTENTE    
        case ATTENTE:
            StopMotor(1);
            etat1 = ATTENTE_EN_COURS;
            break;
        //ATTENTE    
        case ATTENTE_EN_COURS:
            //Il ne passe rien, on force les états montée ou descente
            break;
    }
}

void SetStateMoteur1(ETATMoteur _etat)
{
    etat1=_etat;
}

ETATMoteur etat2=INIT;
void StateMachineMoteur2(void)
{
    switch(etat2)
    {
        //Init
        case INIT:
            SetMotorTargetPos(2, -100000, vitesseMaxMoteur-1000);            //On descend
            etat2=INIT_EN_COURS;
            break;
        //Descente d'init    
        case INIT_EN_COURS:
            if(FIN_COURSE3==0)
            {
                StopMotor(2);       //On arrete le moteur 1
                respiratorState.positionMoteur2=0;           //On reset la position du moteur 1
                etat2=MONTEE;            //On remonte
            }
            break;
        //DESCENTE    
        case DESCENTE:
            SetMotorTargetPos(2, -50, respiratorState.vitesse); //-50 pour descendre au dela du fin de course
            etat2=DESCENTE_EN_COURS;
            break;
        case DESCENTE_EN_COURS:
            if(FIN_COURSE3==0) //Si on touche le fin de course bas
            {
                respiratorState.positionMoteur2=0; //On réinit la position 
                etat2=ATTENTE; //On passe en attente pour ne pas tout casser
            }
            break;
        case MONTEE:
            //respiratorState.positionMoteur=0;
            SetMotorTargetPos(2, respiratorState.amplitude, vitesseMaxMoteur);            //On monte
            etat2=MONTEE_EN_COURS;
            break;
        case MONTEE_EN_COURS:
            if( /*FIN_COURSE4==0 ||*/ respiratorState.positionMoteur2>= respiratorState.targetMoteur2)
            {
                etat2=ATTENTE; //On passe en attente pour ne pas tout casser
                initMoteur2Done=1;
            }
            break;
        //ATTENTE    
        case ATTENTE:
            StopMotor(2);
            etat2 = ATTENTE_EN_COURS;
            break;
        //ATTENTE    
        case ATTENTE_EN_COURS:
            //Il ne passe rien, on force les états montée ou descente
            break;
    }
}

void SetStateMoteur2(ETATMoteur _etat)
{
    etat2=_etat;
}

ETATMoteur etat3=INIT;
void StateMachineMoteur3(void)
{
    switch(etat3)
    {
        //Init
        case INIT:
            SetMotorTargetPos(3, -100000, vitesseMaxMoteur-1000);            //On descend
            etat3=INIT_EN_COURS;
            break;
        //Descente d'init    
        case INIT_EN_COURS:
            if(FIN_COURSE5==0)
            {
                StopMotor(3);       //On arrete le moteur 3
                respiratorState.positionMoteur3=0;           //On reset la position du moteur 3
                etat3=MONTEE;            //On remonte
            }
            break;
        //DESCENTE    
        case DESCENTE:
            SetMotorTargetPos(3, -50, respiratorState.vitesse); //-50 pour descendre au dela du fin de course
            etat3=DESCENTE_EN_COURS;
            break;
        case DESCENTE_EN_COURS:
            if(FIN_COURSE5==0) //Si on touche le fin de course bas
            {
                respiratorState.positionMoteur3=0; //On réinit la position 
                etat3=ATTENTE; //On passe en attente pour ne pas tout casser
            }
            break;
        case MONTEE:
            //respiratorState.positionMoteur=0;
            SetMotorTargetPos(3, respiratorState.amplitude, vitesseMaxMoteur);            //On monte
            etat3=MONTEE_EN_COURS;
            break;
        case MONTEE_EN_COURS:
            if( /*FIN_COURSE6==0 ||*/ respiratorState.positionMoteur3>= respiratorState.targetMoteur3)
            {
                etat3=ATTENTE; //On passe en attente pour ne pas tout casser
                initMoteur3Done=1;
            }
            break;
        //ATTENTE    
        case ATTENTE:
            StopMotor(3);
            etat3 = ATTENTE_EN_COURS;
            break;
        //ATTENTE    
        case ATTENTE_EN_COURS:
            //Il ne passe rien, on force les états montée ou descente
            break;
    }
}

void SetStateMoteur3(ETATMoteur _etat)
{
    etat3=_etat;
}


unsigned char buttonStartLastState=0;
unsigned char buttonResetLastState=0;
void StateMachineButton(void)
{
    if((BUTTON_START == SW_ACTIVE_STATE && buttonStartLastState == 0))
    {
        //On a un appui sur le bouton SW1
        buttonStartLastState = 1;
        #ifdef BUTTON_V2
            SwPushed(SW1);
        #else
            Sw1Pushed();
        #endif
    }
    else if(BUTTON_START == !SW_ACTIVE_STATE && buttonStartLastState == 1)
    {
        //On a relaché le bouton SW1
        buttonStartLastState = 0;
        #ifdef BUTTON_V2
            SwReleased(SW1);
        #else
            Sw1Released();
        #endif
    }
    
    if((BUTTON_RESET == SW_ACTIVE_STATE && buttonResetLastState == 0))
    {
        //On a un appui sur le bouton SW1
        buttonResetLastState = 1;
        #ifdef BUTTON_V2
            SwPushed(SW2);
        #else
            Sw1Pushed();
        #endif
    }
    else if(BUTTON_RESET == !SW_ACTIVE_STATE && buttonResetLastState == 1)
    {
        //On a relaché le bouton SW1
        buttonResetLastState = 0;
        #ifdef BUTTON_V2
            SwReleased(SW2);
        #else
            Sw1Released();
        #endif
    }
    
    IsSequenceFinished();
}
//</editor-fold>

//<editor-fold defaultstate="collapsed" desc="CallBack Timers">
void Timer1CallBack(void)
{
    if(boardVersion3Moteur)
    {
        //On controle le moteur 2
        if(respiratorState.targetMoteur1>respiratorState.positionMoteur1)
        {
            DIR1=SENS_MONTEE;
            STEP1=!STEP1;                   //On genere un pulse sur le moteur 2
            if(STEP1==0)
                respiratorState.positionMoteur1++;
        }
        else if(respiratorState.targetMoteur1<respiratorState.positionMoteur1)
        {
            DIR1=SENS_DESCENTE;
            STEP1=!STEP1;                   //On genere un pulse sur le moteur 2
            if(STEP1==0)
                respiratorState.positionMoteur1--;
        }
        else
        {
            //On a atteint la cible
            TurnOnOffTimer(TIMER1_ID,OFF);           //On arrete le timer
        }
    }
    else
    {
        //On controle le moteur 2
        if(respiratorState.targetMoteur1>respiratorState.positionMoteur1)
        {
            DIR1=SENS_MONTEE;
            STEP1=!STEP1;                   //On genere un pulse sur le moteur 2
            if(STEP1==0)
                respiratorState.positionMoteur1++;
        }
        else if(respiratorState.targetMoteur1<respiratorState.positionMoteur1)
        {
            DIR1=SENS_DESCENTE;
            STEP1=!STEP1;                   //On genere un pulse sur le moteur 2
            if(STEP1==0)
                respiratorState.positionMoteur1--;
        }
        else
        {
            //On a atteint la cible
            TurnOnOffTimer(TIMER1_ID,OFF);           //On arrete le timer
        }
    }
}

void Timer2CallBack(void)
{
    if(boardVersion3Moteur)
    {
        //On controle le moteur 2
        if(respiratorState.targetMoteur2>respiratorState.positionMoteur2)
        {
            DIR2=SENS_MONTEE;
            STEP2=!STEP2;                   //On genere un pulse sur le moteur 2
            if(STEP2==0)
                respiratorState.positionMoteur2++;
        }
        else if(respiratorState.targetMoteur2<respiratorState.positionMoteur2)
        {
            DIR2=SENS_DESCENTE;
            STEP2=!STEP2;                   //On genere un pulse sur le moteur 2
            if(STEP2==0)
                respiratorState.positionMoteur2--;
        }
        else
        {
            //On a atteint la cible
            TurnOnOffTimer(TIMER2_ID,OFF);           //On arrete le timer
        }
        timeStampHighSpeed++;
    }
    else
    {
        
    }
}
            


//Timer generation des pulses (Gestion moteurs)
void Timer3CallBack(void)
{
    //Si on utilise la version 3 moteurs
    if(boardVersion3Moteur)
    {
        //On controle le moteur 3
        if(respiratorState.targetMoteur3>respiratorState.positionMoteur3)
        {
            DIR3=SENS_MONTEE;
            STEP3=!STEP3;                   //On genere un pulse sur le moteur 2
            if(STEP3==0)
                respiratorState.positionMoteur3++;
        }
        else if(respiratorState.targetMoteur3<respiratorState.positionMoteur3)
        {
            DIR3=SENS_DESCENTE;
            STEP3=!STEP3;                   //On genere un pulse sur le moteur 2
            if(STEP3==0)
                respiratorState.positionMoteur3--;
        }
        else
        {
            //On a atteint la cible
            TurnOnOffTimer(TIMER3_ID,OFF);           //On arrete le timer
        }
    }
}

extern unsigned int antiBlockCounterI2C1;
void Timer4CallBack(void)
{
    g_longTimeStamp++;
    antiBlockCounterI2C1++;
}

unsigned char subdiv2=0;
void Timer5CallBack(void)
{
    //respiratorState.volumeCourant+=respiratorState.debitCourant/50.0;
    static unsigned char subdiv=0;
    unsigned char payload[16];
    payload[0]=BREAK_UINT32(g_longTimeStamp,3);
    payload[1]=BREAK_UINT32(g_longTimeStamp,2);
    payload[2]=BREAK_UINT32(g_longTimeStamp,1);
    payload[3]=BREAK_UINT32(g_longTimeStamp,0);
    getBytesFromFloat(payload, 4, (float)respiratorState.volumeCourant);   //Pressure1 (volume)
    getBytesFromFloat(payload, 8, (float)respiratorState.pressure2);           //Pressure2
    getBytesFromFloat(payload, 12, (float)respiratorState.positionMoteur1);
    MakeAndSendMessageWithUTLNProtocol(0x0064, 16, payload);
    unsigned char i2cOut[1];
    unsigned char i2cIn[2];
    I2C1WriteNReadNInterrupt( (0x28<<1), i2cOut,0, i2cIn, 2 );
    respiratorState.pressure2= (float)(((BUILD_INT16((i2cIn[0]&0x3F),i2cIn[1])-1530))*0.257724);
//        if(respiratorState.pressure2<0)
//        respiratorState.pressure2=0;

//    ADC1StartConversionSequence();
    if(subdiv++>=1)
    {
        subdiv=0;
        respiratorState.pressure1=D6FPress_meas();
        
        
        {
            char sign=1;
            float diffPression=respiratorState.pressure1;
            if(diffPression<0)
                sign=-1;
            else
                sign=1;
            
            float vitesse = sqrt(2*ABS(diffPression)/1.23)*(float)sign;
            respiratorState.debitCourant=vitesse*surface;
            
            if(vitesse>=0.01)
                respiratorState.volumeCourant+=(respiratorState.debitCourant*1000)/25.0;
        }
    }
    
    if(subdiv2++>=1)
    {
        freqTimer+=00;
        //SetTimerFreq(TIMER1_ID,freqTimer);
        subdiv2=0;
    }
    
}
//</editor-fold>

void ButtonStartCallback(void)
{
    if(!respiratorStarted)
        startRespirator=true;
    else
        startRespirator=false;
}

void ButtonResetCallback(void)
{
    InitMachine();
}

void CalculateRespiratorParameters(void)
{
    double periode=60000/respiratorState.cyclesPerMinute;       //en ms
    
    //Amplitude = 600 pas..
    //Freq timer = 500
    //Tps de montée= 600/500 = 1.2 sec
    
    //Donc freq=1/((0.2*periode)/amplitude) == amplitude/(0.2*periode)
    double timerFreq=0;
    if(!boardVersion3Moteur)
    {
        timerFreq=respiratorState.amplitude/(0.2*periode/1000.0);
       // SetTimerFreq(TIMER3_ID,MIN(timerFreq,750));//Gestion de la vitesse des pas a pas (limite a 750steps a cause du courant )
        
        
        respiratorState.vitesse=MIN(2*respiratorState.amplitude/(0.2*periode/1000.0), vitesseMaxMoteur);//Gestion de la vitesse des pas a pas (limite a 750steps a cause du courant )       
        respiratorState.tempsMontee=0.2*periode;
        respiratorState.periode=periode*1000*0.2;
    }
    else
    {
        respiratorState.vitesse=MIN(2*respiratorState.amplitude/(0.2*periode/1000.0), vitesseMaxMoteur);//Gestion de la vitesse des pas a pas (limite a 750steps a cause du courant )       
        respiratorState.tempsMontee=0.2*periode;
        respiratorState.periode=periode*1000*0.2;
    }
    respiratorState.attenteHaut=0.3*periode;
    respiratorState.attenteBas=0.5*periode;
}

void SetMotorTargetPos(unsigned char motorID, double target, double speed)
{
    switch(motorID)
    {
        case 1://On demarre le timer 1
            SetTimerFreq(TIMER1_ID,(float)speed);           //On regle la vitesse
            respiratorState.targetMoteur1=target;           //On set la position cible
            TurnOnOffTimer(TIMER1_ID,ON);
            break;
        case 2:
            SetTimerFreq(TIMER2_ID,(float)speed);  
            respiratorState.targetMoteur2=target;            
            TurnOnOffTimer(TIMER2_ID,ON);
            break;
        case 3:
            SetTimerFreq(TIMER3_ID,(float)speed);   
            respiratorState.targetMoteur3=target;            
            TurnOnOffTimer(TIMER3_ID,ON);
            break;
        case 4:
            SetTimerFreq(TIMER4_ID,(float)speed);     
            respiratorState.targetMoteur4=target;              
            TurnOnOffTimer(TIMER4_ID,ON);
            break;            
        case 5:
            SetTimerFreq(TIMER5_ID,(float)speed);  
            respiratorState.targetMoteur5=target;  
            TurnOnOffTimer(TIMER5_ID,ON);
            break;
        default:
            break;            
    }
}

void SetMotorTarget(unsigned char motorID, unsigned int target)
{
    switch(motorID)
    {
        case 1://On demarre le timer 1
            TurnOnOffTimer(TIMER1_ID,ON);
            respiratorState.targetMoteur1=target;
            break;
        case 2:
            TurnOnOffTimer(TIMER2_ID,ON);
            respiratorState.targetMoteur2=target;
            break;
        case 3:
            TurnOnOffTimer(TIMER3_ID,ON);
            respiratorState.targetMoteur3=target;            
            break;
        case 4:
            TurnOnOffTimer(TIMER4_ID,ON);
            respiratorState.targetMoteur4=target;            
            break;            
        case 5:
            TurnOnOffTimer(TIMER5_ID,ON);
            respiratorState.targetMoteur5=target;            
            break;
        default:
            break;            
    }
}

void StopMotor(unsigned char motorID)
{
    switch(motorID)
    {
        case 1://On demarre le timer 1
            TurnOnOffTimer(TIMER1_ID,OFF);
            break;
        case 2:
            TurnOnOffTimer(TIMER2_ID,OFF);
            break;
        case 3:
            TurnOnOffTimer(TIMER3_ID,OFF);
       
            break;
        case 4:
            TurnOnOffTimer(TIMER4_ID,OFF);      
            break;            
        case 5:
            TurnOnOffTimer(TIMER5_ID,OFF);        
            break;
        default:
            break;            
    }
}