#ifndef USTV_I2C_H
#define USTV_I2C_H
	
void InitI2C1(void);
void I2C1WriteN( unsigned char slaveAddress, unsigned char registerAddress, unsigned char* chaine, unsigned int length );
void I2C1Write1( unsigned char slaveAddress, unsigned char registerAddress, unsigned char data) ;
void I2C1ReadN( unsigned char slaveAddress, unsigned char registerAddress, volatile unsigned char* data, unsigned int length );
void I2C1Read1( unsigned char slaveAddress, unsigned char registerAddress, volatile unsigned char* data );


#endif
