using System;
using EventArgsLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Constants;
using Utilities;

namespace MessageGenerator
{
    public class MsgGenerator
    {
        //Input events
        public void GenerateMessageSartStop(object sender, BoolEventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = Convert.ToByte(e.value);
            OnMessageToRespirator((Int16)Commands.START, 1, payload);
        }

        public void GenerateMessageSTOP(object sender, BoolEventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = Convert.ToByte(e.value);

            OnMessageToRespirator((Int16)Commands.EmergencySTOP, 1, payload);
        }

        public void GenerateMessageDoStepsUpDown(object sender, MotorDoStepsArgs e)
        {
            byte[] payload = new byte[54];
            payload[0] = Convert.ToByte(e.motorNum);
            payload.SetValueRange(e.steps.GetBytes(), 1);

            OnMessageToRespirator((Int16)Commands.DoSteps, 5, payload);
        }
       

        public void GenerateMessageResetStepsCounter(object sender, EventArgs e)
        {
            OnMessageToRespirator((Int16)Commands.ResetStepsCounter, 0, null);
        }

        public void GenerateMessageSetStepsOffsetUp(object sender, Int32EventArgs e)
        {
            byte[] payload = new byte[4];
            payload = e.value.GetBytes();

            OnMessageToRespirator((Int16)Commands.SetStepsOffsetFromUp, 4, payload);
        }

        public void GenerateMessageSetStepsOffsetDown(object sender, Int32EventArgs e)
        {
            byte[] payload = new byte[4];
            payload = e.value.GetBytes();

            OnMessageToRespirator((Int16)Commands.SetStepsOffsetFromDown, 4, payload);
        }

        public void GenerateMessageSetStepsAmplitude(object sender, Int32EventArgs e)
        {
            byte[] payload = new byte[4];
            payload = e.value.GetBytes();

            OnMessageToRespirator((Int16)Commands.SetAmplitudeSteps, 4, payload);
        }

        public void GenerateMessageSetSpeed(object sender, DoubleArgs e)
        {
            byte[] payload = new byte[4];
            payload = ((float)(e.Value)).GetBytes();

            OnMessageToRespirator((Int16)Commands.ChangeSpeed, 4, payload);
        }

        public void GenerateMessageSetPauseTimeUp(object sender, DoubleArgs e)
        {
            byte[] payload = new byte[4];
            payload = ((float)(e.Value)).GetBytes();

            OnMessageToRespirator((Int16)Commands.SetPauseTimeUp, 4, payload);
        }

        public void GenerateMessageSetPauseTimeDown(object sender, DoubleArgs e)
        {
            byte[] payload = new byte[4];
            payload = ((float)(e.Value)).GetBytes();

            OnMessageToRespirator((Int16)Commands.SetPauseTimeDown, 4, payload);
        }

        public void GenerateMessageSetPlimit(object sender, DoubleArgs e)
        {
            byte[] payload = new byte[4];
            payload = ((float)(e.Value)).GetBytes();

            OnMessageToRespirator((Int16)Commands.SetPlimit, 4, payload);
        }

        public void GenerateMessageSetVlimit(object sender, DoubleArgs e)
        {
            byte[] payload = new byte[4];
            payload = ((float)(e.Value)).GetBytes();

            OnMessageToRespirator((Int16)Commands.SetVlimit, 4, payload);
        }

        public void GenerateMessageSetMode(object sender, BoolEventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = (byte)((e.value) ? 1 : 0);

            OnMessageToRespirator((Int16)Commands.SetMode, 1, payload);
        }

        public void GenerateMessageInitMachine(object sender, EventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = 0;

            OnMessageToRespirator((Int16)Commands.Init, 1, payload);
        }

        public void GenerateMessageSetCyclesPerMin(object sender, Int32EventArgs e)
        {
            byte[] payload = new byte[1];
            payload[0] = Convert.ToByte(e.value);

            OnMessageToRespirator((Int16)Commands.SetCyclesPerMin, 1, payload);
        }

        public void GenerateMessageSetSeuilDetection(object sender, DoubleArgs e)
        {
            byte[] payload = new byte[4];
            payload = ((float)(e.Value*100)).GetBytes();

            OnMessageToRespirator((Int16)Commands.SetSeuilAssistance, 4, payload);
        }
        //Output events
        public event EventHandler<MessageToRespirateurArgs> OnMessageToRespirateurGeneratedEvent;
        public virtual void OnMessageToRespirator(Int16 msgFunction, Int16 msgPayloadLength, byte[] msgPayload)
        {
            var handler = OnMessageToRespirateurGeneratedEvent;
            if (handler != null)
            {
                handler(this, new MessageToRespirateurArgs { MsgFunction = msgFunction, MsgPayloadLength = msgPayloadLength, MsgPayload=msgPayload});
            }
        }
    }


}
