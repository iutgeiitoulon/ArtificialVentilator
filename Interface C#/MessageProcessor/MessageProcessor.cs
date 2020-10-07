using Constants;
using EventArgsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Utilities;

namespace MessageProcessor
{
    public class MsgProcessor
    {
        Timer tmrComptageMessage;
        public MsgProcessor()
        {
            tmrComptageMessage = new Timer(1000);
            tmrComptageMessage.Elapsed += TmrComptageMessage_Elapsed;
            tmrComptageMessage.Start();
        }

        int nbMessageIMUReceived = 0;
        int nbMessageSpeedReceived = 0;
        private void TmrComptageMessage_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnMessageCounter(nbMessageIMUReceived, nbMessageSpeedReceived);
            nbMessageIMUReceived = 0;
            nbMessageSpeedReceived = 0;
        }

        //Input CallBack        
        public void ProcessDecodedMessage(object sender, MessageDecodedArgs e)
        {
            ProcessDecodedMessage((Int16)e.MsgFunction,(Int16) e.MsgPayloadLength, e.MsgPayload);
        }
        //Processeur de message en provenance du respirateur...
        //Une fois processé, le message sera transformé en event sortant
        public void ProcessDecodedMessage(Int16 command, Int16 payloadLength, byte[] payload)
        {
            byte[] tab;
            uint timeStamp;
            switch (command)
            {
                case (short)Commands.START:
                    bool started = Convert.ToBoolean(payload[0]);
                   
                    OnStartStopCallBackFromRespirator( started);
                    
                    break;
                case (short)Commands.SetAmplitudeSteps:
                     Int32 amplitude = (payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24);
                    OnSetAmplitudeCallBackFromRespirator(amplitude);
                    break;

                case (short)Commands.SetPauseTimeUp:
                    byte[] tab2 = payload.GetRange(0, 4);
                    float pauseTimeUp = tab2.GetFloat();
                    OnSetPauseTimeUpCallBackFromRespirator(pauseTimeUp);
                    break;

                case (short)Commands.SetPauseTimeDown:
                    tab2 = payload.GetRange(0, 4);
                    float pauseTimeDown = tab2.GetFloat();
                    OnSetPauseTimeDownCallBackFromRespirator(pauseTimeDown);
                    break;
                case (short)Commands.SetStepsOffsetFromUp:
                    Int32 offset = (payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24);
                    OnSetOffsetUpCallBackFromRespirator(offset);
                    break;
                case (short)Commands.SetStepsOffsetFromDown:
                    offset = (payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24);
                    OnSetOffsetDownCallBackFromRespirator(offset);
                    break;
                case (short)Commands.SetCyclesPerMin:
                    byte cyclesPerMin =  payload[0];
                    OnSetOffsetDownCallBackFromRespirator(cyclesPerMin);
                    break;
                case (short)Commands.ChangeSpeed:
                    tab2 = payload.GetRange(0, 4);
                    float speed = tab2.GetFloat();
                    OnSetSpeedCallBackFromRespirator(speed);
                    break;
                case (short)Commands.PressureDataFromRespirator:
                    {
                        uint time2 = (uint)(payload[3] | payload[2] << 8 | payload[1] << 16 | payload[0] << 24);
                        tab2 = payload.GetRange(4, 4);
                        float sensor1Pressure = tab2.GetFloat();
                        tab2 = payload.GetRange(8, 4);
                        float sensor2Pressure = tab2.GetFloat();
                        tab2 = payload.GetRange(12, 4);
                        float sensorAmbiantPressure = tab2.GetFloat();
                        //On envois l'event aux abonnés
                        OnPressureDataFromRespirator(time2, sensor1Pressure, sensor2Pressure, sensorAmbiantPressure);
                    }
                    break;


                case (short)Commands.SetMode:
                    if (payload[0]==1)
                        OnSetModeCallBackFromRespirator(true);
                    else
                        OnSetModeCallBackFromRespirator(false);
                    break;

                case (short)Commands.ErrorTextMessage:
                    string errorMsg = Encoding.UTF8.GetString(payload);
                    //On envois l'event aux abonnés
                    OnErrorTextFromRespirateur(errorMsg);
                    break;

                case (short)Commands.SetPlimit:
                    tab2 = payload.GetRange(0, 4);
                    float plimite = tab2.GetFloat();
                    OnSetPLimiteCallBackFromRespirator(plimite);
                    break;
                case (short)Commands.SetVlimit:
                    tab2 = payload.GetRange(0, 4);
                    float vLimite = tab2.GetFloat();
                    OnSetVLimiteCallBackFromRespirator(vLimite);
                    break;
                case (short)Commands.SetSeuilAssistance:
                    tab2 = payload.GetRange(0, 4);
                    float seuil = tab2.GetFloat();
                    OnSetPSeuilCallBackFromRespirator(seuil);
                    break;
                default: break;
            }
        }

        public event EventHandler<BoolEventArgs> OnStartStopCallBackFromRespiratorGeneratedEvent;
        public virtual void OnStartStopCallBackFromRespirator(bool started)
        {
            var handler = OnStartStopCallBackFromRespiratorGeneratedEvent;
            if (handler != null)
            {
                handler(this, new BoolEventArgs { value=started });
            }
        }

        public event EventHandler<Int32EventArgs> OnSetAmplitudeCallBackFromRespiratorGeneratedEvent;
        public virtual void OnSetAmplitudeCallBackFromRespirator(Int32 amplitude)
        {
            var handler = OnSetAmplitudeCallBackFromRespiratorGeneratedEvent;
            if (handler != null)
            {
                handler(this, new Int32EventArgs { value = amplitude });
            }
        }

        public event EventHandler<DoubleArgs> OnSetPauseTimeUpCallBackFromRespiratorGeneratedEvent;
        public virtual void OnSetPauseTimeUpCallBackFromRespirator(double timeUp)
        {
            var handler = OnSetPauseTimeUpCallBackFromRespiratorGeneratedEvent;
            if (handler != null)
            {
                handler(this, new DoubleArgs { Value = timeUp });
            }
        }

        public event EventHandler<DoubleArgs> OnSetPauseTimeDownCallBackFromRespiratorGeneratedEvent;
        public virtual void OnSetPauseTimeDownCallBackFromRespirator(double timeDown)
        {
            var handler = OnSetPauseTimeDownCallBackFromRespiratorGeneratedEvent;
            if (handler != null)
            {
                handler(this, new DoubleArgs { Value = timeDown });
            }
        }

        public event EventHandler<Int32EventArgs> OnSetOffsetUpCallBackFromRespiratorGeneratedEvent;
        public virtual void OnSetOffsetUpCallBackFromRespirator(Int32 offset)
        {
            var handler = OnSetOffsetUpCallBackFromRespiratorGeneratedEvent;
            if (handler != null)
            {
                handler(this, new Int32EventArgs { value = offset });
            }
        }
        public event EventHandler<ByteEventArgs> OnSetCyclesPerMinCallBackFromRespiratorGeneratedEvent;
        public virtual void OnSetCyclesPerMinCallBackFromRespirator(byte cycles)
        {
            var handler = OnSetCyclesPerMinCallBackFromRespiratorGeneratedEvent;
            if (handler != null)
            {
                handler(this, new ByteEventArgs { Value = cycles });
            }
        }

        public event EventHandler<BoolEventArgs> OnSetModeCallBackFromRespiratorGeneratedEvent;
        public virtual void OnSetModeCallBackFromRespirator(bool isAssistance)
        {
            var handler = OnSetModeCallBackFromRespiratorGeneratedEvent;
            if (handler != null)
            {
                handler(this, new BoolEventArgs { value = isAssistance });
            }
        }

        public event EventHandler<Int32EventArgs> OnSetOffsetDownCallBackFromRespiratorGeneratedEvent;
        public virtual void OnSetOffsetDownCallBackFromRespirator(Int32 offset)
        {
            var handler = OnSetOffsetDownCallBackFromRespiratorGeneratedEvent;
            if (handler != null)
            {
                handler(this, new Int32EventArgs { value = offset });
            }
        }

        public event EventHandler<DoubleArgs> OnSetSpeedCallBackFromRespiratorGeneratedEvent;
        public virtual void OnSetSpeedCallBackFromRespirator(double speed)
        {
            var handler = OnSetSpeedCallBackFromRespiratorGeneratedEvent;
            if (handler != null)
            {
                handler(this, new DoubleArgs { Value = speed });
            }
        }

        public event EventHandler<StringEventArgs> OnErrorTextFromRespirateurGeneratedEvent;
        public virtual void OnErrorTextFromRespirateur(string str)
        {
            var handler = OnErrorTextFromRespirateurGeneratedEvent;
            if (handler != null)
            {
                handler(this, new StringEventArgs { value = str });
            }
        }

        //Output events
        public event EventHandler<RespirateurDataEventArgs> OnPressureDataFromRespiratorGeneratedEvent;
        public virtual void OnPressureDataFromRespirator(uint timeStamp, double pressureSensor1, double pressureSensor2, double pressureSensorAmbiant)
        {
            var handler = OnPressureDataFromRespiratorGeneratedEvent;
            if (handler != null)
            {
                handler(this, new RespirateurDataEventArgs { EmbeddedTimeStampInMs = timeStamp, pressureSensor1 = pressureSensor1, pressureSensor2 = pressureSensor2, pressureSensorAmbiant= pressureSensorAmbiant });
            }
        }

        public event EventHandler<DoubleArgs> OnSetPLimiteCallBackFromRespiratorGeneratedEvent;
        public virtual void OnSetPLimiteCallBackFromRespirator(double limit)
        {
            var handler = OnSetPLimiteCallBackFromRespiratorGeneratedEvent;
            if (handler != null)
            {
                handler(this, new DoubleArgs { Value = limit });
            }
        }

        public event EventHandler<DoubleArgs> OnSetVLimiteCallBackFromRespiratorGeneratedEvent;
        public virtual void OnSetVLimiteCallBackFromRespirator(double limit)
        {
            var handler = OnSetVLimiteCallBackFromRespiratorGeneratedEvent;
            if (handler != null)
            {
                handler(this, new DoubleArgs { Value = limit });
            }
        }

        public event EventHandler<DoubleArgs> OnSetPSeuilCallBackFromRespiratorGeneratedEvent;
        public virtual void OnSetPSeuilCallBackFromRespirator(double seuil)
        {
            var handler = OnSetPSeuilCallBackFromRespiratorGeneratedEvent;
            if (handler != null)
            {
                handler(this, new DoubleArgs { Value = seuil });
            }
        }
        public event EventHandler<MsgCounterArgs> OnMessageCounterEvent;
        public virtual void OnMessageCounter(int nbMessageFromImu, int nbMessageFromOdometry)
        {
            var handler = OnMessageCounterEvent;
            if (handler != null)
            {
                handler(this, new MsgCounterArgs { nbMessageIMU = nbMessageFromImu, nbMessageOdometry = nbMessageFromOdometry });
            }
        }
    }
}
