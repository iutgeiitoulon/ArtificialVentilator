/* 
 * File:   pragma_config.h
 * Author: Valentin
 *
 * Created on 28 avril 2016, 23:03
 */

#ifndef PRAGMA_CONFIG_H
#define	PRAGMA_CONFIG_H

#pragma config FNOSC = 0b000    //Initial Oscillator Source Selection bits
                                //111 = Internal Fast RC (FRC) oscillator with postscaler
                                //110 = Internal Fast RC (FRC) oscillator with divide-by-16
                                //101 = LPRC oscillator
                                //100 = Secondary (LP) oscillator
                                //011 = Primary (XT, HS, EC) oscillator with PLL
                                //010 = Primary (XT, HS, EC) oscillator
                                //001 = Internal Fast RC (FRC) oscillator with PLL
                                //000 = FRC oscillator

#pragma config IESO = 1         //Two-speed Oscillator Start-up Enable bit
                                //1 = Start-up device with FRC, then automatically switch to the
                                //      user-selected oscillator source when ready
                                //0 = Start-up device with user-selected oscillator source

#pragma config POSCMD = 0b10    //Primary Oscillator Mode Select bits
                                //11 = Primary oscillator disabled
                                //10 = HS Crystal Oscillator mode
                                //01 = XT Crystal Oscillator mode
                                //00 = EC (External Clock) mode

#pragma config FCKSM = 0b01     //Clock Switching Mode bits
                                //1x = Clock switching is disabled, Fail-Safe Clock Monitor is disabled
                                //01 = Clock switching is enabled, Fail-Safe Clock Monitor is disabled
                                //00 = Clock switching is enabled, Fail-Safe Clock Monitor is enabled

#pragma config OSCIOFNC = 0     //OSC2 Pin Function bit (except in XT and HS modes)
                                //1 = OSC2 is clock output
                                //0 = OSC2 is general purpose digital I/O pin

#pragma config FWDTEN = 0       //Watchdog Timer Enable bit
                                //1 = Watchdog Timer always enabled (LPRC oscillator cannot be disabled.
                                //Clearing the SWDTEN bit in the RCON register has no effect.)
                                //0 = Watchdog Timer enabled/disabled by user software (LPRC can be
                                //disabled by clearing the SWDTEN bit in the RCON register)

#pragma config GWRP = 1         //General Segment Write-Protect bit
                                //1 = User program memory is not write-protected
                                //0 = User program memory is write-protected

#pragma config GSS = 0b11       //General Segment Code-Protect bit
                                //11 = User program memory is not code-protected
                                //10 = Standard security
                                //0x = High security

#pragma config ALTI2C = 1       //Alternate I2C pins
                                //1 = I2C mapped to SDA1/SCL1 pins
                                //0 = I2C mapped to ASDA1/ASCL1 pins

#pragma config JTAGEN = 0       //JTAG Enable bit
                                //1 = JTAG enabled
                                //0 = JTAG disabled

#pragma config ICS = 0b11       //Immediate ICD Communication Channel Select bits
                                //11 = Communicate on PGEC1 and PGED1
                                //10 = Communicate on PGEC2 and PGED2
                                //01 = Communicate on PGEC3 and PGED3
                                //00 = Reserved, do not use

#pragma config IOL1WAY = 0      //Peripheral pin select configuration
                                //1 = Allow only one reconfiguration
                                //0 = Allow multiple reconfigurations

#endif	/* PRAGMA_CONFIG_H */

