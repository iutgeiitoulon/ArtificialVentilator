/* 
 * File:   UTLN_D6F-PH.h
 * Author: Barchasz
 *
 * Created on 22 avril 2020, 12:34
 */
#ifndef UTLN_D6F_PH_H
#define	UTLN_D6F_PH_H
/*=================================================*/
/* D6F-PHDigital Flow Sensor Header File(using STM32) 
 *        * :Copyright: (C) OMRON Corporation, Microdevice H.Q.
 *  * :Auther   : 
 * :Revision:  $Rev$
 * :Id:        $Id$
 * :Date:      $Date$
 ** All Rights Reserved* OMRON Proprietary Right
 *=================================================*/
/*=======================*/
/* for General           */
/*=======================*/
#define SA_7 0x6C // for 7bit Slave Address
#define SA_8 0xD8 // for 7bit Slave Address
#define RANGE_MODE 100  // Full Range +/-50[Pa]
//#define RANGE_MODE 250  // Full Range0-250[Pa]
//#define RANGE_MODE 1000 // Full Range +/-500[Pa]

/*=======================*/
/* for Measure Mode      */
/*=======================*/
#define PRESSURE_MODE 1  // Pressure mode
#define TEMPERATURE_MODE 2  // Temperature mode

/* Function prototypes -------------------------------------------------------*/
void D6F_PHInitialize(void);
void D6FStartMeasurement(void);
float D6FPress_meas(void);
short D6FTemp_meas(void);
#endif	/* UTLN_D6F_PH_H */

