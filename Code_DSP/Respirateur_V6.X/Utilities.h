#ifndef UTILITIES_H
#define	UTILITIES_H

#include "UTLN_Typedefs.h"
float getFloat(unsigned char *p, int index);
double getDouble(unsigned char *p, int index);

void getBytesFromFloat(unsigned char *p, int index, float f);
void getBytesFromDouble(unsigned char *p, int index, double d);
void getBytesFromInt32(unsigned char *p, int index, int32_t in);

double LimitToInterval(double value, double min, double max);
double Modulo2PIAngleRadian(double angleRadian);

#endif	//UTILITIES_H