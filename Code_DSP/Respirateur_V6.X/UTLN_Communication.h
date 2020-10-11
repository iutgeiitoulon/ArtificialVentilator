/* 
 * File:   ustv_MainFunc.h
 * Author: Val
 *
 * Created on 21 janvier 2013, 16:20
 */

#ifndef USTV_COMMUNICATION_H
#define	USTV_COMMUNICATION_H

/*******************************************************************************
 * INCLUDES
 ******************************************************************************/
#include "UTLN_Typedefs.h"
#define UART1_RX_PAYLOAD_SIZE 128
/* Start-of-frame delimiter for UART transport */
#define MT_UART_SOF                     0xFE
/*******************************************************************************
 * VARIABLES
 ******************************************************************************/

extern volatile unsigned long g_longTimeStamp;

/* position of fields in the general format frame */
#define MT_RPC_POS_CMD0       0
#define MT_RPC_POS_CMD1       1
#define MT_RPC_POS_LEN0        2
#define MT_RPC_POS_LEN1        3
#define MT_RPC_POS_DAT0       4

/* 1st byte is the length of the data field, 2nd/3rd bytes are command field. */
#define MT_RPC_FRAME_HDR_SZ   4

#define HCI_RPC_POS_CMD0       0
#define HCI_RPC_POS_CMD1       1
#define HCI_RPC_POS_LEN        2
#define HCI_RPC_FRAME_HDR_SZ   3

#define SOF 0xFE
#define UTLN_FRAME_SIZE 6

//Processor message states
#define RECEPTION_WAIT 0
#define RECEPTION_FUNCTION_MSB 1
#define RECEPTION_FUNCTION_LSB 2
#define RECEPTION_PAYLOAD_LENGTH_MSB 3
#define RECEPTION_PAYLOAD_LENGTH_LSB 4
#define RECEPTION_PAYLOAD 5
#define RECEPTION_CHECKSUM 6


#define POLOLU_SET_TARGET 0x84
#define SEND_WELCOME_MESSAGE 0x0190
#define SEND_IMU_DATA 0x01A2
#define SEND_VX_VX_VTHETA 0x01A3
#define SEND_MOTOR_VITESSE_DATA 0x01A4
#define SEND_MOTOR_POSITION 0x01A5
#define SEND_MOTOR_SPEED_CONSIGNE_DATA 0x01A6
#define SEND_ENCODER_RAW_DATA 0x01A7
#define SET_VX_VY_VTHETA_CONSIGNE 0x01AB
#define SEND_PID_DEBUG_DATA 0x01AC
#define SET_PID_VALUES 0x01AD
#define ENABLE_DISABLE_MOTORS 0x01B3
#define ENABLE_DISABLE_TIR 0x01B4
#define SEND_MOTOR_CURRENT 0x01B5
#define ENABLE_ASSERVISSEMENT 0x01B6
#define ENABLE_MOTOR_CURRENT 0x01B7
#define ENABLE_ENCODER_RAW_DATA 0x01B8
#define ENABLE_POSITION_DATA 0x01B9
#define ENABLE_MOTOR_SPEED_CONSIGNE_DATA 0x01BA
#define ENABLE_PID_DEBUG_DATA 0x01BB
#define SET_SERVO_UP 0x01BD
#define SET_SERVO_DOWN 0x01BE
#define ENABLE_MOTOR_VITESSE_DATA 0x01BF



#define SEND_ERROR_TEXT 0xEEEE
#define EMERGENCY_STOP 0xFFFF

#define ASK_FOR_PARAMETERS 0x02B0





/*******************************************************************************
 * PROTOTYPES
 ******************************************************************************/
uint8 MT_UartCalcFCS( uint8 *msg_ptr, uint16 len );
void Uart1DecodeMessage(unsigned char c);
void SendMessageUARTMt(unsigned char* payload, unsigned char payloadLength);
void SendMessageUART(unsigned short int destAddress, unsigned short int cmd, unsigned short int payloadLength, unsigned char* payload);
void SendMessageUART2(unsigned short int destAddress, unsigned short int cmd, unsigned short int payloadLength, unsigned char* payload);
void SendMessageSRAM(unsigned short int destAddress, unsigned short int cmd, unsigned short int payloadLength, unsigned char* payload);
int SendMessageTxUART3(unsigned char* payload, unsigned char payloadLength);
int SendMessageTxUART(unsigned char* payload, unsigned char payloadLength);
void ProcessMessage( unsigned short int command, unsigned short int length, unsigned char payload[]);
void ProcessReadableMessage(unsigned char* payload, unsigned short payloadLength);

void SendMessageToMZ( unsigned short int command, unsigned short int payloadLength, unsigned char* payload);
void USTV_SendAllParametersToMz(void);
void UTLN_SendReadySignal(void);
void USTV_RequestMZTimeUpdate(void);
void RequestIsMZRecordingStopped(void);
void RequestMzParametersUpdate(void);
void ForwardMessageToMZSDCard(unsigned char dataType,unsigned short int payloadLength, unsigned char* payload);
void USTV_SendMZMPUData(unsigned char* mpuDat,unsigned char size);
void USTV_SendMZMPU2Data(unsigned char* mpuDat,unsigned char size);
void SendMessageToRS232( unsigned short int payloadLength, unsigned char* payload);

#if defined(USE_ZIGBEE) || defined(USE_BLE)
void SendMessageToRadio(unsigned short int destAddress, unsigned short int command, unsigned short int payloadLength, unsigned char* payload);
#endif
void ProcessUartReceiveChar(void);
void ProcessUart2ReceiveChar(void);
void ProcessUSB(void);
void ProcessUart3ReceiveChar(void);


unsigned char GetTxBufferRemainingSpace(void);
unsigned char GetTxBufferUsedSpace(void);
void AddToTxBuffer(unsigned char value);
BOOL Awaiting(unsigned char condition);
unsigned char AwaitingCondition(unsigned char condition);
void UTLN_WaitForSRSP(unsigned short command,unsigned int timeout);
void Uart3DecodeMessage(void);

void ProcessUSBSerialReceiveBuffer(unsigned char* buffer, unsigned int bufferLength);
void SendMessageToUart1(  unsigned char* payload, unsigned short int payloadLength);
void MakeAndSendMessageWithUTLNProtocol(unsigned short command, unsigned int payloadLength, unsigned char* payload);
void SendWelcomeMessage(void);
void SendAsservissementData();
void SendEncoderRawData();
void SendMotorVitesseData();
void SendMotorPositionData();
void SendMotorSpeedConsigneData();
void SendRobotVitesse();
void SendErrorText(const char* str);
#endif	/* USTV_MAINFUNC_H */

