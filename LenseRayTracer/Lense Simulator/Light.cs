using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Lense_Simulator
{
    public class Light
    {
        public string Name { get; set; } = "";
        public enum SourceType { None, Spot, Infinite }
        public SourceType CurrentType { get; protected set; } = SourceType.None;

        public double _Theta { get; set; } = 180;
        public double _Phi { get; set; } = 0;
        public Color _Color{get; set;}
        public double _Granularity { get; protected set; } = 3;
        //public double GranularityAngle = 1;
        public double Intensity { get; protected set; } = 1;
        //public 

        public Light()
        {

        }
        public ModelVisual3D Model3D { get; set; } = new ModelVisual3D();
        public ModelVisual3D LightSource { get; set; } = new ModelVisual3D();
        
        public List<Ray> Rays { get; set; }
        
        //public Color LightColor { get; set; } = Colors.White;
        public List<Ray> GetRayListFromSpotLight(double GranularityAngle)
        {
            //Granularity has to be devided by 10 sense the function multiplies by ten for rendering uses
            Point3DCollection allpts = GenerateOriginLightVectors(Intensity, GranularityAngle);
            Rays = new List<Ray>();
            SpotLight sl = LightSource.Content as SpotLight;
            Color clr = sl.Color;
            double colormag = Math.Sqrt(clr.R * clr.R + clr.G * clr.G + clr.B * clr.B);
            double red = clr.R * Intensity / colormag;
            double green = clr.G * Intensity / colormag;
            double blue = clr.B * Intensity / colormag;
            if (red < 0)
                red = 0;
            if (red > 255)
                red = 255;
            if (green < 0)
                green = 0;
            if (green > 255)
                green = 255;
            if (blue < 0)
                blue = 0;
            if (blue > 255)
                blue = 255;
            Color RayColor = Color.FromRgb((byte)Math.Round(red), (byte)Math.Round(green), (byte)Math.Round(blue));
            for (int i=0;i<allpts.Count;i++)
            {
                Vector3D v3d = new Vector3D(allpts[i].X, allpts[i].Y, allpts[i].Z);

                double angle = AngleBetweenVectors(v3d, sl.Direction);
                if(angle <= sl.OuterConeAngle)
                {
                    Ray newray = new Ray();
                    newray.Direction = v3d;
                    newray.Origin = sl.Position;
                    newray.RayColor = RayColor;
                    
                    Rays.Add(newray);
                }
            }
            CurrentType = SourceType.Spot;
            return Rays;
        }
        public List<Ray> GetRayListFromInfiniteLight(Lense GroundPlane, Vector3D dir, Color cl, double intensity)
        {
            //Granularity has to be devided by 10 sense the function multiplies by ten for rendering uses
            //Point3DCollection allpts = GenerateOriginLightVectors(Intensity, GranularityAngle);
            Rays = new List<Ray>();
            //SpotLight sl = LightSource.Content as SpotLight;
            Color clr = cl;
            double colormag = Math.Sqrt(clr.R * clr.R + clr.G * clr.G + clr.B * clr.B);
            double red = clr.R * intensity / colormag;
            double green = clr.G * intensity / colormag;
            double blue = clr.B * intensity / colormag;
            if (red < 0)
                red = 0;
            if (red > 255)
                red = 255;
            if (green < 0)
                green = 0;
            if (green > 255)
                green = 255;
            if (blue < 0)
                blue = 0;
            if (blue > 255)
                blue = 255;
            Color RayColor = Color.FromRgb((byte)Math.Round(red), (byte)Math.Round(green), (byte)Math.Round(blue));
            if (GroundPlane != null)
            {
                for (int i = 0; i < GroundPlane.pices.Count; i++)
                {

                    Ray newray = new Ray();

                    Point3D endpt = GroundPlane.pices[i].PointInSpace;
                    Point3D origin = new Point3D(endpt.X - dir.X, endpt.Y - dir.Y, endpt.Z - dir.Z);
                    newray.Direction = dir;
                    newray.Origin = origin;
                    newray.RayColor = RayColor;

                    Rays.Add(newray);

                }
            }
            CurrentType = SourceType.Infinite;
            return Rays;
        }
        public List<Ray> GetRayListFromInfiniteLight(Lense GroundPlane, double Theta, double Phi, Color cl, double intensity)
        {
            double R = 1000;
            double dirZ = R * Math.Sin(Phi * Math.PI / 180) * Math.Cos(Theta * Math.PI / 180);
            double dirX = R * Math.Sin(Phi * Math.PI / 180) * Math.Sin(Theta * Math.PI / 180);
            double dirY = R * Math.Cos(Phi * Math.PI / 180);
            Vector3D dir = new Vector3D(dirX, dirY, dirZ);
            return GetRayListFromInfiniteLight(GroundPlane, dir, cl, intensity);
            //return Rays;
        }
        public static Vector3D VectorCrossProduct(Vector3D U, Vector3D V)
        {
            return new Vector3D(U.Y * V.Z - U.Z * V.Y, U.Z * V.X - U.X * V.Z, U.X * V.Y - U.Y * V.X);
        }
        public static Point3D RotatePointAroundAribtraryLine(Point3D PointToRotate, Point3D LinePoint1, Vector3D LineVector, double angleradians)
        {
            Vector3D L = new Vector3D(LineVector.X + LinePoint1.X, LineVector.Y + LinePoint1.Y, LineVector.Z + LinePoint1.Z);
            L.Normalize();
            double a = LinePoint1.X;
            double b = LinePoint1.Y;
            double c = LinePoint1.Z;
            double x = PointToRotate.X;
            double y = PointToRotate.Y;
            double z = PointToRotate.Z;
            double u = LineVector.X;
            double v = LineVector.Y;
            double w = LineVector.Z;

            double X = (a * (Math.Pow(v, 2) + (Math.Pow(w, 2))) - u * (b * v + c * w - u * x - v * y - w * z)) * (1 - Math.Cos(angleradians)) + x * Math.Cos(angleradians) + (-c * v + b * w - w * y + v * z) * Math.Sin(angleradians);
            double Y = (b * (Math.Pow(u, 2) + (Math.Pow(w, 2))) - v * (a * u + c * w - u * x - v * y - w * z)) * (1 - Math.Cos(angleradians)) + y * Math.Cos(angleradians) + ( c * u - a * w + w * x - u * z) * Math.Sin(angleradians);
            double Z = (c * (Math.Pow(u, 2) + (Math.Pow(v, 2))) - w * (a * u + b * v - u * x - v * y - w * z)) * (1 - Math.Cos(angleradians)) + z * Math.Cos(angleradians) + (-b * u + a * v - v * x + u * y) * Math.Sin(angleradians);

            return new Point3D(X, Y, Z);

        }
        public static double AngleBetweenVectors(Vector3D V1, Vector3D V2)
        {
            double MV1 = Math.Acos((V1.X * V2.X + V1.Y * V2.Y + V1.Z * V2.Z) / (V1.Length*V2.Length));
            MV1 = MV1 * 180 / Math.PI;
            return MV1; 
        }
        public void GenerateSpotLight(Point3D Postion, Vector3D Direction, double AngleDegrees, Color cl, double Granularity = 1.0, double intensity = 2.0)
        {
            _Color = cl;
            _Granularity = Granularity;
            CreateSphere(5, Granularity, Postion, cl);
            SpotLight sl = new SpotLight(cl, Postion, Direction, AngleDegrees, AngleDegrees);
            LightSource.Content = sl;
            Intensity = intensity;
            GetRayListFromSpotLight(Granularity);

        }
        public void GenerateInfiniteLight(Lense Groundplane, double Theta, double Phi, Color cl, double intensity = 2.0)
        {
            //CreateSphere(5, Granularity, Postion, cl);
            //SpotLight sl = new SpotLight(cl, Postion, Direction, AngleDegrees, AngleDegrees);
            //LightSource.Content = sl;
            _Theta = Theta;
            _Phi = Phi;
            _Color = cl;
            Intensity = intensity;
            GetRayListFromInfiniteLight(Groundplane, Theta, Phi, cl, Intensity);

        }
       
        public static Point3DCollection GenerateOriginLightVectors(double SphereRadius, double Granularity)
        {
            Point3DCollection posss = new Point3DCollection();
            posss.Add(new Point3D(0, SphereRadius, 0));
           // Granularity *= 10.0;
            for (double i = Granularity; i <= 180 - Granularity; i += (Granularity))
            {
                double CR = Math.Sin(i * Math.PI / 180.0) * SphereRadius;
                double Y = Math.Cos(i * Math.PI / 180.0) * SphereRadius;
                double LevelGranularity = Granularity / Math.Sin(i * Math.PI / 180.0);
                for (double k = 0; k <= 360 - Granularity; k += (LevelGranularity))
                {
                    double Z = Math.Sin(k * Math.PI / 180) * CR;
                    double X = Math.Cos(k * Math.PI / 180) * CR;
                    posss.Add(new Point3D(X, Y, Z));
                    //if(posss.Count>= 10)
                    //{
                    //    k = 1000;
                    //    i = 1000;
                    //}
                }
            }
            posss.Add(new Point3D(0, -SphereRadius, 0));
            return posss;
        }

        public static Point3DCollection GenerateSpericlePoints(double SphereRadius, double Granularity)
        {
            Point3DCollection posss = new Point3DCollection();
            posss.Add(new Point3D(0, SphereRadius, 0));
            Granularity *= 10.0;
            for (double i = Granularity; i <= 180 - Granularity; i += (Granularity))
            {
                double CR = Math.Sin(i * Math.PI / 180.0) * SphereRadius;
                double Y = Math.Cos(i * Math.PI / 180.0) * SphereRadius;
                for (double k = 0; k <= 360 - Granularity; k += (Granularity))
                {
                    double Z = Math.Sin(k * Math.PI / 180) * CR;
                    double X = Math.Cos(k * Math.PI / 180) * CR;
                    posss.Add(new Point3D(X, Y, Z));
                    //if(posss.Count>= 10)
                    //{
                    //    k = 1000;
                    //    i = 1000;
                    //}
                }
            }
            posss.Add(new Point3D(0, -SphereRadius, 0));
            return posss;
        }

        public ModelVisual3D CreateSphere(double SphereRadius, double Granularity, Point3D Position, Color cl)
        {
            ModelVisual3D mv3 = new ModelVisual3D();
            GeometryModel3D gm3 = new GeometryModel3D();
            MeshGeometry3D mg3 = new MeshGeometry3D();
            Point3DCollection posss = GenerateSpericlePoints(SphereRadius, Granularity);
            Int32Collection triss = new Int32Collection();
            

            for (int i = 0; i < posss.Count; i++)
            {
                Point3D posit = posss[i];

                posit.X += Position.X;
                posit.Y += Position.Y;
                posit.Z += Position.Z;
                posss[i] = posit;
            }

            mg3.Positions = posss;

            Granularity *= 10;
            int levelcount = (int)(180 / Granularity) - 1;
            int roundmax = (int)(360 / Granularity);
            for (int i = 1; i <= roundmax; i++)
            {
                triss.Add(0);
                int n1 = i;
                int n2 = i + 1;
                if (n1 > roundmax)
                {
                    n1 -= (roundmax);
                }
                if (n2 > roundmax)
                {
                    n2 -= (roundmax);
                }

                triss.Add(n2);
                triss.Add(n1);
            }
            //roundmax++;
            for (int i = 1; i < levelcount; i++)
            {
                for (int k = 0; k < roundmax; k++)
                {
                    int t1b = (i - 1) * roundmax + k + 1;
                    int t2b = (i - 1) * roundmax + k + 2;
                    if (t2b > roundmax * i)
                    {
                        t2b -= (roundmax);
                    }
                    if (t1b > roundmax * i)
                    {
                        t1b -= (roundmax);
                    }

                    int n1 = (i) * roundmax + k + 1;
                    int n2 = (i) * roundmax + k + 2;
                    if (n1 > (roundmax * (i + 1)))
                    {
                        n1 -= (roundmax);
                    }
                    if (n2 > (roundmax * (i + 1)))
                    {
                        n2 -= (roundmax);
                    }
                    triss.Add(t1b);
                    triss.Add(t2b);
                    triss.Add(n1);
                    triss.Add(n2);
                    triss.Add(n1);
                    triss.Add(t2b);

                }
            }

            roundmax = (int)(360 / Granularity);
            for (int i = posss.Count - 1; i >= posss.Count - roundmax; i--)
            {
                triss.Add(posss.Count - 1);
                int n1 = i - 1;
                int n2 = i - 2;
                if (n1 < posss.Count - roundmax - 1)
                {
                    n1 += (roundmax);
                }
                if (n2 < posss.Count - roundmax - 1)
                {
                    n2 += (roundmax);
                }

                triss.Add(n2);
                triss.Add(n1);
            }

            //for (int i = posss.Count - 1; i >= posss.Count - 10; i--)
            //{
            //    triss.Add(posss.Count - 1);
            //    triss.Add((i - 1));
            //    triss.Add((i - 2));
            //}
            mg3.TriangleIndices = triss;

            gm3.Geometry = mg3;


            MaterialGroup MGR = new MaterialGroup();
            DiffuseMaterial dmtr = new DiffuseMaterial();
            dmtr.Brush = new SolidColorBrush(cl);

            SpecularMaterial spmtr = new SpecularMaterial();
            spmtr.Brush = new SolidColorBrush(cl);

            EmissiveMaterial emt = new EmissiveMaterial();
            emt.Brush = new SolidColorBrush(cl);
            MGR.Children.Add(dmtr);
            MGR.Children.Add(spmtr);
            MGR.Children.Add(emt);

            gm3.Material = MGR;

            mv3.Content = gm3;
            Model3D = mv3;
            return Model3D;
            //viewport3D1.Children.Add(mv3);
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
