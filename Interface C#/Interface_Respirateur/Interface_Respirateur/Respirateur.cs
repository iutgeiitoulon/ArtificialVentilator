using SciChart.Charting.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WpfRespirateur_Interface;
using WpfRespirateur_Interface_Monitor;
using MessageGenerator;
using MessageProcessor;
using ExtendedSerialPort;
using System.IO.Ports;
using MessageDecoder;
using MessageEncoder;
using System.Windows.Threading;


namespace Interface_Respirateur
{
    class Respirateur
    {
        static object ExitLock = new object();

        static ReliableSerialPort serialPort1;
        static MsgDecoder msgDecoder;
        static MsgEncoder msgEncoder;
        static MsgGenerator msgGenerator;
        static MsgProcessor msgProcessor;

        static WpfRespirateurInterface respirateurInterface;            //Pour debug

        static WpfRespirateurMonitor respirateurInterfaceMonitor;     //Pour practicien

        static AdvancedTimers.HighFreqTimer timerSimulation;

        [STAThread] //à ajouter au projet initial
        static void Main(string[] args)
        {
            SciChartSurface.SetRuntimeLicenseKey(@"<LicenseContract>
              <Customer>University of  Toulon</Customer>
              <OrderId>EDUCATIONAL-USE-0109</OrderId>
              <LicenseCount>1</LicenseCount>
              <IsTrialLicense>false</IsTrialLicense>
              <SupportExpires>11/04/2019 00:00:00</SupportExpires>
              <ProductCode>SC-WPF-SDK-PRO-SITE</ProductCode>
              <KeyCode>lwABAQEAAABZVzOfQ0zVAQEAewBDdXN0b21lcj1Vbml2ZXJzaXR5IG9mICBUb3Vsb247T3JkZXJJZD1FRFVDQVRJT05BTC1VU0UtMDEwOTtTdWJzY3JpcHRpb25WYWxpZFRvPTA0LU5vdi0yMDE5O1Byb2R1Y3RDb2RlPVNDLVdQRi1TREstUFJPLVNJVEWDf0QgB8GnCQXI6yAqNM2njjnGbUt2KsujTDzeE+k69K1XYVF1s1x1Hb/i/E3GHaU=</KeyCode>
            </LicenseContract>");

            serialPort1 = new ReliableSerialPort("COM19", 115200, Parity.None, 8, StopBits.One);
            msgDecoder = new MsgDecoder();
            msgEncoder = new MsgEncoder();
            msgGenerator = new MsgGenerator();
            msgProcessor = new MsgProcessor();


            //Gestion des messages envoyé par le respirateur
            msgGenerator.OnMessageToRespirateurGeneratedEvent += msgEncoder.EncodeMessageToRespirateur;
            msgEncoder.OnMessageEncodedEvent += serialPort1.SendMessage;

            //Gestion des messages reçu par le respirateur
            serialPort1.OnDataReceivedEvent += msgDecoder.DecodeMsgReceived;
            msgDecoder.OnMessageDecodedEvent += msgProcessor.ProcessDecodedMessage;
            

            StartInterface();
            

            lock (ExitLock)
            {
                // Do whatever setup code you need here
                // once we are done wait
                System.Threading.Monitor.Wait(ExitLock);
            }
        }

        static Thread t1;
        static void StartInterface()
        {
            
                t1 = new Thread(() =>
                {
                    //Attention, il est nécessaire d'ajouter PresentationFramework, PresentationCore, WindowBase and your wpf window application aux ressources.
                    respirateurInterfaceMonitor = new WpfRespirateurMonitor();
                    respirateurInterfaceMonitor.Loaded += RegisterRespirateurMonitorEvents;
                    respirateurInterfaceMonitor.ShowDialog();
                });
                t1.SetApartmentState(ApartmentState.STA);
            t1.Start();
           
        }

        static Thread t2;
        static bool t2Started = false;
        static void StartAdvancedInterface(object sender, EventArgs args)
        {
            if (!t2Started)
            {
               
                    t2 = new Thread(() =>
                    {
                        //Attention, il est nécessaire d'ajouter PresentationFramework, PresentationCore, WindowBase and your wpf window application aux ressources.
                        respirateurInterface = new WpfRespirateurInterface();

                        //respirateurInterface.Loaded += RegisterRespirateurEvents;
                        respirateurInterface.ShowDialog();

                        t2Started = false;
                    });
                    t2.SetApartmentState(ApartmentState.STA);
                    t2.Start();
                    t2Started = true;
            }
        }
        static void RegisterRespirateurEvents(object sender, EventArgs e)
        {
            respirateurInterface.OnStartStopFromInterfaceGeneratedEvent += msgGenerator.GenerateMessageSartStop;
            respirateurInterface.OnDoStepFromInterfaceGeneratedEvent += msgGenerator.GenerateMessageDoStepsUpDown;
            respirateurInterface.OnSetAmplitudeFromInterfaceGeneratedEvent += msgGenerator.GenerateMessageSetStepsAmplitude;
            respirateurInterface.OnSetOffsetDownFromInterfaceGeneratedEvent += msgGenerator.GenerateMessageSetStepsOffsetDown;
            respirateurInterface.OnSetOffsetUpFromInterfaceGeneratedEvent += msgGenerator.GenerateMessageSetStepsOffsetUp;
            respirateurInterface.OnSetPauseTimeDownFromInterfaceGeneratedEvent += msgGenerator.GenerateMessageSetPauseTimeDown;
            respirateurInterface.OnSetPauseTimeUpFromInterfaceGeneratedEvent += msgGenerator.GenerateMessageSetPauseTimeUp;
            respirateurInterface.OnSetSpeedFromInterfaceGeneratedEvent += msgGenerator.GenerateMessageSetSpeed;

            msgProcessor.OnPressureDataFromRespiratorGeneratedEvent += respirateurInterface.UpdateRespirationDataOnGraph;

            //Callback (confirmation reglage parametres)
            msgProcessor.OnStartStopCallBackFromRespiratorGeneratedEvent += respirateurInterface.ActualizeStartStopButton;
            msgProcessor.OnSetAmplitudeCallBackFromRespiratorGeneratedEvent += respirateurInterface.ActualizeAmplitudeLabel;
            msgProcessor.OnSetOffsetDownCallBackFromRespiratorGeneratedEvent += respirateurInterface.ActualizeOffsetDown;
            msgProcessor.OnSetOffsetUpCallBackFromRespiratorGeneratedEvent += respirateurInterface.ActualizeOffsetUp;
            msgProcessor.OnSetPauseTimeDownCallBackFromRespiratorGeneratedEvent += respirateurInterface.ActualizePauseTimeDown;
            msgProcessor.OnSetPauseTimeUpCallBackFromRespiratorGeneratedEvent += respirateurInterface.ActualizePauseTimeUp;
            msgProcessor.OnSetSpeedCallBackFromRespiratorGeneratedEvent += respirateurInterface.ActualizeSpeed;
        }

        static void RegisterRespirateurMonitorEvents(object sender, EventArgs e)
        {
            respirateurInterfaceMonitor.OnStartStopFromInterfaceGeneratedEvent += msgGenerator.GenerateMessageSartStop;
            respirateurInterfaceMonitor.OnSetCyclesPerMinromInterfaceGeneratedEvent += msgGenerator.GenerateMessageSetCyclesPerMin;
            respirateurInterfaceMonitor.OnSetVlimitFromInterfaceGeneratedEvent += msgGenerator.GenerateMessageSetVlimit;
            respirateurInterfaceMonitor.OnSetPlimitFromInterfaceGeneratedEvent += msgGenerator.GenerateMessageSetPlimit;
            respirateurInterfaceMonitor.OnStartAdvancedInterfaceFromInterfaceGeneratedEvent += StartAdvancedInterface;
            respirateurInterfaceMonitor.OnSetModeFromInterfaceGeneratedEvent += msgGenerator.GenerateMessageSetMode;
            respirateurInterfaceMonitor.OnInitMachineFromInterfaceGeneratedEvent += msgGenerator.GenerateMessageInitMachine;
            respirateurInterfaceMonitor.OnChangeSeuilFromInterfaceGeneratedEvent += msgGenerator.GenerateMessageSetSeuilDetection;


            //Callback (confirmation reglage parametres)
            msgProcessor.OnStartStopCallBackFromRespiratorGeneratedEvent += respirateurInterfaceMonitor.ActualizeStartStopButton;
            msgProcessor.OnSetModeCallBackFromRespiratorGeneratedEvent += respirateurInterfaceMonitor.ActualizeMode;
            msgProcessor.OnPressureDataFromRespiratorGeneratedEvent += respirateurInterfaceMonitor.UpdateVolumeDataOnGraph;
            msgProcessor.OnSetPLimiteCallBackFromRespiratorGeneratedEvent += respirateurInterfaceMonitor.ActualizePLimite;
            msgProcessor.OnSetVLimiteCallBackFromRespiratorGeneratedEvent += respirateurInterfaceMonitor.ActualizeVLimite;
            msgProcessor.OnSetPSeuilCallBackFromRespiratorGeneratedEvent += respirateurInterfaceMonitor.ActualizeSeuilDetection;
        }
    }
}
