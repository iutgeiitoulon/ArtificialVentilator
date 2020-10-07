/******************************************************************************
  Filename:       ustv_Communication.c
  Revised:        $Date: 2012-11-19 18:52:21 $
  Revision:       $Revision: 00000 $

  Description:   USTV functions for USTV Communication (UART Routine,
  USTV Parser, ...)

  Copyright 2012 Universit� du Sud Toulon Var. All rights reserved.

  IMPORTANT: Your use of this Software is limited to those specific rights
  granted under the terms of a software license agreement between the user
  who downloaded the software, his/her employer .You may not use this
  Software unless you agree to abide by the terms of the License. Other than for
  the foregoing purpose, you may not use, reproduce, copy, prepare derivative
  works of, modify, distribute, perform, display or sell this Software and/or
  its documentation for any purpose.

  YOU FURTHER ACKNOWLEDGE AND AGREE THAT THE SOFTWARE AND DOCUMENTATION ARE
  PROVIDED ?AS IS? WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED,
  INCLUDING WITHOUT LIMITATION, ANY WARRANTY OF MERCHANTABILITY, TITLE,
  NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR PURPOSE. IN NO EVENT SHALL
  TEXAS INSTRUMENTS OR ITS LICENSORS BE LIABLE OR OBLIGATED UNDER CONTRACT,
  NEGLIGENCE, STRICT LIABILITY, CONTRIBUTION, BREACH OF WARRANTY, OR OTHER
  LEGAL EQUITABLE THEORY ANY DIRECT OR INDIRECT DAMAGES OR EXPENSES
  INCLUDING BUT NOT LIMITED TO ANY INCIDENTAL, SPECIAL, INDIRECT, PUNITIVE
  OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA, COST OF PROCUREMENT
  OF SUBSTITUTE GOODS, TECHNOLOGY, SERVICES, OR ANY CLAIMS BY THIRD PARTIES
  (INCLUDING BUT NOT LIMITED TO ANY DEFENSE THEREOF), OR OTHER SIMILAR COSTS.

  Should you have any questions regarding your right to use this Software,
  contact DPT GEII at www.univ-tln.fr.

 *****************************************************************************/
/*******************************************************************************
 * INCLUDES
 ******************************************************************************/
#include "UTLN_Communication.h"
#include "Utilities.h"
//#include "UTLN_Message.h"
#include "UTLN_uart.h"

#include "UTLN_Timers.h"
#include "main.h"
#include "RespiratorState.h"

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "Define.h"
#define BUFFER_TX_UART_SIZE 128
extern unsigned long autoStopMotorOnNoCommandTimeStamp;

extern void CalculateRespiratorParameters(void);
extern void InitMachine(void);
/***************************************************************************************************
 * CONSTANTS
 ***************************************************************************************************/
/* State values for ZTool protocol */
#define SOP_STATE      0x00
#define CMD_STATE1     0x01
#define CMD_STATE2     0x02
#define LEN_STATE      0x03
#define DATA_STATE     0x04
#define FCS_STATE      0x05
    
#define HCI_CMD_STATE0     0x00
#define HCI_CMD_STATE1     0x01
#define HCI_LEN_STATE      0x02
#define HCI_DATA_STATE     0x03


/*******************************************************************************
 * VARIABLES
 ******************************************************************************/
extern volatile unsigned long g_longTimeStamp;

/*******************************************************************************
 * FONCTIONS
 ******************************************************************************/
unsigned char UartCalculateChecksum(unsigned int msgFunction,
        unsigned int msgPayloadLength, unsigned char * msgPayload)
{
    unsigned char checksum = 0;
    unsigned int i = 0;
    //checksum ^= 0xFE;
    checksum ^= (unsigned char) (msgFunction >> 8);
    checksum ^= (unsigned char) (msgFunction);
    checksum ^= (unsigned char) (msgPayloadLength >> 8);
    checksum ^= (unsigned char) (msgPayloadLength);
    for (i = 0; i < msgPayloadLength; i++)
    {
        checksum ^= msgPayload[i];
    }
    return checksum;
}

unsigned char rx1ReceptionState = RECEPTION_WAIT;
unsigned int rx1ReceivedFunction;
unsigned int rx1ReceivedPayloadLength;
unsigned char rx1ReceivedPayload[UART1_CIRCULAR_BUFFER_SIZE];
unsigned int rx1ReceivedPayloadIndex = 0;
void Uart1DecodeMessage(unsigned char c)
{    
    switch (rx1ReceptionState)
    {
        case RECEPTION_WAIT:
            if (c == 0xFE)
                rx1ReceptionState = RECEPTION_FUNCTION_MSB;
            break;
        case RECEPTION_FUNCTION_MSB:
            rx1ReceivedFunction = (unsigned int) (c << 8);
            rx1ReceptionState = RECEPTION_FUNCTION_LSB;
            break;
        case RECEPTION_FUNCTION_LSB:
            rx1ReceivedFunction += (unsigned int) c;
            rx1ReceptionState = RECEPTION_PAYLOAD_LENGTH_MSB;
            break;
        case RECEPTION_PAYLOAD_LENGTH_MSB:
            rx1ReceivedPayloadLength = (unsigned int) (c << 8);
            rx1ReceptionState = RECEPTION_PAYLOAD_LENGTH_LSB;
            break;
        case RECEPTION_PAYLOAD_LENGTH_LSB:
            rx1ReceivedPayloadLength += (unsigned int) c;
            if (rx1ReceivedPayloadLength > UART1_CIRCULAR_BUFFER_SIZE)
                rx1ReceptionState = RECEPTION_WAIT;
            else if (rx1ReceivedPayloadLength == 0)
                rx1ReceptionState = RECEPTION_CHECKSUM;
            else
                rx1ReceptionState = RECEPTION_PAYLOAD;
            break;
        case RECEPTION_PAYLOAD:
            rx1ReceivedPayload[rx1ReceivedPayloadIndex] = c;
            rx1ReceivedPayloadIndex++;
            if (rx1ReceivedPayloadIndex == rx1ReceivedPayloadLength)
            {
                rx1ReceivedPayloadIndex = 0;
                rx1ReceptionState = RECEPTION_CHECKSUM;
            }
            break;
        case RECEPTION_CHECKSUM:
            if (c == UartCalculateChecksum(rx1ReceivedFunction,
                    rx1ReceivedPayloadLength, rx1ReceivedPayload))
            {
                //Message valide
                ProcessMessage( rx1ReceivedFunction, rx1ReceivedPayloadLength, rx1ReceivedPayload);
            }
            rx1ReceptionState = RECEPTION_WAIT;
            break;
        default:
            rx1ReceptionState = RECEPTION_WAIT;
            break;
    }
}



/*******************************************************************************
 * @fn      SendMessageTxUART()
 *
 * @brief   
 *
 * @param   payload - 1-256 byte - Tableau dans lequel est enregistr�e la
 *          Payload.
 *          payloadLength - 1 byte - Taille de la payload.
 *
 * @return  None
 *
 ******************************************************************************/
int SendMessageTxUART(unsigned char* payload, unsigned char payloadLength)
{
    IEC0bits.U1TXIE=1;
    #ifdef USE_UART1_CIRCULAR_BUFFER
    int i;
    if(UART1GetRemainingSpaceInTxBuffer()>=payloadLength+2)
    {
        UART1WriteToTxBuffer(0xFE);
        for(i=0;i<payloadLength;i++)
        {
                //On envoie un octet dans le buffer
                UART1WriteToTxBuffer(payload[i]);
        }
    }//end for()
    #endif
    return 0;                                               //On renvoi le nombre de caracteres emis    
}


/*******************************************************************************
 * @fn      ProcessEndDeviceMessage
 *
 * @brief   Fonction permettant le traitement des message recus via le ZigBee.
 *          (Ex: Demande d'acquisition sur Magnetometre, demande de mesure de
 *          batterie, allumer une LED, ...). Une reponse peut etre renvoy�e
 *          au travers du reseau .
 *
 * @param   command - 2 byte - Commande a executer (Propre au protocole USTV)
 *          payload - 1-256 byte - Tableau dans lequel est enregistr�e la
 *          Payload.
 *          length - 1 byte - Taille de la payload.
 *
 * @return  None
 *
 ******************************************************************************/
BOOL startRespirator=false;
void ProcessMessage( unsigned short int command, unsigned short int length, unsigned char payload[])
{
    unsigned char blockMessage = 0;
    unsigned short int msgTxUARTPayloadLength;
    static unsigned char msgTxUARTPayload[BUFFER_TX_UART_SIZE];


    //Valeur par d�faut pour �viter de renvoyer la payload pr�c�dente si la valeur n'est pas renseign�e
    msgTxUARTPayloadLength = 0;
    unsigned char pos=0;

    switch(command)
    {        
        //START/STOP
        case 0x0001:
            blockMessage=0;
            if(payload[0])
            {
                //Start
                startRespirator=true;
            }
            else
            {
                //Stop
                startRespirator=false;
            }
            msgTxUARTPayload[0]=startRespirator;
            msgTxUARTPayloadLength=1;
            break;
        //DoSteps
        case 0x0003:
            respiratorState.doStepsMotorNum=payload[0];
            respiratorState.doStepsCount=BUILD_UINT32(payload[4],payload[3],payload[2],payload[1]);
            respiratorState.flagDoStepsCMD=true;
            break; 
        //Reset Steps Counter
        case 0x0004:
           
            break; 
        //Set Offset Up
        case 0x0005:
            blockMessage=0;
            respiratorState.stepsOffsetUp=BUILD_UINT32(payload[3],payload[2],payload[1],payload[0]);
            getBytesFromInt32(msgTxUARTPayload, 0, respiratorState.stepsOffsetUp);
            msgTxUARTPayloadLength=4;
            break; 
        //Set Offset Down
        case 0x0006:
            blockMessage=0;
            respiratorState.stepsOffsetDown=BUILD_UINT32(payload[3],payload[2],payload[1],payload[0]);
            getBytesFromInt32(msgTxUARTPayload, 0, respiratorState.stepsOffsetDown);
            msgTxUARTPayloadLength=4;
            break;     
        //Set Amplitude
        case 0x0007:
            blockMessage=0;
            respiratorState.amplitude=BUILD_UINT32(payload[3],payload[2],payload[1],payload[0]);
            getBytesFromInt32(msgTxUARTPayload, 0, respiratorState.amplitude);
            msgTxUARTPayloadLength=4;
            break; 
        //Set Cycles/min
        case 0x0008:
            blockMessage=0;
            respiratorState.cyclesPerMinute=payload[0];
            msgTxUARTPayload[0]=respiratorState.cyclesPerMinute;
            CalculateRespiratorParameters();
            msgTxUARTPayloadLength=1;
            break;            
        //Set Speed
        case 0x0014:
            blockMessage=0;
            respiratorState.vitesse=(double)getFloat(payload,0);
            getBytesFromFloat(msgTxUARTPayload, 0, respiratorState.vitesse);
            SetTimerFreq(TIMER3_ID,respiratorState.vitesse*13);
            msgTxUARTPayloadLength=4;
            break;                         
        //Set Pause Time Up
        case 0x0015:
            blockMessage=0;
            respiratorState.attenteHaut=(double)(getFloat(payload,0)*1000.0);
            getBytesFromFloat(msgTxUARTPayload, 0, respiratorState.attenteHaut/1000.0);
            msgTxUARTPayloadLength=4;
            break;  
        //Set Pause Time Down
        case 0x0016:
            blockMessage=0;
            respiratorState.attenteBas=(double)(getFloat(payload,0)*1000);
            getBytesFromFloat(msgTxUARTPayload, 0, respiratorState.attenteBas/1000.0);
            msgTxUARTPayloadLength=4;
            break;      
        //Set V Limit
        case 0x0017:
            blockMessage=0;
            respiratorState.vLimite=(double)(getFloat(payload,0));
            getBytesFromFloat(msgTxUARTPayload, 0, respiratorState.vLimite);
            msgTxUARTPayloadLength=4;
            break;      
        //Set P Limit
        case 0x0018:
            blockMessage=0;
            respiratorState.pLimite=(double)(getFloat(payload,0))*100;
            getBytesFromFloat(msgTxUARTPayload, 0, respiratorState.pLimite);
            msgTxUARTPayloadLength=4;
            break;        
        //Set Mode    
        case 0x0019:
            blockMessage=0;
            if(payload[0])
            {
                respiratorState.isAssistanceMode=1;
            }
            else
            {
                respiratorState.isAssistanceMode=0;
            }
            msgTxUARTPayload[0]=respiratorState.isAssistanceMode;
            msgTxUARTPayloadLength=1;
            break;
        //Init
        case 0x001A:
            blockMessage=1;
            InitMachine();
            break;    
        //Seuil Assistance    
        case 0x001B:
            blockMessage=0;
            respiratorState.seuilAssistance=(double)(getFloat(payload,0));
            getBytesFromFloat(msgTxUARTPayload, 0, respiratorState.seuilAssistance);
            msgTxUARTPayloadLength=4;
            break;        
        case 0x79:
            blockMessage=1;
            g_longTimeStamp=payload[0] + ((unsigned long)payload[1]<<8) + ((unsigned long)payload[2]<<16) + ((unsigned long)payload[3]<<24);
            //On prepare un message
            msgTxUARTPayloadLength = 0;
            break;
            

        case EMERGENCY_STOP:
            blockMessage=1;
            
            break;
        // Unknown command
        default:
        {
            blockMessage=1;
            msgTxUARTPayloadLength = 1;
            msgTxUARTPayload[0]=(unsigned char)0;
        }
        break;
    }
    //On envoie le message
    if(blockMessage==0)
    {
        MakeAndSendMessageWithUTLNProtocol(command, msgTxUARTPayloadLength, msgTxUARTPayload);
    }
    blockMessage=0;
}

void MakeAndSendMessageWithUTLNProtocol(unsigned short command, unsigned int payloadLength, unsigned char* payload)
{
    unsigned char outPayload[payloadLength+UTLN_FRAME_SIZE];
    int i;
    outPayload[0]=SOF;
    outPayload[1]=MSB_UINT16(command);
    outPayload[2]=LSB_UINT16(command);
    outPayload[3]=MSB_UINT16(payloadLength);
    outPayload[4]=LSB_UINT16(payloadLength);
    for(i=0;i<payloadLength;i++)
    {
        outPayload[5+i]=payload[i];
    }
    outPayload[5+payloadLength]=UartCalculateChecksum(command,payloadLength, payload);
    
    SendMessageToUart1( outPayload,UTLN_FRAME_SIZE+payloadLength );
}


void SendMessageToUart1(  unsigned char* payload,unsigned short payloadLength)
{
    if(UART1GetRemainingSpaceInTxBuffer()>payloadLength)
    {
        int i;
        for(i=0;i<payloadLength;i++)
        {
            UART1WriteToTxBuffer(payload[i]);
        }
    }
}

//============================================================================//
void SendWelcomeMessage(void)
{
    unsigned char payload[1];
    MakeAndSendMessageWithUTLNProtocol(SEND_WELCOME_MESSAGE, 0, payload);
}


void SendErrorText(const char* str)
{
    int len=strlen(str);
    MakeAndSendMessageWithUTLNProtocol(SEND_ERROR_TEXT, len,(unsigned char*) str);
}
/*******************************************************************************
 End of File
*/


