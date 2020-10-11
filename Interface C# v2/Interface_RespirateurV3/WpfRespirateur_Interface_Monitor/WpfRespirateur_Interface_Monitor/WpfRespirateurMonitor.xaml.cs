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
using System.Media;
using System.Collections.ObjectModel;
using UnsupervisedLearning;
using SciChart.Charting3D.Model;
using SciChart.Data.Model;
using SciChart.Charting.Visuals;
using SciChart.Charting3D.Axis;

namespace WpfRespirateur_Interface_Monitor
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class WpfRespirateurMonitor : Window
    {
        DispatcherTimer timerAnimationCluster;
        DispatcherTimer timerAffichage;
        DispatcherTimer timerCamera;
        DispatcherTimer timerAlarm;
        DispatcherTimer timerDoubleAlarm;

        TimeSpan interval = new TimeSpan(0, 0, 0, 0, 50);
        TimeSpan dureeSession = new TimeSpan();
        DateTime dateDebutSession = new DateTime();
        bool simulate = false;                           //Permet de simuler la carte pour verif affichage

        bool clearGraphs = false;

        double AxisOffset = 100.0f;

        int cycles = 12;
        int limiteCyclesBas = 6;
        int limiteCyclesHaut = 20;

        public WpfRespirateurMonitor()
        {
            InitializeComponent();
            //RadioButtonVolume.IsChecked = isPilotageVolumeChecked;
            // Set this code once in AppDelegate or application startup
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in App.xaml.cs or application startup
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

            timerAnimationCluster = new DispatcherTimer();
            timerAnimationCluster.Interval = TimeSpan.FromMilliseconds(30);
            timerAnimationCluster.Tick += timerAnimationClusterFunction;

            timerDoubleAlarm = new DispatcherTimer();
            timerDoubleAlarm.Interval = TimeSpan.FromMilliseconds(5500);
            timerDoubleAlarm.Tick += timerDoubleAlarmFunction;

            timerCamera = new DispatcherTimer();
            timerCamera.Interval = TimeSpan.FromMilliseconds(20);
            timerCamera.Tick += timerCameraFunction;
            timerCamera.Start();

            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();

            timerAlarm = new DispatcherTimer();
            timerAlarm.Interval = TimeSpan.FromMilliseconds(500);
            timerAlarm.Tick += timerAlarmFunction;


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
            if (isStarted)
                labelSessionDuree.Content = "Session Time:" + dureeSession.ToString(@"dd\.hh\:mm\:ss");

            if (hasZoomed)
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
            if (!alarmActivated)
            {
                switch (SuccessiveAnomalies)
                {
                    case 1:
                        GridApplication.Background = Brushes.DarkOliveGreen;
                        ClusterFeaturesGroupBox.Background = Brushes.DarkOliveGreen;
                        //oscilloPression.Background = Brushes.DarkOliveGreen;
                        //oscilloVolume.Background = Brushes.DarkOliveGreen;
                        //sciChart.Background = Brushes.DarkOliveGreen;
                        break;
                    case 2:
                        GridApplication.Background = Brushes.DarkGoldenrod;
                        ClusterFeaturesGroupBox.Background = Brushes.DarkGoldenrod;
                        //oscilloPression.Background = Brushes.DarkOrange;
                        //oscilloVolume.Background = Brushes.DarkOrange;
                        //sciChart.Background = Brushes.DarkOrange;
                        break;
                    case 3:
                        GridApplication.Background = Brushes.DarkOrange;
                        ClusterFeaturesGroupBox.Background = Brushes.DarkOrange;
                        //oscilloPression.Background = Brushes.DarkGoldenrod;
                        //oscilloVolume.Background = Brushes.DarkGoldenrod;
                        //sciChart.Background = Brushes.DarkGoldenrod;
                        break;
                    case 4:
                        GridApplication.Background = Brushes.DarkRed;
                        ClusterFeaturesGroupBox.Background = Brushes.DarkRed;
                        //oscilloPression.Background = Brushes.DarkOrange;
                        //oscilloVolume.Background = Brushes.DarkOrange;
                        //sciChart.Background = Brushes.DarkOrange;
                        break;
                    case 5:
                        GridApplication.Background = Brushes.DarkRed;
                        ClusterFeaturesGroupBox.Background = Brushes.DarkRed;
                        //oscilloPression.Background = Brushes.DarkRed;
                        //oscilloVolume.Background = Brushes.DarkRed;
                        //sciChart.Background = Brushes.DarkRed;
                        break;
                    case 6:
                        GridApplication.Background = Brushes.DarkRed;
                        ClusterFeaturesGroupBox.Background = Brushes.DarkRed;
                        //oscilloPression.Background = Brushes.DarkRed;
                        //oscilloVolume.Background = Brushes.DarkRed;
                        //sciChart.Background = Brushes.DarkRed;
                        break;
                    case 7:
                        GridApplication.Background = Brushes.DarkRed;
                        ClusterFeaturesGroupBox.Background = Brushes.DarkRed;
                        ////oscilloPression.Background = Brushes.DarkRed;
                        ////oscilloVolume.Background = Brushes.DarkRed;
                        ////sciChart.Background = Brushes.DarkRed;
                        break;
                    case 8:
                        GridApplication.Background = Brushes.DarkRed;
                        ClusterFeaturesGroupBox.Background = Brushes.DarkRed;
                        //oscilloPression.Background = Brushes.DarkRed;
                        //oscilloVolume.Background = Brushes.DarkRed;
                        //sciChart.Background = Brushes.DarkRed;
                        break;
                    default:
                        GridApplication.Background = new SolidColorBrush(Color.FromRgb(0x22, 0x22, 0x22));
                        ClusterFeaturesGroupBox.Background = new SolidColorBrush(Color.FromRgb(0x22, 0x22, 0x22));
                        oscilloPression.Background = Brushes.Black;
                        oscilloVolume.Background = Brushes.Black;
                        sciChart.Background = Brushes.Black;
                        break;
                }
            }



            if (clearGraphs)
            {
                labelLearningPhaseInitDispersion.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    labelLearningPhaseInitDispersion.Content = " ";
                    labelLearningPhaseInitDispersion.Background = (Brush)converter.ConvertFromString("#222222");
                }));
                labelLearningPhaseTrainingClusters.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    labelLearningPhaseTrainingClusters.Content = " ";
                    labelLearningPhaseTrainingClusters.Background = (Brush)converter.ConvertFromString("#222222");
                }));
                labelLearningPhaseMonitoringFailures.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    labelLearningPhaseMonitoringFailures.Content = " ";
                    labelLearningPhaseMonitoringFailures.Background = (Brush)converter.ConvertFromString("#222222");
                }));
                this.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    this.clustersData.Clear();
                    this.xyzDataSeries3DCluster.Clear();
                    this.xyzDataSeries3DDispersion.Clear();
                    this.xyzDataSeries3DDistance.Clear();
                    this.xyzDataSeries3DSample.Clear();
                    this.xyzDataSeries3DHighlight.Clear();
                    this.clustersData.Clear();
                });
                sciChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    ScatterSeries3DCluster.DataSeries = xyzDataSeries3DCluster;
                    ScatterSeries3DClusterDispersion.DataSeries = xyzDataSeries3DDispersion;
                    ScatterSeries3DClusterDistance.DataSeries = xyzDataSeries3DDistance;
                    ScatterSeries3DSample.DataSeries = xyzDataSeries3DSample;
                }));
                labelLearningStatus.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {                 
                    labelLearningStatus.Content = "Status: Training";
                }));
                clearGraphs = false;
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
                dureeSession = new TimeSpan(0, 0, 0, 0);

            }
        }


        #region InputEvents
        double volume = 0;
        bool usePitot4mm = false;
        public void UpdateVolumeDataOnGraph(object sender, RespirateurDataEventArgs e)
        {
            oscilloPression.AddPointToLine(0, e.EmbeddedTimeStampInMs / 1000.0, e.pressureSensor2 / 100);     //Pression patient (mmH2o)
            //double pression2 = (e.pressureSensor2 - 1.65+ 0.0075)/3.0 * (100000 / 0.085) ;
            double rho = 1.23;
            //double diametre = 0.014;        //en M
            double diffPression = e.pressureSensor1 - 0.08;
            int sign = 1;
            if (diffPression < 0)
                sign = -1;
            else
                sign = 1;
            double vitesse = Math.Sqrt(2 * Math.Abs(diffPression) / rho) * sign;
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
                volume += (debit * 1000) / 50;
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
                if (e.value)
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
            if (e.value)
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
                labelSeuilDetectionVal.Content = (e.Value / 100).ToString("F2");
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
                labelVolumeCurrentVal.Content = "Current value: " + (e.Value).ToString("F2") + " L";
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
                labelPressionCurrentVal.Content = "Current value: " + (e.Value / 100).ToString() + " cmH20";
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

        public event EventHandler<UnsupervisedLearningParametersArgs> OnResetLearningEvent;
        public virtual void OnResetLearning()
        {
            var handler = OnResetLearningEvent;
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
                handler(this, new EventArgs());
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
                handler(this, new EventArgs { });
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
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Clusters[i, j] = 0.0f;
                    Dispersions[i, j] = 0.0f;
                }
                SamplesOnCluster[i] = 0.0f;
            }
            if (cycles < limiteCyclesHaut)
            {
                cycles++;
                OnSetCyclesPerMinFromInterface(cycles);
                labelCycles.Content = cycles.ToString();
            }
            this.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                this.clustersData.Clear();
                this.xyzDataSeries3DCluster.Clear();
                this.xyzDataSeries3DDispersion.Clear();
                this.xyzDataSeries3DDistance.Clear();
                this.xyzDataSeries3DSample.Clear();
                this.xyzDataSeries3DHighlight.Clear();
                this.clustersData.Clear();
            });
            sciChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                ScatterSeries3DCluster.DataSeries = xyzDataSeries3DCluster;
                ScatterSeries3DClusterDispersion.DataSeries = xyzDataSeries3DDispersion;
                ScatterSeries3DClusterDistance.DataSeries = xyzDataSeries3DDistance;
                ScatterSeries3DSample.DataSeries = xyzDataSeries3DSample;
            }));
            labelState.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelState.Background = (Brush)converter.ConvertFromString("#222222");
                labelState.Content = "Normal mode";
            }));

            labelLearningPhaseInitDispersion.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelLearningPhaseInitDispersion.Content = " ";
                labelLearningPhaseInitDispersion.Background = (Brush)converter.ConvertFromString("#222222");
            }));
            labelLearningPhaseTrainingClusters.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelLearningPhaseTrainingClusters.Content = " ";
                labelLearningPhaseTrainingClusters.Background = (Brush)converter.ConvertFromString("#222222");
            }));
            labelLearningPhaseMonitoringFailures.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelLearningPhaseMonitoringFailures.Content = " ";
                labelLearningPhaseMonitoringFailures.Background = (Brush)converter.ConvertFromString("#222222");
            }));
            clearGraphs = true;
            Training = true;
            numberOfClusters = 0;
        }

        private void ButtonCycleM_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Clusters[i, j] = 0.0f;
                    Dispersions[i, j] = 0.0f;
                }
                SamplesOnCluster[i] = 0.0f;
            }
            if (cycles > limiteCyclesBas)
            {
                cycles--;
                OnSetCyclesPerMinFromInterface(cycles);
                labelCycles.Content = cycles.ToString();
            }
            this.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                this.clustersData.Clear();
                this.xyzDataSeries3DCluster.Clear();
                this.xyzDataSeries3DDispersion.Clear();
                this.xyzDataSeries3DDistance.Clear();
                this.xyzDataSeries3DSample.Clear();
                this.xyzDataSeries3DHighlight.Clear();
                this.clustersData.Clear();
            });
            sciChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                ScatterSeries3DCluster.DataSeries = xyzDataSeries3DCluster;
                ScatterSeries3DClusterDispersion.DataSeries = xyzDataSeries3DDispersion;
                ScatterSeries3DClusterDistance.DataSeries = xyzDataSeries3DDistance;
                ScatterSeries3DSample.DataSeries = xyzDataSeries3DSample;
            }));
            labelState.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelState.Background = (Brush)converter.ConvertFromString("#222222");
                labelState.Content = "Normal mode";
            }));

            labelLearningPhaseInitDispersion.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelLearningPhaseInitDispersion.Content = " ";
                labelLearningPhaseInitDispersion.Background = (Brush)converter.ConvertFromString("#222222");
            }));
            labelLearningPhaseTrainingClusters.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelLearningPhaseTrainingClusters.Content = " ";
                labelLearningPhaseTrainingClusters.Background = (Brush)converter.ConvertFromString("#222222");
            }));
            labelLearningPhaseMonitoringFailures.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelLearningPhaseMonitoringFailures.Content = " ";
                labelLearningPhaseMonitoringFailures.Background = (Brush)converter.ConvertFromString("#222222");
            }));
            labelNumberOfClusters.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                labelNumberOfClusters.Content = "Number of Clusters: 0";
            }));
            clearGraphs = true;
            Training = true;
            numberOfClusters = 0;
        }

        private void ButtonPressionSet(object sender, RoutedEventArgs e)
        {
            double pressionMmH2o = Convert.ToDouble(textBoxPression.Text);
            OnSetPlimitFromInterface(pressionMmH2o);
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Clusters[i, j] = 0.0f;
                    Dispersions[i, j] = 0.0f;
                }
                SamplesOnCluster[i] = 0.0f;
            }
            this.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                this.clustersData.Clear();
                this.xyzDataSeries3DCluster.Clear();
                this.xyzDataSeries3DDispersion.Clear();
                this.xyzDataSeries3DDistance.Clear();
                this.xyzDataSeries3DSample.Clear();
                this.xyzDataSeries3DHighlight.Clear();
                this.clustersData.Clear();
            });
            sciChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                ScatterSeries3DCluster.DataSeries = xyzDataSeries3DCluster;
                ScatterSeries3DClusterDispersion.DataSeries = xyzDataSeries3DDispersion;
                ScatterSeries3DClusterDistance.DataSeries = xyzDataSeries3DDistance;
                ScatterSeries3DSample.DataSeries = xyzDataSeries3DSample;
            }));
            labelState.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelState.Background = (Brush)converter.ConvertFromString("#222222");
                labelState.Content = "Normal mode";
            }));

            labelLearningPhaseInitDispersion.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelLearningPhaseInitDispersion.Content = " ";
                labelLearningPhaseInitDispersion.Background = (Brush)converter.ConvertFromString("#222222");
            }));
            labelLearningPhaseTrainingClusters.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelLearningPhaseTrainingClusters.Content = " ";
                labelLearningPhaseTrainingClusters.Background = (Brush)converter.ConvertFromString("#222222");
            }));
            labelLearningPhaseMonitoringFailures.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelLearningPhaseMonitoringFailures.Content = " ";
                labelLearningPhaseMonitoringFailures.Background = (Brush)converter.ConvertFromString("#222222");
            }));
            clearGraphs = true;
            Training = true;
            numberOfClusters = 0;
        }

        private void ButtonVolumeSet(object sender, RoutedEventArgs e)
        {
            double volume = Convert.ToDouble(textBoxVolume.Text);
            OnSetVlimitFromInterface(volume);
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Clusters[i, j] = 0.0f;
                    Dispersions[i, j] = 0.0f;
                }
                SamplesOnCluster[i] = 0.0f;
            }
            this.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                this.clustersData.Clear();
                this.xyzDataSeries3DCluster.Clear();
                this.xyzDataSeries3DDispersion.Clear();
                this.xyzDataSeries3DDistance.Clear();
                this.xyzDataSeries3DSample.Clear();
                this.xyzDataSeries3DHighlight.Clear();
                this.clustersData.Clear();
            });
            sciChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                ScatterSeries3DCluster.DataSeries = xyzDataSeries3DCluster;
                ScatterSeries3DClusterDispersion.DataSeries = xyzDataSeries3DDispersion;
                ScatterSeries3DClusterDistance.DataSeries = xyzDataSeries3DDistance;
                ScatterSeries3DSample.DataSeries = xyzDataSeries3DSample;
            }));
            labelState.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelState.Background = (Brush)converter.ConvertFromString("#222222");
                labelState.Content = "Normal mode";
            }));

            labelLearningPhaseInitDispersion.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelLearningPhaseInitDispersion.Content = " ";
                labelLearningPhaseInitDispersion.Background = (Brush)converter.ConvertFromString("#222222");
            }));
            labelLearningPhaseTrainingClusters.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelLearningPhaseTrainingClusters.Content = " ";
                labelLearningPhaseTrainingClusters.Background = (Brush)converter.ConvertFromString("#222222");
            }));
            labelLearningPhaseMonitoringFailures.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                var converter = new System.Windows.Media.BrushConverter();
                labelLearningPhaseMonitoringFailures.Content = " ";
                labelLearningPhaseMonitoringFailures.Background = (Brush)converter.ConvertFromString("#222222");
            }));
            clearGraphs = true;
            Training = true;
            numberOfClusters = 0;
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

        int ActualAnomaliesNumber = 0;
        int SuccessiveAnomalies = 0;
        bool Training = true;
        int numberOfClusters = 1;
        int normalSamples = 3;
        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        public void ActualizeUnsupervisedLearningParamaters(object sender, UnsupervisedLearningParametersArgs e)
        {
            //La solution consiste a passer par un delegué qui executera l'action a effectuer depuis le thread concerné.
            //Ici, l'action a effectuer est la modification d'un bouton. Ce bouton est un objet UI, et donc l'action doit etre executée depuis un thread UI.
            //Sachant que chaque objet UI (d'interface graphique) dispose d'un dispatcher qui permet d'executer un delegué (une methode) depuis son propre thread.
            ////La difference entre un Invoke et un beginInvoke est le fait que le Invoke attend la fin de l'execution de l'action avant de sortir.
            //labelSpeed.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            //{
            //    labelSpeed.Content = Convert.ToString(e.Value);
            //}));

            numberOfClusters = e.numberOfClusters;

            labelNumberOfClusters.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                labelNumberOfClusters.Content = "Number of Clusters: " + e.numberOfClusters.ToString() + "Number of Anomalies: " + e.numberOfAnomalies.ToString();
            }));

            //labelAnomaliesNumberVal.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            //{
            //    labelAnomaliesNumberVal.Content = e.numberOfAnomalies.ToString();
            //}));
            if(e.numberOfAnomalies> ActualAnomaliesNumber)
            {
                SuccessiveAnomalies += 1;
                normalSamples=3;
            }
            else
            {
                if (SuccessiveAnomalies > 0)
                {
                    //Si on 3 samples normales on reveint en arriere 
                    if (normalSamples > 0)
                        normalSamples--;
                    if (normalSamples == 0)
                    {
                        SuccessiveAnomalies--;
                        normalSamples = 3;
                    }

                }
            }
            ActualAnomaliesNumber = e.numberOfAnomalies;


            labelLearningStatus.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                if (e.trainingState == 1)
                {
                    labelLearningStatus.Content = "Status: Training";

                    if (numberOfClusters == 0)
                    {
                        labelLearningPhaseInitDispersion.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                        {
                            labelLearningPhaseInitDispersion.Content = "Evaluating data dispersion";
                            labelLearningPhaseInitDispersion.Background = Brushes.DarkGoldenrod;
                        }));
                    }
                    else
                    {
                        labelLearningPhaseInitDispersion.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                        {
                            labelLearningPhaseInitDispersion.Content = "Data dispersion Evaluated";
                            labelLearningPhaseInitDispersion.Background = Brushes.DarkGreen;
                        }));
                        labelLearningPhaseTrainingClusters.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                        {
                            labelLearningPhaseTrainingClusters.Content = "Training Clusters";
                            labelLearningPhaseTrainingClusters.Background = Brushes.DarkGoldenrod;
                        }));
                    }
                }
                else
                {
                    Training = false;
                    labelLearningStatus.Content = "Status: Monitoring";

                    labelLearningPhaseTrainingClusters.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        labelLearningPhaseTrainingClusters.Content = "Clusters trained";
                        labelLearningPhaseTrainingClusters.Background = Brushes.DarkGreen;
                    }));
                    labelLearningPhaseMonitoringFailures.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        labelLearningPhaseMonitoringFailures.Content = "Monitoring failures";
                        labelLearningPhaseMonitoringFailures.Background = Brushes.DarkGreen;
                    }));
                }
            }));

            //if (Training)
            //{
            //    if (numberOfClusters == 0)
            //    {
            //        labelLearningPhaseInitDispersion.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            //        {
            //            labelLearningPhaseInitDispersion.Content = "Evaluating data dispersion";
            //            labelLearningPhaseInitDispersion.Background = Brushes.DarkGoldenrod;
            //        }));
            //    }
            //    else
            //    {
            //        labelLearningPhaseInitDispersion.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            //        {
            //            labelLearningPhaseInitDispersion.Content = "Data dispersion Evaluated";
            //            labelLearningPhaseInitDispersion.Background = Brushes.DarkGreen;
            //        }));
            //        labelLearningPhaseTrainingClusters.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            //        {
            //            labelLearningPhaseTrainingClusters.Content = "Training Clusters";
            //            labelLearningPhaseTrainingClusters.Background = Brushes.DarkGoldenrod;
            //        }));
            //    }
            //}
            //else
            //{
            //    labelLearningPhaseTrainingClusters.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            //    {
            //        labelLearningPhaseTrainingClusters.Content = "Clusters trained";
            //        labelLearningPhaseTrainingClusters.Background = Brushes.DarkGreen;
            //    }));
            //    labelLearningPhaseMonitoringFailures.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            //    {
            //        labelLearningPhaseMonitoringFailures.Content = "Monitoring failures";
            //        labelLearningPhaseMonitoringFailures.Background = Brushes.DarkGreen;
            //    }));
            //}

        }

        XyzDataSeries3D<double> xyzDataSeries3DCluster = new XyzDataSeries3D<double>() { SeriesName = "Clusters" };
        XyzDataSeries3D<double> xyzDataSeries3DDispersion = new XyzDataSeries3D<double>() { SeriesName = "Dispersions" };
        XyzDataSeries3D<double> xyzDataSeries3DDistance = new XyzDataSeries3D<double>() { SeriesName = "Range" };
        XyzDataSeries3D<double> xyzDataSeries3DSample = new XyzDataSeries3D<double>() { SeriesName = "Samples" };
        XyzDataSeries3D<double> xyzDataSeries3DHighlight = new XyzDataSeries3D<double>() { SeriesName = "Highlight" };
        ObservableCollection<Cluster> clustersData = new ObservableCollection<Cluster>();
        double[,] Clusters = new double[25, 10];
        double[,] Dispersions = new double[25, 10];
        double[] SamplesOnCluster = new double[25];
        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        public void ActualizeUnsupervisedLearningClusterInfo(object sender, UnsupervisedLearningParametersArgs e)
        {
            //La solution consiste a passer par un delegué qui executera l'action a effectuer depuis le thread concerné.
            //Ici, l'action a effectuer est la modification d'un bouton. Ce bouton est un objet UI, et donc l'action doit etre executée depuis un thread UI.
            //Sachant que chaque objet UI (d'interface graphique) dispose d'un dispatcher qui permet d'executer un delegué (une methode) depuis son propre thread.
            ////La difference entre un Invoke et un beginInvoke est le fait que le Invoke attend la fin de l'execution de l'action avant de sortir.
            //labelSpeed.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            //{
            //    labelSpeed.Content = Convert.ToString(e.Value);
            //}));

            //GetData() creates a collection of Customer data from a database

            for (int i = 0; i < 3; i++)
            {
                Clusters[e.ClusterIndex, i] = e.Clusters[e.ClusterIndex, i];
                Dispersions[e.ClusterIndex, i] = e.Dispersions[e.ClusterIndex, i];
            }
            SamplesOnCluster[e.ClusterIndex] = e.NumberOfSamplesInCluster[e.ClusterIndex];
            numberOfClusters = e.numberOfClusters;
            if (Training)
            {
                if (numberOfClusters == 0)
                {
                    labelLearningPhaseInitDispersion.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        labelLearningPhaseInitDispersion.Content = "Evaluating data dispersion";
                        labelLearningPhaseInitDispersion.Background = Brushes.DarkGoldenrod;
                    }));
                }
                else
                {
                    labelLearningPhaseInitDispersion.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        labelLearningPhaseInitDispersion.Content = "Data dispersion Evaluated";
                        labelLearningPhaseInitDispersion.Background = Brushes.DarkGreen;
                    }));
                    labelLearningPhaseTrainingClusters.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        labelLearningPhaseTrainingClusters.Content = "Training Clusters";
                        labelLearningPhaseTrainingClusters.Background = Brushes.DarkGoldenrod;
                    }));
                }
            }
            else
            {
                labelLearningPhaseTrainingClusters.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    labelLearningPhaseTrainingClusters.Content = "Clusters trained";
                    labelLearningPhaseTrainingClusters.Background = Brushes.DarkGreen;
                }));
                labelLearningPhaseMonitoringFailures.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    labelLearningPhaseMonitoringFailures.Content = "Monitoring failures";
                    labelLearningPhaseMonitoringFailures.Background = Brushes.DarkGreen;
                }));
            }

            // Si on reçoir le premier cluster on clear la collection de clusters
            if (e.ClusterIndex == 0)
            {

                this.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    //this.clustersData.Clear();

                    ObservableCollection<Cluster> newClusterList = new ObservableCollection<Cluster>();

                    for (int i = 0; i < e.numberOfClusters; i++)
                    {
                        ////New cluster recevied
                        Cluster cluster = new Cluster();
                        cluster.ClusterID = i;// e.ClusterIndex;
                        cluster.NumberOfSamplesInCluster = SamplesOnCluster[i]; // e.NumberOfSamplesInCluster[e.ClusterIndex];
                        cluster.VolumeArea = Clusters[i, 0];//e.Clusters[e.ClusterIndex, 0];
                        cluster.PressurePeak = Clusters[i, 1]; //e.Clusters[e.ClusterIndex, 1];
                        cluster.PressureArea = Clusters[i, 2]; //e.Clusters[e.ClusterIndex, 2];
                        cluster.DispersionVolumeArea = Dispersions[i, 0];//e.Dispersions[e.ClusterIndex, 0];
                        cluster.DispersionPressurePeak = Dispersions[i, 1];//e.Dispersions[e.ClusterIndex, 1];
                        cluster.DispersionPressureArea = Dispersions[i, 2];//e.Dispersions[e.ClusterIndex, 2];
                        newClusterList.Add(cluster);
                    }
                    clustersData = newClusterList;
                });
            }
            ClusterFeaturesGroupBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                DataGridCluster.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {                
                if (DataGridCluster.IsVisible) DataGridCluster.DataContext = clustersData;
            }));
            }));

            sciChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                ScatterSeries3DCluster.DataSeries = xyzDataSeries3DCluster;
                ScatterSeries3DClusterDispersion.DataSeries = xyzDataSeries3DDispersion;
                ScatterSeries3DClusterDistance.DataSeries = xyzDataSeries3DDistance;

                if (e.ClusterIndex == 0)
                {
                    xyzDataSeries3DCluster.Clear();
                    xyzDataSeries3DDispersion.Clear();
                    xyzDataSeries3DDistance.Clear();
                }
                float DispersionMult = (float) (100.0f * Dispersions[e.ClusterIndex, 0] * Dispersions[e.ClusterIndex, 1] * Dispersions[e.ClusterIndex, 2]);
                float DispersionRadio = (float) Math.Pow(DispersionMult, 0.33333f);

                Color clusterColor = Color.FromArgb(0xFF, 255, 0, 0);
                float clusterScale = 0.3f;
                Color dispersionColor = Color.FromArgb(0xBB, 125, 0, 0);
                //float dispersionScale = 2.2f;
                float dispersionScale = 0.7f + (float)(DispersionRadio/25.0f);
                if (dispersionScale > 4.2)
                    dispersionScale = 4.2f;
                Color distanceColor = Color.FromArgb(0x11, 0, 255, 0);
                //float distanceScale = 3.2f;
                float distanceScale = 4.3f;

                xyzDataSeries3DCluster.Append(e.Clusters[e.ClusterIndex, 0], e.Clusters[e.ClusterIndex, 1], e.Clusters[e.ClusterIndex, 2]
                    , new PointMetadata3D(clusterColor, clusterScale));
                xyzDataSeries3DCluster.Append(e.Clusters[e.ClusterIndex, 0], e.Clusters[e.ClusterIndex, 1], e.Clusters[e.ClusterIndex, 2]
                    , new PointMetadata3D(dispersionColor, dispersionScale));
                xyzDataSeries3DCluster.Append(e.Clusters[e.ClusterIndex, 0], e.Clusters[e.ClusterIndex, 1], e.Clusters[e.ClusterIndex, 2]
                    , new PointMetadata3D(distanceColor, distanceScale));

                //xyzDataSeries3DCluster.Append(e.Clusters[e.ClusterIndex, 0], e.Clusters[e.ClusterIndex, 1], e.Clusters[e.ClusterIndex, 2]);
                //xyzDataSeries3DDispersion.Append(e.Clusters[e.ClusterIndex, 0], e.Clusters[e.ClusterIndex, 1], e.Clusters[e.ClusterIndex, 2]);
                //xyzDataSeries3DDistance.Append(e.Clusters[e.ClusterIndex, 0], e.Clusters[e.ClusterIndex, 1], e.Clusters[e.ClusterIndex, 2]);

                //EllipseDispersion.Size = (float)Math.Sqrt(Math.Pow(e.Dispersions[e.ClusterIndex, 0], 2) + Math.Pow(e.Dispersions[e.ClusterIndex, 1], 2) +
                //Math.Pow(e.Dispersions[e.ClusterIndex, 0], 2));
                sciChart.ZoomExtents();
                IRange axisX3D = axisX.GetMaximumRange();
                IRange axisY3D = axisY.GetMaximumRange();
                IRange axisZ3D = axisZ.GetMaximumRange();

                axisX3D.SetMinMax((double)axisX3D.Min - AxisOffset, (double)axisX3D.Max + AxisOffset);
                axisY3D.SetMinMax((double)axisY3D.Min - AxisOffset, (double)axisY3D.Max + AxisOffset);
                axisZ3D.SetMinMax((double)axisZ3D.Min - AxisOffset, (double)axisZ3D.Max + AxisOffset);
                //sciChart.WorldDimensions.X = (float)(((double)axisX3D.Max - (double)axisX3D.Min) + AxisOffset);
                //sciChart.WorldDimensions.Y = (float)(((double)axisY3D.Max - (double)axisY3D.Min) + AxisOffset);
                //sciChart.WorldDimensions.Z = (float)(((double)axisZ3D.Max - (double)axisZ3D.Min) + AxisOffset);
                CameraSciChart.Radius = 500;
            }));

        }

        ObservableCollection<Sample> samplesData = new ObservableCollection<Sample>();
        double samplesCounter = 0;
        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        public void ActualizeUnsupervisedLearningSamplesInfo(object sender, UnsupervisedLearningParametersArgs e)
        {
            //La solution consiste a passer par un delegué qui executera l'action a effectuer depuis le thread concerné.
            //Ici, l'action a effectuer est la modification d'un bouton. Ce bouton est un objet UI, et donc l'action doit etre executée depuis un thread UI.
            //Sachant que chaque objet UI (d'interface graphique) dispose d'un dispatcher qui permet d'executer un delegué (une methode) depuis son propre thread.
            ////La difference entre un Invoke et un beginInvoke est le fait que le Invoke attend la fin de l'execution de l'action avant de sortir.
            //labelSpeed.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            //{
            //    labelSpeed.Content = Convert.ToString(e.Value);
            //}));

            //New sample recevied
            Sample sample = new Sample();

            samplesCounter++;

            sample.SampleID = samplesCounter;
            sample.VolumeArea = e.Sample[0];
            sample.PressurePeak = e.Sample[1];
            sample.PressureArea = e.Sample[2];

            samplesData.Add(sample);

            sciChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                ScatterSeries3DSample.DataSeries = xyzDataSeries3DSample;

                xyzDataSeries3DSample.Append(e.Sample[0], e.Sample[1], e.Sample[2]);

                for (int i = 0; i < numberOfClusters; i++)
                {
                    double dist = 0;
                    for (int j = 0; j < 3; j++)
                    {
                        if (Clusters[i, j] > 1)
                            //calculate distance between the sample and the cluster
                            dist += Math.Pow((Clusters[i, j] - e.Sample[j]) / Clusters[i, j], 2.0f);
                        else
                            dist += Math.Pow((Clusters[i, j] - e.Sample[j]) / (Clusters[i, j] + 1), 2.0f);
                    }

                    dist = Math.Sqrt(dist);
                    if (dist < 1.0f)
                    {
                        xyzDataSeries3DHighlight.Clear();
                        ScatterSeries3DHighlight.DataSeries = xyzDataSeries3DHighlight;
                        xyzDataSeries3DHighlight.Append(Clusters[i, 0], Clusters[i, 1], Clusters[i, 2]);
                        //ClusterActualSeries.DataSeries = clusterActualSeries;
                        //clusterActualSeries.AcceptsUnsortedData = true;
                        //clusterActualSeries.Clear();
                        //clusterActualSeries.Append(Clusters[i, 1], Clusters[i, 2]);
                        timerAnimationCluster.Start();
                    }
                }


                IRange Yaxisdatavalue = axisY.GetMaximumRange();//346
                IRange Xaxisdatavalue = axisX.GetMaximumRange();//244.4
                                                                //The size of the ellipse is variable respecto to the size of the graph
                                                                //ClusterEllipse.Width = 140;
                                                                //ClusterEllipse.Height = 140;
                double CustomAreaGraph = 346 * 244.4;
                double areaGraph = ((double)Yaxisdatavalue.Max - (double)Yaxisdatavalue.Min) *
                    ((double)Xaxisdatavalue.Max - (double)Xaxisdatavalue.Min);
                double ellipseRadio = 140 * CustomAreaGraph / areaGraph;

                double minimumEllipseRadio = 30;
                if (ellipseRadio > minimumEllipseRadio)
                {
                    //ClusterEllipse.Width = (int)ellipseRadio;
                    //ClusterEllipse.Height = (int)ellipseRadio;

                    //ClusterEllipseLimit.Width = (int)ellipseRadio;
                    //ClusterEllipseLimit.Height = (int)ellipseRadio;
                }
                else
                {
                    //ClusterEllipse.Width = (int)minimumEllipseRadio;
                    //ClusterEllipse.Height = (int)minimumEllipseRadio;

                    //ClusterEllipseLimit.Width = (int)minimumEllipseRadio;
                    //ClusterEllipseLimit.Height = (int)minimumEllipseRadio;
                }

                sciChart.ZoomExtents();
                IRange axisX3D = axisX.GetMaximumRange();
                IRange axisY3D = axisY.GetMaximumRange();
                IRange axisZ3D = axisZ.GetMaximumRange();

                axisX3D.SetMinMax((double)axisX3D.Min - AxisOffset, (double)axisX3D.Max + AxisOffset);
                axisY3D.SetMinMax((double)axisY3D.Min - AxisOffset, (double)axisY3D.Max + AxisOffset);
                axisZ3D.SetMinMax((double)axisZ3D.Min - AxisOffset, (double)axisZ3D.Max + AxisOffset);
                //sciChart.WorldDimensions.X = (float)((double)axisX3D.Max - (double)axisX3D.Min);
                //sciChart.WorldDimensions.Y = (float)((double)axisY3D.Max - (double)axisY3D.Min);
                //sciChart.WorldDimensions.Z = (float)((double)axisZ3D.Max - (double)axisZ3D.Min);
                //sciChart.WorldDimensions.X = (float)(((double)axisX3D.Max - (double)axisX3D.Min) + AxisOffset);
                //sciChart.WorldDimensions.Y = (float)(((double)axisY3D.Max - (double)axisY3D.Min) + AxisOffset);
                //sciChart.WorldDimensions.Z = (float)(((double)axisZ3D.Max - (double)axisZ3D.Min) + AxisOffset);

                CameraSciChart.Radius = 700;
             }));

        }
        bool alarmActivated = false;
        //Methode appelée sur evenement (event) provenant du port Serie.
        //Cette methode est donc appelée depuis le thread du port Serie. Ce qui peut poser des problemes d'acces inter-thread
        public void AlarmSystem(object sender, UnsupervisedLearningParametersArgs e)
        {

            labelState.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                labelState.Background = Brushes.Red;
                labelState.Content = "ALERT";
            }));
            xyzDataSeries3DSample.Clear();
             SuccessiveAnomalies = 0;
            alarmActivated = true;
            timerAlarm.Start();
            //OnStartStopFromInterface(false);
            new SoundPlayer(@"..\..\..\TF002.WAV").Play();
            timerDoubleAlarm.Start();
            xyzDataSeries3DCluster.Clear();
            sciChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                ScatterSeries3DCluster.DataSeries = xyzDataSeries3DCluster;
            }));
        }

            double timercounter = 0;
        void timerAnimationClusterFunction(object sender, EventArgs e)
        {
            sciChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                timercounter++;
                if (timercounter < 8)
                {
                    //EllipseCluster.Size += 5f;
                    //EllipseCluster.Opacity += 0.075f;

                    EllipseHighlight.Size += 10f;
                    EllipseHighlight.Opacity += 0.075f;
                }
                else
                {
                    //EllipseCluster.Size -= 5f;
                    //EllipseCluster.Opacity -= 0.075f;

                    EllipseHighlight.Size -= 10f;
                    EllipseHighlight.Opacity -= 0.075f;

                    if (timercounter >= 16)
                    {

                        //EllipseCluster.Size = 20f;
                        //EllipseCluster.Opacity = 0.9f;

                        EllipseHighlight.Size = 0.1f;
                        EllipseHighlight.Opacity = 0.2f;

                        sciChart.ZoomExtents();
                        IRange axisX3D = axisX.GetMaximumRange();
                        IRange axisY3D = axisY.GetMaximumRange();
                        IRange axisZ3D = axisZ.GetMaximumRange();

                        axisX3D.SetMinMax((double)axisX3D.Min - AxisOffset, (double)axisX3D.Max + AxisOffset);
                        axisY3D.SetMinMax((double)axisY3D.Min - AxisOffset, (double)axisY3D.Max + AxisOffset);
                        axisZ3D.SetMinMax((double)axisZ3D.Min - AxisOffset, (double)axisZ3D.Max + AxisOffset);

                        //sciChart.WorldDimensions.X = (float)((double)axisX3D.Max - (double)axisX3D.Min);
                        //sciChart.WorldDimensions.Y = (float)((double)axisY3D.Max - (double)axisY3D.Min);
                        //sciChart.WorldDimensions.Z = (float)((double)axisZ3D.Max - (double)axisZ3D.Min);
                        //sciChart.WorldDimensions.X = (float)(((double)axisX3D.Max - (double)axisX3D.Min) + AxisOffset);
                        //sciChart.WorldDimensions.Y = (float)(((double)axisY3D.Max - (double)axisY3D.Min) + AxisOffset);
                        //sciChart.WorldDimensions.Z = (float)(((double)axisZ3D.Max - (double)axisZ3D.Min) + AxisOffset);

                        CameraSciChart.Radius = 500;

                        timercounter = 0;
                        timerAnimationCluster.Stop();
                    }

                }
            }));
        }


        void timerCameraFunction(object sender, EventArgs e)
        {
            sciChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                CameraSciChart.OrbitalYaw += 0.5f;
            }));
            timerCamera.Start();
        }

        int alarmCounter = 0;
        bool BackgroundRED = false;
        void timerAlarmFunction(object sender, EventArgs e)
        {
            alarmCounter++;

            if (!BackgroundRED)
            {
                GridApplication.Background = Brushes.Red;
                ClusterFeaturesGroupBox.Background = Brushes.Red;
                oscilloPression.Background = Brushes.Red;
                oscilloVolume.Background = Brushes.Red;
                sciChart.Background = Brushes.Red;
                BackgroundRED = true;
            }
            else
            {
                GridApplication.Background = new SolidColorBrush(Color.FromRgb(0x22, 0x22, 0x22));
                ClusterFeaturesGroupBox.Background = new SolidColorBrush(Color.FromRgb(0x22, 0x22, 0x22));
                oscilloPression.Background = Brushes.Black;
                oscilloVolume.Background = Brushes.Black;
                sciChart.Background = Brushes.Black;
                BackgroundRED = false;
            }


            if (alarmCounter >= 21)
            {
                labelState.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    labelState.Background = (Brush)converter.ConvertFromString("#222222");
                    labelState.Content = "Normal mode";
                }));

                labelLearningPhaseInitDispersion.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    labelLearningPhaseInitDispersion.Content = " ";
                    labelLearningPhaseInitDispersion.Background = (Brush)converter.ConvertFromString("#222222");
                }));
                labelLearningPhaseTrainingClusters.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    labelLearningPhaseTrainingClusters.Content = " ";
                    labelLearningPhaseTrainingClusters.Background = (Brush)converter.ConvertFromString("#222222");
                }));
                labelLearningPhaseMonitoringFailures.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    labelLearningPhaseMonitoringFailures.Content = " ";
                    labelLearningPhaseMonitoringFailures.Background = (Brush)converter.ConvertFromString("#222222");
                }));
                this.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    this.clustersData.Clear();
                    this.xyzDataSeries3DCluster.Clear();
                    this.xyzDataSeries3DDispersion.Clear();
                    this.xyzDataSeries3DDistance.Clear();
                    this.xyzDataSeries3DSample.Clear();
                    this.xyzDataSeries3DHighlight.Clear();
                    this.clustersData.Clear();
                });
                sciChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    ScatterSeries3DCluster.DataSeries = xyzDataSeries3DCluster;
                    ScatterSeries3DClusterDispersion.DataSeries = xyzDataSeries3DDispersion;
                    ScatterSeries3DClusterDistance.DataSeries = xyzDataSeries3DDistance;
                    ScatterSeries3DSample.DataSeries = xyzDataSeries3DSample;
                }));

                //labelLearningStatus.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                //{
                //        labelLearningStatus.Content = "Status: Stopped";
                //}));

                for (int i=0; i < 25; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        Clusters[i, j] = 0.0f;
                        Dispersions[i, j] = 0.0f;
                    }
                    SamplesOnCluster[i] = 0.0f;
                }
                alarmActivated = false;
                SuccessiveAnomalies = 0;
                ActualAnomaliesNumber = 0;
                Training = true;
                numberOfClusters = 0;
                clearGraphs = true;
                //OnStartStopFromInterface(true);
                alarmCounter = 0;
                timerAlarm.Stop();
                GridApplication.Background = new SolidColorBrush(Color.FromRgb(0x22, 0x22, 0x22));
                //OnInitMachineFromInterface();
            }
        }

        void timerDoubleAlarmFunction(object sender, EventArgs e)
        {   
            new SoundPlayer(@"..\..\..\TF002.WAV").Play();
            timerDoubleAlarm.Stop();            
        }

    }
}
