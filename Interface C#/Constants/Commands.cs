using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constants
{
    public enum Commands
    {
        #region Commandes générales
        #pragma warning disable CS1591

        Unknown = 0,
        START=1,
        ErrorTextMessage=2,
        DoSteps=3,
        ResetStepsCounter=4,
        SetStepsOffsetFromUp = 5,
        SetStepsOffsetFromDown =6,
        SetAmplitudeSteps=7,
        SetCyclesPerMin=8,
        ChangeSpeed=20,
        SetPauseTimeUp=21,
        SetPauseTimeDown=22,
        SetVlimit = 23,
        SetPlimit = 24,
        SetMode = 25,
        Init=26,
        SetSeuilAssistance=27,


        PressureDataFromRespirator =100,
       

        EmergencySTOP=-1,


#pragma warning restore CS1591
        #endregion
    }

    
}
