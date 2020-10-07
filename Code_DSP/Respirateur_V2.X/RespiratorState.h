/* Microchip Technology Inc. and its subsidiaries.  You may use this software 
 * and any derivatives exclusively with Microchip products. 
 * 
 * THIS SOFTWARE IS SUPPLIED BY MICROCHIP "AS IS".  NO WARRANTIES, WHETHER 
 * EXPRESS, IMPLIED OR STATUTORY, APPLY TO THIS SOFTWARE, INCLUDING ANY IMPLIED 
 * WARRANTIES OF NON-INFRINGEMENT, MERCHANTABILITY, AND FITNESS FOR A 
 * PARTICULAR PURPOSE, OR ITS INTERACTION WITH MICROCHIP PRODUCTS, COMBINATION 
 * WITH ANY OTHER PRODUCTS, OR USE IN ANY APPLICATION. 
 *
 * IN NO EVENT WILL MICROCHIP BE LIABLE FOR ANY INDIRECT, SPECIAL, PUNITIVE, 
 * INCIDENTAL OR CONSEQUENTIAL LOSS, DAMAGE, COST OR EXPENSE OF ANY KIND 
 * WHATSOEVER RELATED TO THE SOFTWARE, HOWEVER CAUSED, EVEN IF MICROCHIP HAS 
 * BEEN ADVISED OF THE POSSIBILITY OR THE DAMAGES ARE FORESEEABLE.  TO THE 
 * FULLEST EXTENT ALLOWED BY LAW, MICROCHIP'S TOTAL LIABILITY ON ALL CLAIMS 
 * IN ANY WAY RELATED TO THIS SOFTWARE WILL NOT EXCEED THE AMOUNT OF FEES, IF 
 * ANY, THAT YOU HAVE PAID DIRECTLY TO MICROCHIP FOR THIS SOFTWARE.
 *
 * MICROCHIP PROVIDES THIS SOFTWARE CONDITIONALLY UPON YOUR ACCEPTANCE OF THESE 
 * TERMS. 
 */

/* 
 * File:   
 * Author: 
 * Comments:
 * Revision history: 
 */

// This is a guard condition so that contents of this file are not included
// more than once.  
#ifndef RESPIRATORSTATE_H
#define	RESPIRATORSTATE_H

typedef struct robotStateBITS {
    union {
        struct {
            unsigned char isAssistanceMode;
            
            float pressure1;
            float pressure2;
            float volume;
            float seuilAssistance;
            
            double pLimite;             //Seuil pression limite
            double vLimite;             //Seuil volume limite
            double attenteHaut;         //Attente haut en miliseconde
            double tempsMontee;         //Indication sur le temps de montée en pression max
            double debitCourant;               //Debit en M3/s
            double volumeCourant;               //Volume en M3
            double attenteBas;         //Attente bas en miliseconde
            long stepsOffsetUp;
            long stepsOffsetDown;
            long amplitude;           //474 pas
            unsigned char cyclesPerMinute;
            double vitesse;
            unsigned char sensMoteur1;
            unsigned char sensMoteur2;
            unsigned char sensMoteur3;
            double positionMoteur1;
            double positionMoteur2;
            double positionMoteur3;
            double targetMoteur1;
            double targetMoteur2;
            double targetMoteur3;
            double targetMoteur4;
            double targetMoteur5; 
            
            
            unsigned long periode;
            
            unsigned char flagDoStepsCMD;
            unsigned char doStepsMotorNum;
            unsigned long doStepsCount;
        } ;
    } ;
} RESPIRATOR_STATE_BITS;
extern volatile RESPIRATOR_STATE_BITS respiratorState ;
#endif	/* XC_HEADER_TEMPLATE_H */

