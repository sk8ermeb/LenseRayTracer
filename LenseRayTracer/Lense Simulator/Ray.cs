using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Lense_Simulator
{
    public class Ray
    {
        public Vector3D Direction { get; set; }
        public Point3D Origin { get; set; }
        public Color RayColor { get; set; }
        public int TraceCount { get; set; } = 0;
        public object OriginObj { get; set; } = null;
        public Ray()
        {
            RayColor = Colors.White;
            Origin = new Point3D(0, 0, 0);
            Direction = new Vector3D(0, 0, 0);
        }
        public bool HasBeenSet()
        {
            if (TraceCount >0 || RayColor != Colors.White || Origin != new Point3D(0, 0, 0) || Direction != new Vector3D(0, 0, 0))
                return true;
            return false;
        }
        public static Point3D BadPoint = new Point3D() { X = double.NaN, Y = double.NaN, Z = double.NaN };
        public static bool isPoint3DBad(Point3D p3d)
        {
            if (double.IsNaN(p3d.X) && double.IsNaN(p3d.Y) && double.IsNaN(p3d.Z))
                return true;
            return false;
        }
        public Point3D GetRayPlaneIntersect(Plane PI)
        {            
            double Y = (-(PI.Slope_X * Origin.Y * Direction.X)
                - (PI.Slope_Z * Origin.Y * Direction.Z)
                + (PI.Slope_X * Origin.X * Direction.Y)
                + (PI.Slope_Z * Origin.Z * Direction.Y)
                + (PI.Off_Y * Direction.Y))
                /
                (Direction.Y
                - (PI.Slope_X * Direction.X) 
                - (PI.Slope_Z*Direction.Z));
            double X = ((Y - Origin.Y) * Direction.X / Direction.Y) + Origin.X;
            double Z = ((Y - Origin.Y) * Direction.Z / Direction.Y) + Origin.Z;
            Point3D intersect = new Point3D(X, Y, Z);

            if((Direction.X > 0 && X <= Origin.X) || (Direction.X < 0 && X >= Origin.X) ||
                (Direction.Y > 0 && Y <= Origin.Y) || (Direction.Y < 0 && Y >= Origin.Y) ||
                (Direction.Z > 0 && Z <= Origin.Z) || (Direction.Z < 0 && Z >= Origin.Z) 
                )
                return BadPoint;
            
            if(Math.Sqrt(Math.Pow(Origin.X - X, 2)+ Math.Pow(Origin.Y - Y, 2)+ Math.Pow(Origin.Z - Z, 2)) < .00001)
            {
                return BadPoint;
            }


            if (!PI.isPointWithinXY(intersect))
                return BadPoint;
            return intersect;
        }
        public double GetAbsDistanceToPoint(Point3D CheckPoint)
        {
            return Math.Sqrt(Math.Pow(CheckPoint.X - Origin.X, 2)
                + Math.Pow(CheckPoint.Y - Origin.Y, 2)
                + Math.Pow(CheckPoint.Z - Origin.Z, 2));
        }
    }
}
