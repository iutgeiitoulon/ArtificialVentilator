using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning
{
    public class Cluster
    {
        public double ClusterID { get; set; }
        public double NumberOfSamplesInCluster { get; set; }
        public double VolumeArea { get; set; }
        public double PressurePeak { get; set; }
        public double PressureArea { get; set; }
        public double DispersionVolumeArea { get; set; }
        public double DispersionPressurePeak { get; set; }
        public double DispersionPressureArea { get; set; }
    }
}
