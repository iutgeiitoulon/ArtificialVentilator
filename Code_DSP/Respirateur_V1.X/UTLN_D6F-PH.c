#include "UTLN_D6F-PH.h"
#include "ustv_i2c_interrupt.h"
#include "oscillator.h"

short RD_FIFO;  /* 16bit data width */
unsigned short uRD_FIFO; /* 16bit data width */
uint8_t RD_REG;   /*  8bit data width */
char  setting_done_flag = 0;

// Dummy wait routine
void adc_wait(volatile unsigned long delay){ 
    __delay_ms(delay);
}

/*=================================================*/
/* Initialize Function                             */
/* Usage        : Initialize( void )               */
/* Argument     : Null                             */
/* Return value : T.B.D                            */
/*=================================================*/
void D6F_PHInitialize( void )
{
    /* EEPROM Control <= 00h */
    unsigned char send1[] = { 0x00};
    I2C1WriteNInterrupt(SA_8,0x0B, send1, 1);
    
    adc_wait(5);
    
}

void D6FStartMeasurement(void)
{
    unsigned char send2[]={0xD0, 0x40, 0x18, 0x06};
    I2C1WriteNInterrupt(SA_8,0x00, send2, 4);
}
/*=======================================================*/
/* Pressure measure Function                             */
/* Usage        : Press_meas( void )                     */
/* Argument     : NULL                                   */
/* Return value : Compensated Pressure value(unsigned)   */
/*=======================================================*/
float D6FPress_meas(void)
{
    unsigned short rd_fifo;
    float rd_flow;
    unsigned long  wait_time;
    
    unsigned char read[2];
    
    D6FStartMeasurement();
    
    /* [D040] <= 06h */
    unsigned char send2[] = { 0xD0, 0x51, 0x2c};
    
    I2C1WriteNInterrupt( SA_8, 0x00, send2, 3 );
    wait_time   = 33; /*33msec wait */
    /* wait time depend on resolution mode */
    //adc_wait(wait_time);

    /* [D051/D052] => Read Compensated Flow value */
    unsigned char send3[] = {0x07, 0xD0, 0x51, 0x2C, 0x07};

    I2C1WriteNReadNInterrupt( SA_8, send3, 1, read, 2);
    
    rd_fifo = ((read[0] << 8) | read[1]);
    // Press Mode : [Pa] = (xx[count] -1024) * Full Range [Pa]/ 60000 -Full Range [Pa] at other
    if (RANGE_MODE == 250) 
    {
        rd_flow = ((rd_fifo -1024) * RANGE_MODE *10.0/60000.0); /* convert to [Pa] */
    }
    else
    {
        rd_flow = (((float)rd_fifo -1024) / 600.0) - 50; /* convert to [Pa] */
    }
    return rd_flow;
}

short D6FTemp_meas(void)
{
    unsigned char read[2];
    short    rd_temp;
    unsigned long wait_time;
    /* [D040] <= 06h */
    unsigned char send2[] = { 0xD0, 0x40, 0x18, 0x06};

    I2C1WriteNInterrupt(SA_8, 0x00,send2, 4);
    /* wait time depend on resolution mode */
    wait_time  = 33; /* 33msec wait */
    adc_wait(wait_time);
    /* [D061/D062] => Read TMP_H/TMP_L value */
    unsigned char send3[] = {0x00, 0xD0, 0x61, 0x2C, 0x07};

    I2C1WriteNReadNInterrupt( SA_8, send3, 5, read, 2);
    RD_FIFO = ((read[0] << 8) | read[1]);
    rd_temp = ((RD_FIFO -10214)*1000 / 3739); // convert to degree-C(x10)
    return rd_temp;
}

void D6FHarwareReset(void)
{
    I2C1Write1(SA_8,0x0D,0x80);
}