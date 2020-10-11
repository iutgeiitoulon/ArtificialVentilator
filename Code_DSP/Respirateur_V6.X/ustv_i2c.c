#include "ustv_i2c.h"
#include <i2c.h>
#include "ustv_led.h"
#include "define.h"

//#include <p33FJ128MC804.h>
#include <delay.h>
#include <Libpic30.h>

#ifdef __PIC24FJ64GB004__
    #define I2CCONbits I2C1CONbits
#endif

void i2c_wait(unsigned int cnt)
{
	while(--cnt)
	{
		asm( "nop" );
		asm( "nop" );
	}
}
void InitI2C1(void)
{
    IdleI2C1();

    /* Baud rate is set for 100 kHz */
    double Fscl;
    //Fscl= 400000;
    Fscl= 400000;

    // Configure I2C1CON register
    #ifdef __PIC24FJ64GB004__
        I2C1CONbits.I2CEN = 0;      //Module I2C arrêté
        I2C1CONbits.I2CSIDL = 0;    // 1 = Discontinue module operation when device enters an Idle mode
                                    // 0 = Continue module operation in Idle mode
        I2C1CONbits.SCLREL = 0;     // operating as i2C slave only
                                    // 1 = Release SCLx clock
                                    // 0 = Hold SCLx clock low (clock stretch)
        I2C1CONbits.IPMIEN = 0;     // 1 = IPMI Support mode is enabled; all addresses Acknowledged
                                    // 0 = IPMI Support mode disabled
        I2C1CONbits.A10M = 0;       // 1 = I2CxADD is a 10-bit slave address
                                    // 0 = I2CxADD is a 7-bit slave address
        I2C1CONbits.DISSLW = 0;     // 1 = Slew rate control disabled
                                    // 0 = Slew rate control enabled
        I2C1CONbits.SMEN = 0;       // 1 = Enable I/O pin thresholds compliant with SMBus specification
                                    // 0 = Disable SMBus input thresholds
        I2C1CONbits.GCEN = 0;       // 1 = Enable interrupt when a general call address is received in the I2CxRSR (module is enabled for reception)
                                    // 0 = General call address disabled
        I2C1CONbits.STREN = 1;      // 1 = Enable software or receive clock stretching
                                    // 0 = Disable software or receive clock stretching
        I2C1CONbits.ACKDT = 1;      // 1 = Send NACK during Acknowledge
                                    // 0 = Send ACK during Acknowledge

        /*config1 = (
                I2C_ON &
                I2C_IDLE_CON &
                I2C_CLK_HLD &
                I2C_IPMI_DIS &
                I2C_7BIT_ADD &
                I2C_SLW_EN &
                I2C_SM_DIS &
                I2C_GCALL_DIS &
                I2C_STR_EN &
                I2C_NACK &
                I2C_ACK_DIS &
                I2C_RCV_DIS &
                I2C_STOP_DIS &
                I2C_RESTART_DIS &
                I2C_START_DIS);
         */
    #elif defined (__dsPIC33FJ128MC804__)
        config1 = (I2C1_ON & I2C1_IDLE_CON & I2C1_CLK_HLD &
         I2C1_IPMI_DIS & I2C1_7BIT_ADD &
         I2C1_SLW_DIS & I2C1_SM_DIS &
         I2C1_GCALL_DIS & I2C1_STR_EN &
         I2C1_NACK & I2C1_ACK_DIS & I2C1_RCV_DIS &
         I2C1_STOP_DIS & I2C1_RESTART_DIS &
         I2C1_START_DIS);
    #endif

    //I2C1BRG = config2 = (int)(FCY/Fscl - FCY/1111111)+5; // 30,6 -> 31
    //I2C1BRG = (int)(FCY/Fscl - FCY/10000000-1);  //37.4 -> 37 //cf. datasheet
    I2C1BRG = 34;               // Valeur donnant réellement 400kHz
    I2C1CONbits.I2CEN = 1;      // Module I2C démarré

    //OpenI2C1(config1,config2);
}

//******************************************************************************
//* Function Name: I2CByteRead *
//* Return Value: error condition status *
//* Parameters: EE memory device control, address and data *
//* bytes. *
//* Description: Write single data byte to I2C EE memory *
//* device. This routine can be used for any I2C *
//* EE memory device, which uses 2 bytes of *
//* address data as in the 24LC32A/64A/256A. *
//* *
//******************************************************************************

void I2C1ReadN( unsigned char slaveAddress, unsigned char registerAddress, volatile unsigned char* data, unsigned int length )
{
    unsigned char i2cData[2];
    int DataSz;
    int index;
    i2cData[0] = (slaveAddress & 0xFE);	//Device Address & WR
    i2cData[1] = registerAddress;	//eeprom low address byte
    DataSz = 2;
    StartI2C1();
    IdleI2C1();

    //send the address to read from
    index = 0;
    while( DataSz )
    {
            MasterWriteI2C1( i2cData[index++] );
            IdleI2C1();		//Wait to complete

            DataSz--;

            // ACKSTAT is 0 when slave acknowledge,
            // if 1 then slave has not acknowledge the data.
            if( I2C1STATbits.ACKSTAT )
                    break;
    }

    //now send a start sequence again
    RestartI2C1();	//Send the Restart condition
    i2c_wait(10);
    //wait for this bit to go back to zero
    IdleI2C1();	//Wait to complete

    MasterWriteI2C1( slaveAddress | 0x01 ); //transmit read command
    IdleI2C1();		//Wait to complete

    // read some bytes back
    MastergetsI2C1(length, (unsigned char* )data, 20);

    StopI2C1();	//Send the Stop condition
    IdleI2C1();	//Wait to complete
    /*
     *	End reading several bytes. [3]
     */

}


void I2C1Read1( unsigned char slaveAddress, unsigned char registerAddress, volatile unsigned char* data )
{
    I2C1ReadN( slaveAddress, registerAddress, data, 1);
}

void I2C1WriteN( unsigned char slaveAddress, unsigned char registerAddress, unsigned char* data, unsigned int length )
{
    unsigned char txData[length+2];
    int DataSz;
    int index = 0;
    txData[0] = (slaveAddress & 0xFE);
    txData[1] = registerAddress;
    int i;
    for(i=0;i<length;i++)
    {
        txData[i+2]=data[i];
    }
    DataSz=length+2;

    StartI2C1();
    IdleI2C1();

    while( DataSz )
    {
        MasterWriteI2C1( txData[index++] );
        IdleI2C1();		//Wait to complete

        DataSz--;

        //ACKSTAT is 0 when slave acknowledge,
        //if 1 then slave has not acknowledge the data.
        if( I2C1STATbits.ACKSTAT )
                break;
    }
    StopI2C1();	//Send the Stop condition
    IdleI2C1();	//Wait to complete
}

void I2C1Write1( unsigned char slaveAddress, unsigned char registerAddress, unsigned char data)
{
    unsigned char tab[1];
    tab[0]=data;
    I2C1WriteN( slaveAddress, registerAddress, tab, 1) ;
}

/*void __attribute__((interrupt, no_auto_psv)) _MI2C1Interrupt(void)
{
    // Insert ISR code here
    IFS1bits.MI2C1IF = 0; // Clear CN interrupt
    if(I2C1STATbits.BCL==1)
    {
            int value = I2C1STAT & 0b1111101111111111;
            I2C1STAT=value;
            InitI2C1();
    }
}*/



