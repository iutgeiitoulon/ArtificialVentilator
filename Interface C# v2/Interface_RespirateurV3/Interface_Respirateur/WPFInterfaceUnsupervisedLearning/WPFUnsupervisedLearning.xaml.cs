using EventArgsLibrary;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals;
using SciChart.Data.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace WPFInterfaceUnsupervisedLearning
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class WpfUnsupervisedLearning : Window
    {
        XyDataSeries<double, double> sampleSeries = new XyDataSeries<double, double> { };
        XyDataSeries<double, double> clusterSeries = new XyDataSeries<double, double> { };
        XyDataSeries<double, double> clusterLimitSeries = new XyDataSeries<double, double> { };
        XyDataSeries<double, double> clusterActualSeries = new XyDataSeries<double, double> { };

        DispatcherTimer timerAnimationCluster;
        double timercounter = 0;

        int ActualAnomaliesNumber = 0;
        int SuccessiveAnomalies = 0;

        bool Training = true;
        int numberOfClusters = 1;
        double[,] Clusters = new double[25, 10];
        double[,] Dispersions = new double[25, 10];
        double[] SamplesOnCluster = new double[25];

        public WpfUnsupervisedLearning()
        {
            InitializeComponent();
            // Set this code once in AppDelegate or application startup
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in App.xaml.cs or application startup

            // TODO: Put your SciChart License Key here if needed
            // Set this code once in App.xaml.cs or application startup
            //            SciChartSurface.SetRuntimeLicenseKey("3ZfsjQ4YCVy5abTNosBHy7yZz+wrMJFGGy0olJEbGbbDoJCmcLIwe4pVqv1Z3yz7TQXMy+RgefltbnKFWrgxbq69b41b/fI/91qhYFsOpHncBiY301vyOAKJPsXQW1qnKvNRvWA/OZsqkBfdGNAaPsW5CD2Bc2pAhkxaVNr5hAECbwQGeLSkFuRyNKzQIqvMqiwijcGT4WBkpSO2DHrlzy+jKQxKe55sd9BKkpD/oR++FzCYLo5TVlqCrWFN3MHDBzGnXxES9p95rpKPQRJzEPMcj6SfSB5GEdnuFPswXT9FRk3dfUX8IF8keGobdOISXU6R+hPTv6beCx/EbSKR6BURxknbt7C8h8TTjA3NCbvlIdVXUe2nNIOF1OriMu+FewAWcJBLAdto/gAjTcreuhNTWShp+pR/SUTUv+/TZjotbCsFF1VLRu+DJtS/3yAA0MbHCqnKlA0RJh5J5s+tbdrc6voYHDBBaFvbSq6NzxZ6KUpq");
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in App.xaml.cs or application startup
            // Set this code once in App.xaml.cs or application startup
            //SciChartSurface.SetRuntimeLicenseKey("alZ8d1UhrI0H9SwzPAi19H+Jk7SBJw+7iITDtj8xgeuAPB5MboJg78xfEwnZj4UFhox2pFdcXJ8cxlaQBJKVrU8vZLL+HEeJE0ozWJt+T7nHUfEFTE+iLshRO1pizX6uG14y+ve3PNXtnvty+Aur4tuP7cvJo57amxRvRNKLOg+aoB8GSNx45Jbe9Erfm5OPL2cxboEqwW/ju7MoohzSv7KsfbLt2UilfgbszsTcCJt9WqQgScRp3rJ3+IsO+tpiG9iAYVSNnzcU8sCAfJWtiNVmkrAACu1eW9VS7h+qf/cCeHXRe/XDR9TBDKA6EKlqWrjPnC9HJO+ZA7PhHunweXHCGyG/YEA4EzZ2nLhczGlP7oJkYtnqkNJ4YGFcsmQ3t0cS3GHo+o7CZBQaIpOBaX49jXMOZa11iBiWOUYHRerIxxFEemkPi1CtrPoxD+6ppcbjkfeUceJG89R8xcnFp+LdJ4xDKQCZUNE9MWSoMvYknhxD");
            //SciChartSurface.SetRuntimeLicenseKey("D+rzQAwAqkMm5k9aeMUtypgMKfGGheCEFKWMpHn/lAesbSFiS8PyN05A7bUrVzE9AOFN0vulXuSiybLQsoZQA8ez2wfNzf5fcD0Ij0cGq58F65ARQ5bjw/6E7UIhqeaqpAbaaQ2lGIaoorrwzfiUllTM0CRNTtdWcMNCcy+N1uCNlpwhZj1IICLtOLo7PyT8GQkVIysEcXZ9/eFHK8MCT3GwBOAqOTpcbaehogozwy0/95Lm3aQs7IvKYTmi5ZkpT3Y+Ewep3gP5YjTz6L/rGW01898EhjwKv7Yof1vKmJcL4p0fgq1kUhpaZKR6epj7dFdBF7ZRSsxr/A9jOrmKunO1CIuq04vJO1rJu79w2InsBx3l2i6EYjF/0JJgcrtSU9LfgVilkS6X756fk8yyEwztBDJAIieQUt0B/rzfJp9FYKmnlXDwvbKM72inx04C7MLjOmPkudVItC5BZRcaYEJzXgIWHdqUdgGQEG3XNAFyin/o");
            //SciChartSurface.SetRuntimeLicenseKey("D+rzQAwAqkMm5k9aeMUtypgMKfGGheCEFKWMpHn/lAesbSFiS8PyN05A7bUrVzE9AOFN0vulXuSiybLQsoZQA8ez2wfNzf5fcD0Ij0cGq58F65ARQ5bjw/6E7UIhqeaqpAbaaQ2lGIaoorrwzfiUllTM0CRNTtdWcMNCcy+N1uCNlpwhZj1IICLtOLo7PyT8GQkVIysEcXZ9/eFHK8MCT3GwBOAqOTpcbaehogozwy0/95Lm3aQs7IvKYTmi5ZkpT3Y+Ewep3gP5YjTz6L/rGW01898EhjwKv7Yof1vKmJcL4p0fgq1kUhpaZKR6epj7dFdBF7ZRSsxr/A9jOrmKunO1CIuq04vJO1rJu79w2InsBx3l2i6EYjF/0JJgcrtSU9LfgVilkS6X756fk8yyEwztBDJAIieQUt0B/rzfJp9FYKmnlXDwvbKM72inx04C7MLjOmPkudVItC5BZRcaYEJzXgIWHdqUdgGQEG3XNAFyin/o");
            //SciChartSurface.SetRuntimeLicenseKey("9MjJT/tE1wWKl0wgwkMsWNasyLEGagtVpe5gfjlsOkOj2rEqBgJHzS/zCy2EK/Pud+XoEXcgI90YxRZrSJn6lFsP1OhZd6UtHe3N5vrjYcnJzFbEJ9tyJ8ezFTGtzQtf9nxEger6Bgx6VK2DnZGbeT2Z5rRiU/qdqGb0QanIRJ5Hl/F8tVU0aych/39v1AAjDGzUmYqArBIfwlk/2K/H1f3Vo3v74pgBp8SvBaN/EF9v2gKCAR6ktbXUtrJWeFdXywbr6hIkj2fD+Zfs27VYEzcdJdVXML1N3uoIjNybMVHg8+FcM2D01tpwsgypa1zb0B1XrHNa+Ln/wOSXmS9S4IvNmyp1W7OLaprL//ueVihKw+t4cCvN9ksvO8bmTUfCl6JW9uHACHMqLE0WtxnIoUWl71yOkBrDDeIR6m4okp5BBN71gvtF95PzuAQz8aCoOYQg9JdX8c++Q4NFefsObGEItnAAhs89WtZJYn6dQKuGztA2TeJW");
            //SciChartSurface.SetRuntimeLicenseKey("bW0PcRmktMo9TBbRKfDEOH7D/NUuY7Keo9cOSjXUPYYdcJxpQ/47ek6WUH+FUGE69eiej19VKGgmYuSGvfRLP5dzX9xvDuPs1V4SbknOsSCVU8q6qcE4/V73T9/kE9ojYzsFwXvBW9hFCLcPWuiHc2QQQpiQBlYt8HM0Yw8wf3VhF276eye02ycopdiLxEZmE65tuaNOR34j0ID7AEF+vZBUOhVvCgZw8krN9OEsTUu46isYLWXkDfxIN46G2Bxy5IAunZjOrMY6mbd+97YEpTAjpvNgZ4zqCQfwPFZ+NBIcOXNvhvGU/38XDYdIQpTzLE5EkvLIDSgD0O7MJSBfsG8D8sQ/QCp7do8y4Ege9cHQ43S93OLo3ORYG1ZZIGP1LCeuWJkw8C5du901ySJhJwjRiHLRgiFlMzEar6w5TbhcvR0h7Jrbth/hhAVXMX58k79JPSiQVny4EuO8XyOiHkOhBgijQMjBrU3vCFtuD5moKiu4lO5YWiCu+LT1Oie352EMzuaohwhbjDGoCuXsGw==");
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

            labelClusterNumberVal.Content = "0";
            labelAnomaliesNumberVal.Content = "0";
            labelTraining.Content = "Training";

            timerAnimationCluster = new DispatcherTimer();
            timerAnimationCluster.Interval = TimeSpan.FromMilliseconds(15);
            timerAnimationCluster.Tick += timerAnimationClusterFunction;

        }

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

            labelClusterNumberVal.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                labelClusterNumberVal.Content = e.numberOfClusters.ToString();
            }));

            labelAnomaliesNumberVal.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                labelAnomaliesNumberVal.Content = e.numberOfAnomalies.ToString();
            }));

            labelTraining.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                if (e.trainingState == 1)
                {
                    labelTraining.Content = "Training";
                }
                else
                {
                    Training = false;
                    labelTraining.Content = "Detection";
                }
            }));

            //Detect successive anomalies 
            if (ActualAnomaliesNumber == 0)
            {
                ActualAnomaliesNumber = e.numberOfAnomalies;
            }
            else
            {
                if (e.numberOfAnomalies - ActualAnomaliesNumber > 0)
                {
                    SuccessiveAnomalies += 2;
                    ActualAnomaliesNumber = e.numberOfAnomalies;
                }
                else
                {
                    if (SuccessiveAnomalies > 0)
                        SuccessiveAnomalies--;
                    ActualAnomaliesNumber = e.numberOfAnomalies;
                }
            }
            switch (SuccessiveAnomalies)
            {
                case 1:
                    labelAnomaliesNumber.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        labelAnomaliesNumber.Background = Brushes.YellowGreen;
                    }));

                    labelAnomaliesNumberVal.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        labelAnomaliesNumberVal.Background = Brushes.YellowGreen;
                    }));
                    break;
                case 2:
                    labelAnomaliesNumber.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        labelAnomaliesNumber.Background = Brushes.Orange;
                    }));

                    labelAnomaliesNumberVal.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        labelAnomaliesNumberVal.Background = Brushes.Orange;
                    }));
                    break;
                case 3:
                    labelAnomaliesNumber.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        labelAnomaliesNumber.Background = Brushes.DarkOrange;
                    }));

                    labelAnomaliesNumberVal.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        labelAnomaliesNumberVal.Background = Brushes.DarkOrange;
                    }));
                    break;
                case 4:
                    labelAnomaliesNumber.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        labelAnomaliesNumber.Background = Brushes.OrangeRed;
                    }));

                    labelAnomaliesNumberVal.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        labelAnomaliesNumberVal.Background = Brushes.OrangeRed;
                    }));
                    break;
                case 5:
                    labelAnomaliesNumber.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        labelAnomaliesNumber.Background = Brushes.Red;
                    }));

                    labelAnomaliesNumberVal.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        labelAnomaliesNumberVal.Background = Brushes.Red;
                    }));

                    SuccessiveAnomalies = 0;

                    new SoundPlayer(@"..\..\..\TF002.WAV").Play();

                    break;
                default:
                    labelAnomaliesNumber.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        var converter = new System.Windows.Media.BrushConverter();
                        labelAnomaliesNumber.Background = (Brush)converter.ConvertFromString("#222222");
                    }));

                    labelAnomaliesNumberVal.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        var converter = new System.Windows.Media.BrushConverter();
                        labelAnomaliesNumberVal.Background = (Brush)converter.ConvertFromString("#222222");
                    }));
                    break;
            }


        }
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

            ClustersInfo.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                ClustersInfo.Text += "Cluster" + (e.ClusterIndex) + ": ";
                for (int i = 0; i < e.ClustersDimension; i++)
                {
                    ClustersInfo.Text += e.Clusters[e.ClusterIndex, i] + "; ";
                }
                ClustersInfo.Text += "\r\n";
                for (int i = 0; i < e.ClustersDimension; i++)
                {
                    ClustersInfo.Text += e.Dispersions[e.ClusterIndex, i] + "; ";
                }
                ClustersInfo.Text += "\r\n";
                ClustersInfo.Text += "Number of Samples = " + (e.NumberOfSamplesInCluster) + "\r\n ";
            }));

            sciChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                ClusterSeries.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    if (e.ClusterIndex == 0)
                    {
                        clusterSeries.Clear();
                        clusterLimitSeries.Clear();
                    }

                    ClusterSeries.DataSeries = clusterSeries;
                    clusterSeries.AcceptsUnsortedData = true;

                    ClusterLimitSeries.DataSeries = clusterLimitSeries;
                    clusterLimitSeries.AcceptsUnsortedData = true;

                    clusterSeries.Append(e.Clusters[e.ClusterIndex, 1], e.Clusters[e.ClusterIndex, 2]);
                    clusterLimitSeries.Append(e.Clusters[e.ClusterIndex, 1], e.Clusters[e.ClusterIndex, 2]);

                    for (int i = 0; i < 3; i++)
                    {
                        Clusters[e.ClusterIndex, i] = e.Clusters[e.ClusterIndex, i];
                        Dispersions[e.ClusterIndex, i] = e.Dispersions[e.ClusterIndex, i];
                        SamplesOnCluster[e.ClusterIndex] = e.NumberOfSamplesInCluster[e.ClusterIndex];
                    }
                    sciChart.ZoomExtents();
                }));
            }));


            switch (e.ClusterIndex)
            {
                case 0:
                    Cluster0.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster0.Content = "1";
                    }));
                    Cluster0VolumeArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster0VolumeArea.Content = e.Clusters[e.ClusterIndex, 0];
                    }));
                    Cluster0PressurePeak.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster0PressurePeak.Content = e.Clusters[e.ClusterIndex, 1];
                    }));
                    Cluster0PressureArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster0PressureArea.Content = e.Clusters[e.ClusterIndex, 2];
                    }));
                    break;
                case 1:
                    Cluster1.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster1.Content = "2";
                    }));
                    Cluster1VolumeArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster1VolumeArea.Content = e.Clusters[e.ClusterIndex, 0];
                    }));
                    Cluster1PressurePeak.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster1PressurePeak.Content = e.Clusters[e.ClusterIndex, 1];
                    }));
                    Cluster1PressureArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster1PressureArea.Content = e.Clusters[e.ClusterIndex, 2];
                    }));
                    break;
                case 2:
                    Cluster2.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster2.Content = "3";
                    }));
                    Cluster2VolumeArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster2VolumeArea.Content = e.Clusters[e.ClusterIndex, 0];
                    }));
                    Cluster2PressurePeak.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster2PressurePeak.Content = e.Clusters[e.ClusterIndex, 1];
                    }));
                    Cluster2PressureArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster2PressureArea.Content = e.Clusters[e.ClusterIndex, 2];
                    }));
                    break;
                case 3:
                    Cluster3.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster3.Content = "4";
                    }));
                    Cluster3VolumeArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster3VolumeArea.Content = e.Clusters[e.ClusterIndex, 0];
                    }));
                    Cluster3PressurePeak.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster3PressurePeak.Content = e.Clusters[e.ClusterIndex, 1];
                    }));
                    Cluster3PressureArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster3PressureArea.Content = e.Clusters[e.ClusterIndex, 2];
                    }));
                    break;
                case 4:
                    Cluster4.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster4.Content = "5";
                    }));
                    Cluster4VolumeArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster4VolumeArea.Content = e.Clusters[e.ClusterIndex, 0];
                    }));
                    Cluster4PressurePeak.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster4PressurePeak.Content = e.Clusters[e.ClusterIndex, 1];
                    }));
                    Cluster4PressureArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster4PressureArea.Content = e.Clusters[e.ClusterIndex, 2];
                    }));
                    break;
                case 5:
                    Cluster5.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster5.Content = "6";
                    }));
                    Cluster5VolumeArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster5VolumeArea.Content = e.Clusters[e.ClusterIndex, 0];
                    }));
                    Cluster5PressurePeak.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster5PressurePeak.Content = e.Clusters[e.ClusterIndex, 1];
                    }));
                    Cluster5PressureArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster5PressureArea.Content = e.Clusters[e.ClusterIndex, 2];
                    }));
                    break;
                case 6:
                    Cluster6.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster6.Content = "7";
                    }));
                    Cluster6VolumeArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster6VolumeArea.Content = e.Clusters[e.ClusterIndex, 0];
                    }));
                    Cluster6PressurePeak.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster6PressurePeak.Content = e.Clusters[e.ClusterIndex, 1];
                    }));
                    Cluster6PressureArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster6PressureArea.Content = e.Clusters[e.ClusterIndex, 2];
                    }));
                    break;
                case 7:
                    Cluster7.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster7.Content = "8";
                    }));
                    Cluster7VolumeArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster7VolumeArea.Content = e.Clusters[e.ClusterIndex, 0];
                    }));
                    Cluster7PressurePeak.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster7PressurePeak.Content = e.Clusters[e.ClusterIndex, 1];
                    }));
                    Cluster7PressureArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster7PressureArea.Content = e.Clusters[e.ClusterIndex, 2];
                    }));
                    break;
                case 8:
                    Cluster8.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster8.Content = "9";
                    }));
                    Cluster8VolumeArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster8VolumeArea.Content = e.Clusters[e.ClusterIndex, 0];
                    }));
                    Cluster8PressurePeak.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster8PressurePeak.Content = e.Clusters[e.ClusterIndex, 1];
                    }));
                    Cluster8PressureArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster8PressureArea.Content = e.Clusters[e.ClusterIndex, 2];
                    }));
                    break;
                case 9:
                    Cluster9.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster9.Content = "10";
                    }));
                    Cluster9VolumeArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster9VolumeArea.Content = e.Clusters[e.ClusterIndex, 0];
                    }));
                    Cluster9PressurePeak.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster9PressurePeak.Content = e.Clusters[e.ClusterIndex, 1];
                    }));
                    Cluster9PressureArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        Cluster9PressureArea.Content = e.Clusters[e.ClusterIndex, 2];
                    }));
                    break;
                default: break;
            }

        }
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



            SamplesInfo.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                SamplesInfo.Text += "Sample" + ": ";
                for (int i = 0; i < e.SampleDimension; i++)
                    SamplesInfo.Text += e.Sample[i] + "; ";
                SamplesInfo.Text += "\r\n";
            }));

            SampleVolumeArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                SampleVolumeArea.Content = e.Sample[0];
            }));
            SamplePressurePeak.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                SamplePressurePeak.Content = e.Sample[1];
            }));
            SamplePressureArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                SamplePressureArea.Content = e.Sample[2];
            }));

            sciChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                SamplesSeries.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    ClusterActualSeries.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        YAxisChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                        {
                            XAxisChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                            {
                                ClusterEllipse.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                                {
                                    ClusterEllipseLimit.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                                    {
                                        SamplesSeries.DataSeries = sampleSeries;
                                        sampleSeries.AcceptsUnsortedData = true;
                                        sampleSeries.Append(e.Sample[1], e.Sample[2]);

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
                                                ClusterActualSeries.DataSeries = clusterActualSeries;
                                                clusterActualSeries.AcceptsUnsortedData = true;
                                                clusterActualSeries.Clear();
                                                clusterActualSeries.Append(Clusters[i, 1], Clusters[i, 2]);
                                                timerAnimationCluster.Start();
                                            }
                                        }


                                        IRange Yaxisdatavalue = YAxisChart.GetMaximumRange();//346
                                        IRange Xaxisdatavalue = XAxisChart.GetMaximumRange();//244.4
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
                                            ClusterEllipse.Width = (int)ellipseRadio;
                                            ClusterEllipse.Height = (int)ellipseRadio;

                                            ClusterEllipseLimit.Width = (int)ellipseRadio;
                                            ClusterEllipseLimit.Height = (int)ellipseRadio;
                                        }
                                        else
                                        {
                                            ClusterEllipse.Width = (int)minimumEllipseRadio;
                                            ClusterEllipse.Height = (int)minimumEllipseRadio;

                                            ClusterEllipseLimit.Width = (int)minimumEllipseRadio;
                                            ClusterEllipseLimit.Height = (int)minimumEllipseRadio;
                                        }

                                        sciChart.ZoomExtents();
                                    }));
                                }));
                            }));
                        }));
                    }));
                }));
            }));

        }
        void timerAnimationClusterFunction(object sender, EventArgs e)
        {
            sciChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
            SamplesSeries.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
            ClusterActualSeries.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                YAxisChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    XAxisChart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                    {
                        ClusterEllipse.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                        {
                            ClusterEllipseLimit.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
                            {
                                timercounter++;
                                if (timercounter < 8)
                                {
                                    ClusterEllipse.StrokeThickness++;
                                    ClusterEllipse.Width++;
                                    ClusterEllipse.Height++;
                                }
                                else
                                {
                                    ClusterEllipse.StrokeThickness -= 1;
                                    ClusterEllipse.Width--;
                                    ClusterEllipse.Height--;
                                    if (timercounter >= 16)
                                    {
                                        ClusterEllipse.StrokeThickness = 9;

                                        sciChart.ZoomExtents();

                                        IRange Yaxisdatavalue = YAxisChart.GetMaximumRange();//346
                                        IRange Xaxisdatavalue = XAxisChart.GetMaximumRange();//244.4
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
                                            if (ellipseRadio < 140)
                                            {
                                                ClusterEllipse.Width = (int)ellipseRadio;
                                                ClusterEllipse.Height = (int)ellipseRadio;

                                                ClusterEllipseLimit.Width = (int)ellipseRadio;
                                                ClusterEllipseLimit.Height = (int)ellipseRadio;
                                            }
                                            else
                                            {
                                                ClusterEllipse.Width = 140;
                                                ClusterEllipse.Height = 140;

                                                ClusterEllipseLimit.Width = 140;
                                                ClusterEllipseLimit.Height = 140;
                                            }
                                        }
                                        else
                                        {
                                            ClusterEllipse.Width = (int)minimumEllipseRadio;
                                            ClusterEllipse.Height = (int)minimumEllipseRadio;

                                            ClusterEllipseLimit.Width = (int)minimumEllipseRadio;
                                            ClusterEllipseLimit.Height = (int)minimumEllipseRadio;
                                        }

                                        timercounter = 0;
                                        timerAnimationCluster.Stop();
                                    }

                                }
                                }));
                        }));
                    }));
                }));
            }));
            }));
            }));
        

        }

    }
}
