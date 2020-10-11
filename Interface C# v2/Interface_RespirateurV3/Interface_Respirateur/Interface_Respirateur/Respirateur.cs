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
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in AppDelegate or application startup
            // Set this code once in AppDelegate or application startup
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in App.xaml.cs or application startup
            SciChartSurface.SetRuntimeLicenseKey("htr4prsPz/Td3FK3/AurDqCQrTE2jgwFjhFBVVXMvIKWmRJ3fEz5RnW6MCzGf5c0sdFSGEkFMNLWn+yJv9MRt0SaZP1t5e0C7o6TsjNtuotFqgS6keJYndoaaVoEV+J095DIFSxn8KDDibRLgRYiTO+PDvfIxlOFV2zdKcMYveB1HMJqxa8X5eNgcO0mTAmIYs9TOeaoa09Rzj15xMfOHYcOFvTeSuYDC1t5oIM9qnTqKKihC/4d8uYRzCaSuoflTKiL0A2b7IpTb/1ayFHMHPPYEFc1mmjboC1x1C6l5rarPaJBlfZP2kZUOwR+7AG6F0PDBhAwX/xLnEX1Z/Ijb+upXiSOVW6Mss9Wc/WlEBQE5iY/F0Uvs7UnNRco5LqbKeK2qPn0RDwAPuSPkCjfb1etqxYFWu7zCpkgJ/dKcAopst9VkzNtxzFUrzPp8cd1CjewQ19MscTW42Ip4OyTS55FbE02Qfh7QzZLC24Ca55T1QqsL4cx5+2PQsZYsso8FQ==");
            //SciChartSurface.SetRuntimeLicenseKey("d5LxyOnankWo/T3eKtzC21Tya6NHt33F9l4FYjTyb2UekJiUGgfEz7/b5WpRlc1ZiOd6phLwjGdRp3F2H5yLm1IHW+axuBaFjhhXXAeZVkFujziMXokyLRyAV8SMFuHNo+1f/2WRzIb0aDYkAMBvJ4+PzhdvXlxYyYAo0U/fdu3S0AzC8l1afiw0GDy79vthx87DaKd7gZXysRIV/3z1lVM4saOrB0uX3FXgzCzidsceIsA63cehAI+EUQyii63o2NsyQWgSUuvaGnCUtU2oOR1B7QZkzjr14hXDLgNItEs46CjSEf5ngd3CiGuVfBGKWuv77y5ZyCML+zT+2D1/qoRJE91GalxUhL6YWLvxc0lLmQYNeg/nh6ksTnFj8qRkFfeDMM1otVAGWJYWA4OKuckXjJHFDqDRulIun8BmBOekbo5aLK/yCH5QqZZt1K2sKVvbt0AGdlCTG6UDUyoeMFzwJo3J21ch3znwt16DhceBDYyb");
            //SciChartSurface.SetRuntimeLicenseKey("alZ8d1UhrI0H9SwzPAi19H+Jk7SBJw+7iITDtj8xgeuAPB5MboJg78xfEwnZj4UFhox2pFdcXJ8cxlaQBJKVrU8vZLL+HEeJE0ozWJt+T7nHUfEFTE+iLshRO1pizX6uG14y+ve3PNXtnvty+Aur4tuP7cvJo57amxRvRNKLOg+aoB8GSNx45Jbe9Erfm5OPL2cxboEqwW/ju7MoohzSv7KsfbLt2UilfgbszsTcCJt9WqQgScRp3rJ3+IsO+tpiG9iAYVSNnzcU8sCAfJWtiNVmkrAACu1eW9VS7h+qf/cCeHXRe/XDR9TBDKA6EKlqWrjPnC9HJO+ZA7PhHunweXHCGyG/YEA4EzZ2nLhczGlP7oJkYtnqkNJ4YGFcsmQ3t0cS3GHo+o7CZBQaIpOBaX49jXMOZa11iBiWOUYHRerIxxFEemkPi1CtrPoxD+6ppcbjkfeUceJG89R8xcnFp+LdJ4xDKQCZUNE9MWSoMvYknhxD");
            //SciChartSurface.SetRuntimeLicenseKey("D+rzQAwAqkMm5k9aeMUtypgMKfGGheCEFKWMpHn/lAesbSFiS8PyN05A7bUrVzE9AOFN0vulXuSiybLQsoZQA8ez2wfNzf5fcD0Ij0cGq58F65ARQ5bjw/6E7UIhqeaqpAbaaQ2lGIaoorrwzfiUllTM0CRNTtdWcMNCcy+N1uCNlpwhZj1IICLtOLo7PyT8GQkVIysEcXZ9/eFHK8MCT3GwBOAqOTpcbaehogozwy0/95Lm3aQs7IvKYTmi5ZkpT3Y+Ewep3gP5YjTz6L/rGW01898EhjwKv7Yof1vKmJcL4p0fgq1kUhpaZKR6epj7dFdBF7ZRSsxr/A9jOrmKunO1CIuq04vJO1rJu79w2InsBx3l2i6EYjF/0JJgcrtSU9LfgVilkS6X756fk8yyEwztBDJAIieQUt0B/rzfJp9FYKmnlXDwvbKM72inx04C7MLjOmPkudVItC5BZRcaYEJzXgIWHdqUdgGQEG3XNAFyin/o");
            //SciChartSurface.SetRuntimeLicenseKey("D+rzQAwAqkMm5k9aeMUtypgMKfGGheCEFKWMpHn/lAesbSFiS8PyN05A7bUrVzE9AOFN0vulXuSiybLQsoZQA8ez2wfNzf5fcD0Ij0cGq58F65ARQ5bjw/6E7UIhqeaqpAbaaQ2lGIaoorrwzfiUllTM0CRNTtdWcMNCcy+N1uCNlpwhZj1IICLtOLo7PyT8GQkVIysEcXZ9/eFHK8MCT3GwBOAqOTpcbaehogozwy0/95Lm3aQs7IvKYTmi5ZkpT3Y+Ewep3gP5YjTz6L/rGW01898EhjwKv7Yof1vKmJcL4p0fgq1kUhpaZKR6epj7dFdBF7ZRSsxr/A9jOrmKunO1CIuq04vJO1rJu79w2InsBx3l2i6EYjF/0JJgcrtSU9LfgVilkS6X756fk8yyEwztBDJAIieQUt0B/rzfJp9FYKmnlXDwvbKM72inx04C7MLjOmPkudVItC5BZRcaYEJzXgIWHdqUdgGQEG3XNAFyin/o");
            // Set this code once in App.xaml.cs or application startup
            //SciChartSurface.SetRuntimeLicenseKey("9mTewA7oj8XjYOJy4BlbtSQws7fIQ7FrHHW9peKyWsx1XcErhBcC0LKfX/ykdteUUnCFlwBxfyPF7uj8FdzdvrqCKzMnhD5aXpN7E3DWMbphVBnxeAMxCGMo7I/AEd362TCNX6GQw/Xk2RYjqPNV8R5MYK6CWp1yADmu+XF2chGBTLVfLRxowttiocgj2Mw25kAF5xetNsjpPskB6d30SodMmZ+BI3YO2dc3Cr5vcJ7stvH2AYJvP9WF7hOf2IVRXXvZyi/Txa7CWV5wfjtgbOSTzuvNVWC2/fcbJOWyqn+/r2HcGgWNIp4mHzD5EfwlliRG6TUl96TKEPWIVHLm3zfmiL5w9EIhm3Bk74REgNOYoJN5b3vwOVxcrlj7+fiRXjqVsQ6/8FYKeJFuvTwBqbVd+NEmsyOtitVh8KliVmdiIDzMhElEjnl6UJaQPo11nzVC8qyuxSMLYs51R/QDiy6IQKEhkd+TKExUC2AY5UC8ownO0WmQ");
            //SciChartSurface.SetRuntimeLicenseKey("9MjJT/tE1wWKl0wgwkMsWNasyLEGagtVpe5gfjlsOkOj2rEqBgJHzS/zCy2EK/Pud+XoEXcgI90YxRZrSJn6lFsP1OhZd6UtHe3N5vrjYcnJzFbEJ9tyJ8ezFTGtzQtf9nxEger6Bgx6VK2DnZGbeT2Z5rRiU/qdqGb0QanIRJ5Hl/F8tVU0aych/39v1AAjDGzUmYqArBIfwlk/2K/H1f3Vo3v74pgBp8SvBaN/EF9v2gKCAR6ktbXUtrJWeFdXywbr6hIkj2fD+Zfs27VYEzcdJdVXML1N3uoIjNybMVHg8+FcM2D01tpwsgypa1zb0B1XrHNa+Ln/wOSXmS9S4IvNmyp1W7OLaprL//ueVihKw+t4cCvN9ksvO8bmTUfCl6JW9uHACHMqLE0WtxnIoUWl71yOkBrDDeIR6m4okp5BBN71gvtF95PzuAQz8aCoOYQg9JdX8c++Q4NFefsObGEItnAAhs89WtZJYn6dQKuGztA2TeJW");
            //SciChartSurface.SetRuntimeLicenseKey(@"<LicenseContract>
            //  <Customer>University of  Toulon</Customer>
            //  <OrderId>EDUCATIONAL-USE-0109</OrderId>
            //  <LicenseCount>1</LicenseCount>
            //  <IsTrialLicense>false</IsTrialLicense>
            //  <SupportExpires>11/04/2019 00:00:00</SupportExpires>
            //  <ProductCode>SC-WPF-SDK-PRO-SITE</ProductCode>
            //  <KeyCode>lwABAQEAAABZVzOfQ0zVAQEAewBDdXN0b21lcj1Vbml2ZXJzaXR5IG9mICBUb3Vsb247T3JkZXJJZD1FRFVDQVRJT05BTC1VU0UtMDEwOTtTdWJzY3JpcHRpb25WYWxpZFRvPTA0LU5vdi0yMDE5O1Byb2R1Y3RDb2RlPVNDLVdQRi1TREstUFJPLVNJVEWDf0QgB8GnCQXI6yAqNM2njjnGbUt2KsujTDzeE+k69K1XYVF1s1x1Hb/i/E3GHaU=</KeyCode>
            //</LicenseContract>");

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
        static Thread t3;
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

            //    t3 = new Thread(() =>
            //    {
            //        //Attention, il est nécessaire d'ajouter PresentationFramework, PresentationCore, WindowBase and your wpf window application aux ressources.
            //        wpfUnsupervisedLearning = new WpfUnsupervisedLearning();
            //        wpfUnsupervisedLearning.Loaded += RegisterUnsupervisedLearningInterfaceEvents;
            //        //wpfUnsupervisedLearning.ShowDialog();
            //    });
            //    t3.SetApartmentState(ApartmentState.STA);
            //t3.Start();
            


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
            respirateurInterfaceMonitor.OnResetLearningEvent += msgGenerator.GenerateMessageResetLearning;


            //Callback (confirmation reglage parametres)
            msgProcessor.OnStartStopCallBackFromRespiratorGeneratedEvent += respirateurInterfaceMonitor.ActualizeStartStopButton;
            msgProcessor.OnSetModeCallBackFromRespiratorGeneratedEvent += respirateurInterfaceMonitor.ActualizeMode;
            msgProcessor.OnPressureDataFromRespiratorGeneratedEvent += respirateurInterfaceMonitor.UpdateVolumeDataOnGraph;
            msgProcessor.OnSetPLimiteCallBackFromRespiratorGeneratedEvent += respirateurInterfaceMonitor.ActualizePLimite;
            msgProcessor.OnSetVLimiteCallBackFromRespiratorGeneratedEvent += respirateurInterfaceMonitor.ActualizeVLimite;
            msgProcessor.OnSetPSeuilCallBackFromRespiratorGeneratedEvent += respirateurInterfaceMonitor.ActualizeSeuilDetection;
            msgProcessor.OnSetUnsupervisedLearningParametersEvent += respirateurInterfaceMonitor.ActualizeUnsupervisedLearningParamaters;
            msgProcessor.OnSetUnsupervisedLearningClusterInfoEvent += respirateurInterfaceMonitor.ActualizeUnsupervisedLearningClusterInfo;
            msgProcessor.OnSetUnsupervisedLearningSampleInfoEvent += respirateurInterfaceMonitor.ActualizeUnsupervisedLearningSamplesInfo;
            msgProcessor.OnSetAlarmSystemEvent += respirateurInterfaceMonitor.AlarmSystem;
        }
    }
}
