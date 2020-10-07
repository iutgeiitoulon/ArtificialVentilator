/* 
 * File:   Toolbox.h
 * Author: Valentin
 *
 * Created on 30 avril 2016, 17:01
 */

#ifndef TOOLBOX_H
#define	TOOLBOX_H

#define PI 3.141592653589793

double ModuloAngleDegre(double angleDegre);
double Modulo2PIAngleRadian(double angleRadian);
double ModuloPIAngleRadian(double angleRadian);
double Abs(double value);
double Max(double value, double value2);
double Min(double value, double value2);

double Limiteur(double value, double lowLimit, double highLimit);
double RadianToDegree(double value);
double DegreeToRadian(double value);
double RadianToMmInRotation(double value);

#endif	/* TOOLBOX_H */

