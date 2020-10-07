/* 
 * File:   oscillator.h
 * Author: Valentin
 *
 * Created on 28 avril 2016, 23:14
 */

#ifndef OSCILLATOR_H
#define	OSCILLATOR_H

#define FCY 40000000

#define __delay_ms(d) \
  { __delay32( (unsigned long) (((unsigned long long) d)*(FCY)/1000ULL)); }
#define __delay_us(d) \
  { __delay32( (unsigned long) (((unsigned long long) d)*(FCY)/1000000ULL)); }


void InitOscillator();


#endif	/* OSCILLATOR_H */

