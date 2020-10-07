#ifndef UTLN_UART_H
#define UTLN_UART_H
/*******************************************************************************
 * INCLUDES
 ******************************************************************************/
#include <stdlib.h>
//#include <uart.h>
#include "UTLN_Typedefs.h"

/*******************************************************************************
 * CONSTANTS
 ******************************************************************************/
#define UART2_BUFFER_SIZE 256


#define UART1_CIRCULAR_BUFFER_SIZE 256
#define UART2_CIRCULAR_BUFFER_SIZE 128
#define UART3_CIRCULAR_BUFFER_SIZE 512
#define UART4_CIRCULAR_BUFFER_SIZE 128
#define UART5_CIRCULAR_BUFFER_SIZE 255

/*******************************************************************************
 * FUNCTIONS PROTOTYPE
 ******************************************************************************/
void initDmaUart1Rx(void);
void initDmaUart2Rx(void);
void initDmaUart1Tx(void);
void initDma3(void);
void initUART1(void);
void initUART2(void);
void initUART3(void);
void sendUART1DMABuffer(void);
void sendUART2DMABuffer(void);
//void WriteCharUart1(unsigned char val);
size_t ReadUart2(unsigned char * buffer, size_t length);

//void putCFromTxBuffer();
void UART1ResetRxBuffer(void);
void UART1ResetTxBuffer(void);
void UART1ReadToRxBuffer(void);
BOOL UART1IsDataReadyInRxBuffer(void);
unsigned char UART1ReadFromRxBuffer(void);
void UART1WriteToTxBuffer(unsigned char value);
BOOL UART1IsDataReadyInTxBuffer(void);
void UART1WriteFromTxBuffer(void);
void Uart1MonitorAndFixTxTransmission();
unsigned int UART1GetRemainingSpaceInTxBuffer(void);
unsigned int UART1GetNbOfValuesInTxBuffer(void);
void UART1IncrementTxAntiBlockCounter(void);
void UART1ResetTxAntiBlockCounter(void);
void UART1MonitorAndFixTxTransmission();

void UART2ReadToRxBuffer(void);
void UART2WriteFromTxBuffer(void);
BOOL UART2IsDataReadyInTxBuffer(void);
void UART2WriteToTxBuffer(unsigned char value);
unsigned int UART2GetRemainingSpaceInTxBuffer(void);
BOOL UART2IsDataReadyInRxBuffer(void);
unsigned char UART2ReadFromRxBuffer(void);

void UART3ReadToRxBuffer(void);
BOOL UART3IsDataReadyInRxBuffer(void);
unsigned char UART3ReadFromRxBuffer(void);
void UART3WriteToTxBuffer(unsigned char value);
BOOL UART3IsDataReadyInTxBuffer(void);
void UART3WriteFromTxBuffer(void);
unsigned int UART3GetRemainingSpaceInTxBuffer(void);
unsigned int UART3GetNbOfValuesInTxBuffer(void);
BOOL UART3IsTransmissionActive(void);

void initUART4(void);
void UART4ReadToRxBuffer(void);
BOOL UART4IsDataReadyInRxBuffer(void);
unsigned char UART4ReadFromRxBuffer(void);
void UART4WriteToTxBuffer(unsigned char value);
BOOL UART4IsDataReadyInTxBuffer(void);
BOOL UART4IsTransmissionActive(void);
void UART4WriteFromTxBuffer(void);
unsigned int UART4GetNbOfValuesInTxBuffer(void);
unsigned int UART4GetRemainingSpaceInTxBuffer(void);
void UART4WriteToTxBuffer(unsigned char value);
BOOL UART4IsDataReadyInTxBuffer(void);
BOOL UART4IsTransmissionActive(void);
void UART4WriteFromTxBuffer(void);
unsigned int UART4GetNbOfValuesInTxBuffer(void);
unsigned int UART4GetRemainingSpaceInTxBuffer(void);

#ifdef USE_UART3
void __attribute__((__interrupt__, no_auto_psv)) _U3TXInterrupt(void);
void __attribute__((__interrupt__, no_auto_psv)) _U3RXInterrupt(void);
#endif
/*******************************************************************************
 * ISR PROTOTYPE
 ******************************************************************************/
#ifdef USE_DMA
void __attribute__((__interrupt__, auto_psv)) _DMA1Interrupt(void);
void __attribute__((__interrupt__, auto_psv)) _DMA2Interrupt(void);
void __attribute__((__interrupt__, auto_psv)) _DMA3Interrupt(void);
void putcUART1DMA(unsigned char c);
#endif

/*******************************************************************************
 * VARIABLES GLOBALS
 ******************************************************************************/
extern volatile unsigned int flagRxUART1DataAvailable;
extern volatile unsigned char flagRxUART2DataAvailable;

//Variables neccessaires au fonctionnement du buffer tournant
/*
extern volatile BOOL U1TxTransmissionEnCours;
extern volatile unsigned char bufferTxHead;
extern volatile unsigned char bufferTxTail;
extern volatile unsigned char bufferTx[BUFFER_TX_UART_SIZE];
 */

#endif
