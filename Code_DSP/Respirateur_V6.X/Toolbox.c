#include <math.h>
#include "Toolbox.h"


double ModuloAngleDegre(double angleDegre)
{
    while (angleDegre > 180)
        angleDegre -= 360.0;

    while (angleDegre<-180)
        angleDegre += 360.0;

    return angleDegre;
}

double Abs(double value)
{
    if (value >= 0)
        return value;
    else return -value;
}

double Max(double value, double value2)
{
    if (value > value2)
        return value;
    else
        return value2;
}

double Min(double value, double value2)
{
    if (value < value2)
        return value;
    else
        return value2;
}


double Modulo2PIAngleRadian(double angleRadian)
{
    while (angleRadian > PI)
        angleRadian -= 2 * PI;

    while (angleRadian<-PI)
        angleRadian += 2 * PI;

    return angleRadian;
}

double ModuloPIAngleRadian(double angleRadian)
{
    while (angleRadian > PI / 2)
        angleRadian -= PI;

    while (angleRadian<-PI / 2)
        angleRadian += PI;

    return angleRadian;
}

double Limiteur(double value, double lowLimit, double highLimit)
{
    if (value > highLimit)
        value = highLimit;
    else if (value < lowLimit)
        value = lowLimit;

    return value;
}

double RadianToDegree(double value)
{
    return value / PI * 180.0;
}

double DegreeToRadian(double value)
{
    return value * PI / 180.0;
}


