using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Lense_Simulator
{
    public class Plane
    {
        public double Slope_X { get; set; } = 0;
        public double Slope_Z { get; set; } = 0;
        public double Off_Y { get; set; } = 0;
        public double XMin { get; set; } = double.MaxValue;
        public double XMax { get; set; } = double.MinValue;
        public double ZMin { get; set; } = double.MaxValue;
        public double ZMax { get; set; } = double.MinValue;
        public double GranularitySize { get; set; } = 0;
        public Plane()
        {

        }

        public bool isPointWithinXY(Point3D point)
        {
            return point.X > (XMin - GranularitySize/2)
                && point.X < (XMax + GranularitySize / 2)
                && point.Z > (ZMin - GranularitySize/2)
                && point.Z < (ZMax + GranularitySize/2);
        }
    }
}
