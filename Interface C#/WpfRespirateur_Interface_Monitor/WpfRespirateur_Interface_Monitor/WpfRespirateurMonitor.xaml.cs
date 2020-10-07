using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfOscilloscopeControl;
using EventArgsLibrary;
using System.Windows.Threading;
using System.Globalization;
using System.Threading;
using System.Windows.Markup;

namespace WpfRespirateur_Interface_Monitor
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class WpfRespirateurMonitor : Window
    {
        DispatcherTimer timerAffichage;
        TimeSpan interval = new TimeSpan(0, 0, 0, 0, 50);
        TimeSpan dureeSession = new TimeSpan();
        DateTime dateDebutSession = new DateTime();
        bool simulate = false;                           //Permet de simuler la carte pour verif affichage

        int cycles = 12;
        int limiteCyclesBas = 6;
        int limiteCyclesHaut = 20;

        public WpfRespirateurMonitor()
        {
            InitializeComponent();
            //RadioButtonVolume.IsChecked = isPilotageVolumeChecked;

            oscilloVolume.SetTitle("Courbe Volume");
            oscilloVolume.AddOrUpdateLine(0, 500, "Volume");
            oscilloVolume.ChangeLineColor(0, Colors.Red);
            oscilloVolume.SetAutoScaleY(false);
            oscilloVolume.SetYAxisScale(0, 2);//1.2);

            oscilloPression.SetTitle("Courbe pression");
            oscilloPression.AddOrUpdateLine(0, 500, "Pression");
            oscilloPression.ChangeLineColor(0, Colors.Blue);
            oscilloPression.SetAutoScaleY(false);
            oscilloPression.SetYAxisScale(-5, 45);
            

            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0,0,0,0,100);
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();

            labelSeuilDetection.Visibility = Visibility.Hidden;
            labelSeuilDetectionVal.Visibility = Visibility.Hidden;
            sliderSeuil.Visibility = Visibility.Hidden;

            //Among other settings, this code may be used
            CultureInfo ci = CultureInfo.CurrentUICulture;

            try
            {
                //Override the default culture with something from app settings
                ci = new CultureInfo("Fr");
            }
            catch { }
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            //Here is the important part for databinding default converters
            //FrameworkElement.LanguageProperty.OverrideMetadata(
            //        typeof(FrameworkElement),
            //        new FrameworkPropertyMetadata(
            //            XmlLanguage.GetLanguage(ci.IetfLanguageTag)));

            //Among other code
            if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator != ".")
            {
                //Handler attach - will not be done if not needed
                PreviewKeyDown += new KeyEventHandler(MainWindow_PreviewKeyDown);
            }
        }
        void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Decimal)
            {
                e.Handled = true;

                if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Length > 0)
                {
                    Keyboard.FocusedElement.RaiseEvent(
                        new TextCompositionEventArgs(
                            InputManager.Current.PrimaryKeyboardDevice,
                            new TextComposition(InputManager.Current,
                                Keyboard.FocusedElement,
                                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                            )
                        { RoutedEvent = TextCompositionManager.TextInputEvent });
                }
            }
        }

        int k = 0;
        public void TimerAffichage_Tick(object sender, EventArgs e)
        {
            dureeSession = DateTime.Now.Subtract(dateDebutSession);
            if(isStarted)
                labelSessionDuree.Content = "Session Time:" + dureeSession.ToString(@"dd\.hh\:mm\:ss");

            if(hasZoomed)
            {
                k++;
                if (k == 5)
                {
                    oscilloVolume.SetAutoScaleY(false);
                    oscilloVolume.SetYAxisScale(0, 1.2);
                    oscilloPression.SetAutoScaleY(false);
                    oscilloPression.SetYAxisScale(0, 50);
                    hasZoomed = false;
                    k = 0;
                }
            }
        }

        double zoomFactor = 5;
        bool isZoomed = false;
        int lastZoomedRow = 0;
        int lastZoomedCol = 0;
        bool hasZoomed = false;
        private void ZoomOnGraph_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int row = 0, column = 0;
            if (sender.GetType() == typeof(WpfOscilloscope))
            {
                WpfOscilloscope s = (WpfOscilloscope)sender;
                if (s != null)
                {
                    row = Grid.GetRow(s);
                    column = Grid.GetColumn(s);
                }
            }
            else if (sender.GetType() == typeof(GroupBox))
            {
                GroupBox s = (GroupBox)sender;
                if (s != null)
                {
                    row = Grid.GetRow(s);
                    column = Grid.GetColumn(s);
                }
            }

            if (!isZoomed)
            {
                GridApplication.ColumnDefinitions[column].Width = new GridLength(GridApplication.ColumnDefinitions[column].Width.Value * zoomFactor, GridUnitType.Star);
                GridApplication.RowDefinitions[row].Height = new GridLength(GridApplication.RowDefinitions[row].Height.Value * zoomFactor, GridUnitType.Star);
                lastZoomedCol = column;
                lastZoomedRow = row;
                isZoomed = true;
            }
            else
            {
                GridApplication.ColumnDefinitions[lastZoomedCol].Width = new GridLength(GridApplication.ColumnDefinitions[lastZoomedCol].Width.Value / zoomFactor, GridUnitType.Star);
                GridApplication.RowDefinitions[lastZoomedRow].Height = new GridLength(GridApplication.RowDefinitions[lastZoomedRow].Height.Value / zoomFactor, GridUnitType.Star);
                isZoomed = false;
                if (lastZoomedRow != row || lastZoomedCol != column)
                {
                    GridApplication.ColumnDefinitions[column].Width = new GridLength(GridApplication.ColumnDefinitions[column].Width.Value * zoomFactor, GridUnitType.Star);
                    GridApplication.RowDefinitions[row].Height = new GridLength(GridApplication.RowDefinitions[row].Height.Value * zoomFactor, GridUnitType.Star);
                    lastZoomedCol = column;
                    lastZoomedRow = row;
                    isZoomed = true;
                    
                }
            }
            hasZoomed = true;
        }


       

        bool isStarted = false;
        private void ButtonStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (isStarted)
            {
                OnStartStopFromInterface(false);
               
                labelSessionStop.Content = "Session Stop:" + DateTime.Now.ToString();
               
            }
            else
            {
                OnStartStopFromInterface(true);
               
                    dateDebutSession = DateTime.Now;
                    labelSessionStart.Content = "Session Start:" + dateDebutSession.ToString();
                    dureeSession = new TimeSpan(0,0,0,0);
                
            }
        }


        #region InputEvents
        double volume = 0;
        bool usePitot4mm = false;
        public void UpdateVolumeDataOnGraph(object sender, RespirateurDataEventArgs e)
        {
            oscilloPression.AddPointToLine(0, e.EmbeddedTimeStampInMs / 1000.0, e.pressureSensor2/100);     //Pression patient (mmH2o)
            //double pression2 = (e.pressureSensor2 - 1.65+ 0.0075)/3.0 * (100000 / 0.085) ;
            double rho = 1.23;
            //double diametre = 0.014;        //en M
            double diffPression = e.pressureSensor1-0.08;
            int sign=1;
            if (diffPression < 0)
                sign = -1;
            else
                sign = 1;
            double vitesse = Math.Sqrt(2 * Math.Abs(diffPression) / rho)* sign;
            //double surface = (diametre * diametre) / 4 * Math.PI;
            double surface = 1;
            if (!usePitot4mm)
            {
                surface = 9.4 / 100000;                 //En M² pour tube en 4mm externe
            }
            else
            {
                surface = 7.26 / 100000;                //En M² pour tube en 6mm externe
            }
            //double surface = 7.26 / 100000;                 //En M² pour tube en 6mm externe
            double debit = vitesse * surface;           //En M3/s
            
            if (vitesse > 0.01)
                volume += (debit*1000) / 50;
            else
                volume = 0;
            oscilloVolume.AddPointToLine(0, e.EmbeddedTimeStampInMs / 1000.0, e.pressureSensor1);      //Tube pito
        }


        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        public void ActualizeStartStopButton(object sender, BoolEventArgs e)
        {
            //La solution consiste a passer par un delegué qui executera l'action a effectuer depuis le thread concerné.
            //Ici, l'action a effectuer est la modification d'un bouton. Ce bouton est un objet UI, et donc l'action doit etre executée depuis un thread UI.
            //Sachant que chaque objet UI (d'interface graphique) dispose d'un dispatcher qui permet d'executer un delegué (une methode) depuis son propre thread.
            //La difference entre un Invoke et un beginInvoke est le fait que le Invoke attend la fin de l'execution de l'action avant de sortir.
            ButtonStartStop.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                if (!e.value)
                {
                    ButtonStartStop.Content = "START";
                    isStarted = false;
                }
                else
                {
                    ButtonStartStop.Content = "STOP";
                    isStarted = true;
                }
            }));
            labelSessionStart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                if(e.value)
                    labelSessionStart.Content = "Session Start:" + DateTime.Now.ToString();
            }));
        }

        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        public void ActualizeCyclesPerMinute(object sender, ByteEventArgs e)
        {
            //La solution consiste a passer par un delegué qui executera l'action a effectuer depuis le thread concerné.
            //Ici, l'action a effectuer est la modification d'un bouton. Ce bouton est un objet UI, et donc l'action doit etre executée depuis un thread UI.
            //Sachant que chaque objet UI (d'interface graphique) dispose d'un dispatcher qui permet d'executer un delegué (une methode) depuis son propre thread.
            //La difference entre un Invoke et un beginInvoke est le fait que le Invoke attend la fin de l'execution de l'action avant de sortir.
            labelCycles.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                labelCycles.Content = e.Value.ToString();
                cycles = e.Value;
            }));
        }

        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        public void ActualizeMode(object sender, BoolEventArgs e)
        {
            //La solution consiste a passer par un delegué qui executera l'action a effectuer depuis le thread concerné.
            //Ici, l'action a effectuer est la modification d'un bouton. Ce bouton est un objet UI, et donc l'action doit etre executée depuis un thread UI.
            //Sachant que chaque objet UI (d'interface graphique) dispose d'un dispatcher qui permet d'executer un delegué (une methode) depuis son propre thread.
            //La difference entre un Invoke et un beginInvoke est le fait que le Invoke attend la fin de l'execution de l'action avant de sortir.
            if(e.value)
            {
                //On est bien en mode Assistance
                if (MenuItemReanimation != null)
                {
                    MenuItemReanimation.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        MenuItemReanimation.IsChecked = false;
                    }));

                }

                MenuItemAssistance.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    MenuItemAssistance.IsChecked = true;
                }));

                labelSeuilDetection.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    labelSeuilDetection.Visibility = Visibility.Visible;
                }));
                labelSeuilDetectionVal.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    labelSeuilDetectionVal.Visibility = Visibility.Visible;
                }));
                sliderSeuil.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    sliderSeuil.Visibility = Visibility.Visible;
                }));
                //if (groupBoxCycles != null)
                //{
                //    groupBoxCycles.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                //    {
                //        groupBoxCycles.Visibility = Visibility.Hidden;
                //    }));
                //}
            }
            else
            {
                //On est pas en mode Assistance
                if (MenuItemAssistance != null)
                {
                    MenuItemAssistance.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        MenuItemAssistance.IsChecked = false;
                    }));
                }

                MenuItemReanimation.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    MenuItemReanimation.IsChecked = true;
                }));

                labelSeuilDetection.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    labelSeuilDetection.Visibility = Visibility.Hidden;
                }));
                labelSeuilDetectionVal.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    labelSeuilDetectionVal.Visibility = Visibility.Hidden;
                }));
                sliderSeuil.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    sliderSeuil.Visibility = Visibility.Hidden;
                }));
                //if (groupBoxCycles != null)
                //{
                //    groupBoxCycles.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                //    {
                //        groupBoxCycles.Visibility = Visibility.Visible;
                //    }));
                //}
            }
        }

        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        public void ActualizeSeuilDetection(object sender, DoubleArgs e)
        {
            //La solution consiste a passer par un delegué qui executera l'action a effectuer depuis le thread concerné.
            //Ici, l'action a effectuer est la modification d'un bouton. Ce bouton est un objet UI, et donc l'action doit etre executée depuis un thread UI.
            //Sachant que chaque objet UI (d'interface graphique) dispose d'un dispatcher qui permet d'executer un delegué (une methode) depuis son propre thread.
            //La difference entre un Invoke et un beginInvoke est le fait que le Invoke attend la fin de l'execution de l'action avant de sortir.
            labelSeuilDetectionVal.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                labelSeuilDetectionVal.Content = (e.Value/100).ToString("F2");
            }));
        }

        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        public void ActualizeVLimite(object sender, DoubleArgs e)
        {
            //La solution consiste a passer par un delegué qui executera l'action a effectuer depuis le thread concerné.
            //Ici, l'action a effectuer est la modification d'un bouton. Ce bouton est un objet UI, et donc l'action doit etre executée depuis un thread UI.
            //Sachant que chaque objet UI (d'interface graphique) dispose d'un dispatcher qui permet d'executer un delegué (une methode) depuis son propre thread.
            //La difference entre un Invoke et un beginInvoke est le fait que le Invoke attend la fin de l'execution de l'action avant de sortir.
            labelVolumeCurrentVal.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                labelVolumeCurrentVal.Content = "Current value: "+(e.Value).ToString("F2") +" L";
            }));
        }
        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        public void ActualizePLimite(object sender, DoubleArgs e)
        {
            //La solution consiste a passer par un delegué qui executera l'action a effectuer depuis le thread concerné.
            //Ici, l'action a effectuer est la modification d'un bouton. Ce bouton est un objet UI, et donc l'action doit etre executée depuis un thread UI.
            //Sachant que chaque objet UI (d'interface graphique) dispose d'un dispatcher qui permet d'executer un delegué (une methode) depuis son propre thread.
            //La difference entre un Invoke et un beginInvoke est le fait que le Invoke attend la fin de l'execution de l'action avant de sortir.
            labelPressionCurrentVal.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                labelPressionCurrentVal.Content = "Current value: " + (e.Value/100).ToString();
            }));
        }
        #endregion

        #region OutputEvents
        //OUTPUT EVENT

        public event EventHandler<BoolEventArgs> OnStartStopFromInterfaceGeneratedEvent;
        public virtual void OnStartStopFromInterface(bool val)
        {
            var handler = OnStartStopFromInterfaceGeneratedEvent;
            if (handler != null)
            {
                handler(this, new BoolEventArgs { value = val });
            }
        }

        public event EventHandler<Int32EventArgs> OnDoStepFromInterfaceGeneratedEvent;
        public virtual void OnDoStepFromInterface(int nbSteps)
        {
            var handler = OnDoStepFromInterfaceGeneratedEvent;
            if (handler != null)
            {
                handler(this, new Int32EventArgs { value = nbSteps });
            }
        }

        public event EventHandler<Int32EventArgs> OnSetAmplitudeFromInterfaceGeneratedEvent;
        public virtual void OnSetAmplitudeFromInterface(int amplitude)
        {
            var handler = OnSetAmplitudeFromInterfaceGeneratedEvent;
            if (handler != null)
            {
                handler(this, new Int32EventArgs { value = amplitude });
            }
        }

        public event EventHandler<Int32EventArgs> OnSetOffsetUpFromInterfaceGeneratedEvent;
        public virtual void OnSetOffsetUpFromInterface(int offsetStep)
        {
            var handler = OnSetOffsetUpFromInterfaceGeneratedEvent;
            if (handler != null)
            {
                handler(this, new Int32EventArgs { value = offsetStep });
            }
        }

        public event EventHandler<Int32EventArgs> OnSetOffsetDownFromInterfaceGeneratedEvent;
        public virtual void OnSetOffsetDownFromInterface(int offsetStep)
        {
            var handler = OnSetOffsetDownFromInterfaceGeneratedEvent;
            if (handler != null)
            {
                handler(this, new Int32EventArgs { value = offsetStep });
            }
        }

        public event EventHandler<DoubleArgs> OnSetPauseTimeUpFromInterfaceGeneratedEvent;
        public virtual void OnSetPauseTimeUpFromInterface(double pauseTime)
        {
            var handler = OnSetPauseTimeUpFromInterfaceGeneratedEvent;
            if (handler != null)
            {
                handler(this, new DoubleArgs { Value = pauseTime });
            }
        }

        public event EventHandler<DoubleArgs> OnSetPauseTimeDownFromInterfaceGeneratedEvent;
        public virtual void OnSetPauseTimeDownFromInterface(double pauseTime)
        {
            var handler = OnSetPauseTimeDownFromInterfaceGeneratedEvent;
            if (handler != null)
            {
                handler(this, new DoubleArgs { Value = pauseTime });
            }
        }

        public event EventHandler<DoubleArgs> OnSetPlimitFromInterfaceGeneratedEvent;
        public virtual void OnSetPlimitFromInterface(double plimit)
        {
            var handler = OnSetPlimitFromInterfaceGeneratedEvent;
            if (handler != null)
            {
                handler(this, new DoubleArgs { Value = plimit });
            }
        }

        public event EventHandler<DoubleArgs> OnSetVlimitFromInterfaceGeneratedEvent;
        public virtual void OnSetVlimitFromInterface(double vlimit)
        {
            var handler = OnSetVlimitFromInterfaceGeneratedEvent;
            if (handler != null)
            {
                handler(this, new DoubleArgs { Value = vlimit });
            }
        }

        public event EventHandler<DoubleArgs> OnSetSpeedFromInterfaceGeneratedEvent;
        public virtual void OnSetSpeedFromInterface(double speed)
        {
            var handler = OnSetSpeedFromInterfaceGeneratedEvent;
            if (handler != null)
            {
                handler(this, new DoubleArgs { Value = speed });
            }
        }

        public event EventHandler<EventArgs> OnStartAdvancedInterfaceFromInterfaceGeneratedEvent;
        public virtual void OnStartAdvancedInterfaceFromInterface()
        {
            var handler = OnStartAdvancedInterfaceFromInterfaceGeneratedEvent;
            if (handler != null)
            {
                handler(this, new EventArgs() );
            }
        }

        public event EventHandler<Int32EventArgs> OnSetCyclesPerMinromInterfaceGeneratedEvent;
        public virtual void OnSetCyclesPerMinFromInterface(Int32 val)
        {
            var handler = OnSetCyclesPerMinromInterfaceGeneratedEvent;
            if (handler != null)
            {
                handler(this, new Int32EventArgs { value = val });
            }
        }

        public event EventHandler<BoolEventArgs> OnSetModeFromInterfaceGeneratedEvent;
        public virtual void OnSetModeFromInterface(bool isAssistance)
        {
            var handler = OnSetModeFromInterfaceGeneratedEvent;
            if (handler != null)
            {
                handler(this, new BoolEventArgs { value = isAssistance });
            }
        }

        public event EventHandler<EventArgs> OnInitMachineFromInterfaceGeneratedEvent;
        public virtual void OnInitMachineFromInterface()
        {
            var handler = OnInitMachineFromInterfaceGeneratedEvent;
            if (handler != null)
            {
                handler(this, new EventArgs {});
            }
        }

        public event EventHandler<DoubleArgs> OnChangeSeuilFromInterfaceGeneratedEvent;
        public virtual void OnChangeSeuilFromInterface(double seuil)
        {
            var handler = OnChangeSeuilFromInterfaceGeneratedEvent;
            if (handler != null)
            {
                handler(this, new DoubleArgs { Value = seuil });
            }
        }
        #endregion

        bool isPilotageVolumeChecked = true;
        private void RadioButtonVolume_Click(object sender, RoutedEventArgs e)
        {
            //if(!isPilotageVolumeChecked)
            //{
            //    RadioButtonPression.IsChecked = false;
            //    isPilotageVolumeChecked = true;
            //}
        }

        private void RadioButtonPression_Click(object sender, RoutedEventArgs e)
        {
            //if (isPilotageVolumeChecked)
            //{
            //    RadioButtonVolume.IsChecked = false;
            //    isPilotageVolumeChecked = false;
            //}
        }

        private void ButtonCycleP_Click(object sender, RoutedEventArgs e)
        {
            if (cycles < limiteCyclesHaut)
            {
                cycles++;
                OnSetCyclesPerMinFromInterface(cycles);
                labelCycles.Content = cycles.ToString();
            }
        }

        private void ButtonCycleM_Click(object sender, RoutedEventArgs e)
        {
            if (cycles > limiteCyclesBas)
            {
                cycles--;
                OnSetCyclesPerMinFromInterface(cycles);
                labelCycles.Content = cycles.ToString();
            }
        }

        private void ButtonPressionSet(object sender, RoutedEventArgs e)
        {
            double pressionMmH2o = Convert.ToDouble(textBoxPression.Text);
            OnSetPlimitFromInterface(pressionMmH2o);
        }

        private void ButtonVolumeSet(object sender, RoutedEventArgs e)
        {
            double volume = Convert.ToDouble(textBoxVolume.Text);
            OnSetVlimitFromInterface(volume);
        }

        private void MenuItemUsePitot4_Checked(object sender, RoutedEventArgs e)
        {
            MenuItemUsePitot6.IsChecked = false;
            usePitot4mm = true;
        }

        private void MenuItemUsePitot6_Checked(object sender, RoutedEventArgs e)
        {
            MenuItemUsePitot4.IsChecked = false;
            usePitot4mm = false;
        }

        private void MenuItemAdvanced_Click(object sender, RoutedEventArgs e)
        {
                OnStartAdvancedInterfaceFromInterface();
        }


        private void MenuItemInit_Click(object sender, RoutedEventArgs e)
        {
            OnInitMachineFromInterface();
        }

        private void MenuItemReanimation_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemAssistance != null)
                MenuItemAssistance.IsChecked = false;
            if (MenuItemReanimation != null)
                MenuItemReanimation.IsChecked = false;
            OnSetModeFromInterface(false);
        }

        private void MenuItemAssistance_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemAssistance != null)
                MenuItemAssistance.IsChecked = false;
            if (MenuItemReanimation != null)
                MenuItemReanimation.IsChecked = false;
            OnSetModeFromInterface(true);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            OnChangeSeuilFromInterface(e.NewValue);
        }
    }
}
