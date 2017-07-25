using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Threading;
namespace Lense_Simulator
{
    public class MaterialPiece
    {
        public Color BaseColor = Color.FromArgb(0,100, 100, 100);
        public Color OriginalColor { set { BaseColor = value; originalcolor = value; } }
        private Color originalcolor = Color.FromArgb(0, 100, 100, 100);
        public int HowLit = 0;
        public Point3D PointInSpace = new Point3D(0, 0, 0);
        public double Granularity = 1;
        public double ToptXTilt = 0;
        public double TopZTilt = 0;
        public double BotXTilt = 0;
        public double BotZTilt = 0;
        private double RotateDepthX = 0;
        private double RotateHorrZ = 0;

        public double IndexOfRefraction { get; set; } = 2;
        public double Opacity = 0;

        //Point3DCollection posss = new Point3DCollection();
        List<Point3D> posss = new List<Point3D>();
        public void Calculate3DPoints()
        {
            posss.Clear();
            double sizetemp = Granularity / 2.0;
            posss.Add(new Point3D(PointInSpace.X - sizetemp, PointInSpace.Y - sizetemp, PointInSpace.Z - sizetemp));
            posss.Add(new Point3D(PointInSpace.X + sizetemp, PointInSpace.Y - sizetemp, PointInSpace.Z - sizetemp));
            posss.Add(new Point3D(PointInSpace.X + sizetemp, PointInSpace.Y - sizetemp, PointInSpace.Z + sizetemp));
            posss.Add(new Point3D(PointInSpace.X - sizetemp, PointInSpace.Y - sizetemp, PointInSpace.Z + sizetemp));
            posss.Add(new Point3D(PointInSpace.X - sizetemp, PointInSpace.Y + sizetemp, PointInSpace.Z - sizetemp));
            posss.Add(new Point3D(PointInSpace.X + sizetemp, PointInSpace.Y + sizetemp, PointInSpace.Z - sizetemp));
            posss.Add(new Point3D(PointInSpace.X + sizetemp, PointInSpace.Y + sizetemp, PointInSpace.Z + sizetemp));
            posss.Add(new Point3D(PointInSpace.X - sizetemp, PointInSpace.Y + sizetemp, PointInSpace.Z + sizetemp));
            Point3D p0 = posss[0];
            Point3D p1 = posss[1];
            Point3D p2 = posss[2];
            Point3D p3 = posss[3];
            Point3D p4 = posss[4];
            Point3D p5 = posss[5];
            Point3D p6 = posss[6];
            Point3D p7 = posss[7];
    
            TiltPointsAroundAxis(ref p0, ref p1, ref p2, ref p3, PointInSpace, Axis.X, BotZTilt);
            TiltPointsAroundAxis(ref p0, ref p1, ref p2, ref p3, PointInSpace, Axis.Z, BotXTilt);
            TiltPointsAroundAxis(ref p4, ref p5, ref p6, ref p7, PointInSpace, Axis.X, TopZTilt);
            TiltPointsAroundAxis(ref p4, ref p5, ref p6, ref p7, PointInSpace, Axis.Z, ToptXTilt);
            posss[0] = p0;
            posss[1] = p1;
            posss[2] = p2;
            posss[3] = p3;
            posss[4] = p4;
            posss[5] = p5;
            posss[6] = p6;
            posss[7] = p7;

        }
        public enum Axis { X, Y, Z}
        public void TiltPointsAroundAxis(ref Point3D P1, ref Point3D P2, ref Point3D P3, ref Point3D P4, Point3D RefPoint, Axis ax, double tilt)
        {
            if(ax == Axis.X)
            {
                double PZDiff = P1.Z - RefPoint.Z;
                P1.Y += tilt * PZDiff;
                PZDiff = P2.Z - RefPoint.Z;
                P2.Y += tilt * PZDiff;
                PZDiff = P3.Z - RefPoint.Z;
                P3.Y += tilt * PZDiff;
                PZDiff = P4.Z - RefPoint.Z;
                P4.Y += tilt * PZDiff;
            }
            if (ax == Axis.Z)
            {
                double PXDiff = P1.X - RefPoint.X;
                P1.Y += tilt * PXDiff;
                PXDiff = P2.X - RefPoint.X;
                P2.Y += tilt * PXDiff;
                PXDiff = P3.X - RefPoint.X;
                P3.Y += tilt * PXDiff;
                PXDiff = P4.X - RefPoint.X;
                P4.Y += tilt * PXDiff;
            }
        }
        public ModelVisual3D Generated3DPiece { get; set; } = null;
        DiffuseMaterial MainMaterialColor = new DiffuseMaterial();
        public ModelVisual3D Generate3DMaterialPiece(bool IncludeReflection = true)
        {
            ModelVisual3D mv3 = new ModelVisual3D();
            GeometryModel3D gm3 = new GeometryModel3D();
            MeshGeometry3D mg3 = new MeshGeometry3D();
            //Calculate3DPoints();
            //posss.Clear();
            //double sizetemp = Granularity / 2.0;
            //posss.Add(new Point3D(PointInSpace.X - sizetemp, PointInSpace.Y - sizetemp, PointInSpace.Z - sizetemp));
            //posss.Add(new Point3D(PointInSpace.X + sizetemp, PointInSpace.Y - sizetemp, PointInSpace.Z - sizetemp));
            //posss.Add(new Point3D(PointInSpace.X + sizetemp, PointInSpace.Y - sizetemp, PointInSpace.Z + sizetemp));
            //posss.Add(new Point3D(PointInSpace.X - sizetemp, PointInSpace.Y - sizetemp, PointInSpace.Z + sizetemp));
            //posss.Add(new Point3D(PointInSpace.X - sizetemp, PointInSpace.Y + sizetemp, PointInSpace.Z - sizetemp));
            //posss.Add(new Point3D(PointInSpace.X + sizetemp, PointInSpace.Y + sizetemp, PointInSpace.Z - sizetemp));
            //posss.Add(new Point3D(PointInSpace.X + sizetemp, PointInSpace.Y + sizetemp, PointInSpace.Z + sizetemp));
            //posss.Add(new Point3D(PointInSpace.X - sizetemp, PointInSpace.Y + sizetemp, PointInSpace.Z + sizetemp));
            Point3DCollection p33 = new Point3DCollection(posss);
            mg3.Positions = p33;
            Int32Collection triss = new Int32Collection(new int[] { 2, 0, 1, 0, 2, 3, 2, 1, 6, 5, 6, 1, 6, 7, 2, 3, 2, 7, 7, 6, 5, 7, 5, 4, 0, 4, 1, 5, 1, 4, 3, 7, 0, 4, 0, 7 });
            mg3.TriangleIndices = triss;

            gm3.Geometry = mg3;


            MaterialGroup MGR = new MaterialGroup();
            MainMaterialColor = new DiffuseMaterial();
            int red = BaseColor.R;
            int green = BaseColor.G;
            int blue = BaseColor.B;
            red += HowLit * 5;
            if (red > 255)
                red = 255;
            green += HowLit * 5;
            if (green > 255)
                green = 255;
            blue += HowLit * 5;
            if (blue > 255)
                blue = 255;
            //Color newscb = Color.FromArgb(BaseColor.A, (byte)red, (byte)green, (byte)blue);
            BaseColor = Color.FromArgb(BaseColor.A, (byte)red, (byte)green, (byte)blue);
            MainMaterialColor.Brush = new SolidColorBrush(BaseColor);
            if (IncludeReflection)
            {
                SpecularMaterial spmtr = new SpecularMaterial();
                spmtr.Brush = Brushes.White;
                MGR.Children.Add(spmtr);
            }
            MGR.Children.Add(MainMaterialColor);
            gm3.Material = MGR;
            mv3.Content = gm3;
            Generated3DPiece = mv3;
            return mv3;
        }


        /// <summary>
        /// Tilts are in degrees
        /// </summary>
        /// <param name="XTilt">Around the Z axis in degrees</param>
        /// <param name="ZTilt">Around the X axis in degrees</param>
        /// <param name="P3D"></param>
        public void RotateXZAroundPoint(double XTilt, double ZTilt, Point3D P3D)
        {
            
            double DX = PointInSpace.X - P3D.X;
            double DY = PointInSpace.Y - P3D.Y;
            double mag = Math.Sqrt(Math.Pow(DX, 2) + Math.Pow(DY, 2));
            double theta = Math.Atan2(DY, DX)*180/Math.PI;
            theta += ZTilt;
            theta = theta * Math.PI / 180;
            DY = mag * Math.Sin(theta);
            DX = mag * Math.Cos(theta);
            PointInSpace.X = P3D.X + DX;
            PointInSpace.Y = P3D.Y + DY;

            double DZ = PointInSpace.Z - P3D.Z;
            DY = PointInSpace.Y - P3D.Y;
            mag = Math.Sqrt(Math.Pow(DZ, 2) + Math.Pow(DY, 2));
            theta = Math.Atan2(DY, DZ) * 180 / Math.PI;
            theta += XTilt;
            theta = theta * Math.PI / 180;
            DY = mag * Math.Sin(theta);
            DZ = mag * Math.Cos(theta);
            PointInSpace.Z = P3D.Z + DZ;
            PointInSpace.Y = P3D.Y + DY;

            for (int i = 0; i < posss.Count; i++)
            {
                Point3D temp = new Point3D(posss[i].X, posss[i].Y, posss[i].Z);
                DX = temp.X - P3D.X;
                DY = temp.Y - P3D.Y;
                mag = Math.Sqrt(Math.Pow(DX, 2) + Math.Pow(DY, 2));
                theta = Math.Atan2(DY, DX) * 180 / Math.PI;
                theta += ZTilt;
                theta = theta * Math.PI / 180;
                DY = mag * Math.Sin(theta);
                DX = mag * Math.Cos(theta);
                temp.X = P3D.X + DX;
                temp.Y = P3D.Y + DY;

                DZ = temp.Z - P3D.Z;
                DY = temp.Y - P3D.Y;
                mag = Math.Sqrt(Math.Pow(DZ, 2) + Math.Pow(DY, 2));
                theta = Math.Atan2(DY, DZ) * 180 / Math.PI;
                theta += XTilt;
                theta = theta * Math.PI / 180;
                DY = mag * Math.Sin(theta);
                DZ = mag * Math.Cos(theta);
                temp.Z = P3D.Z + DZ;
                temp.Y = P3D.Y + DY;
                posss[i] = temp;
            }
            RotateDepthX = XTilt;
            RotateHorrZ = ZTilt;

        }
        public void updaterendering()
        {
            if(MainWindow.MainWindowHandle.Dispatcher.Thread != Thread.CurrentThread)
            {
                MainWindow.MainWindowHandle.Dispatcher.Invoke(updaterendering);
                return;
            }
            MainMaterialColor.Brush = new SolidColorBrush(BaseColor);

        }
        public void ResetLight()
        {
            BaseColor = originalcolor;
            updaterendering();
        }
        public void AddLight(Color cr)
        {
            //GeometryModel3D gm3 =  Generated3DPiece.Content as GeometryModel3D;
            //MaterialGroup mgr = gm3.Material as MaterialGroup;
            //DiffuseMaterial dm =  mgr.Children[1] as DiffuseMaterial;
            //SolidColorBrush br  = dm.Brush as SolidColorBrush;
           // Color crr = br.Color;
            BaseColor = Color.Add(cr, BaseColor);
            //Color ncolor = Color.FromRgb((byte)(crr.R + cr.R), (byte)(crr.G + cr.G), (byte)(crr.B + cr.B));
            if(MainWindow.UpdateRealTime)
            {
                updaterendering();
            }
            
        }
        public Ray EncounterPiece (Ray EncounterRay)
        {
            //return new Ray();
            Vector3D V = new Vector3D(posss[5].X - posss[6].X, posss[5].Y - posss[6].Y, posss[5].Z - posss[6].Z);
            Vector3D U = new Vector3D(posss[5].X - posss[7].X, posss[5].Y - posss[7].Y, posss[5].Z - posss[7].Z);
            Vector3D Norm = Light.VectorCrossProduct(U, V);
            Norm.Normalize();
            Vector3D rayvector = new Vector3D(EncounterRay.Direction.X, EncounterRay.Direction.Y, EncounterRay.Direction.Z);
            rayvector.Normalize();
            rayvector.Negate();
            double angle = Light.AngleBetweenVectors(Norm, rayvector);
            angle = angle * Math.PI / 180;
            double theta2 = Math.Asin(Math.Sin(angle) / IndexOfRefraction);
            //double angle2 = theta2 * 180 / Math.PI;
            Vector3D RotationLine = Light.VectorCrossProduct(Norm, rayvector);
            RotationLine.Normalize();
            Norm.Negate();

            Point3D PtToRotate = new Point3D(Norm.X, Norm.Y, Norm.Z);

            Point3D p3d =  Light.RotatePointAroundAribtraryLine(PtToRotate, new Point3D(0, 0, 0), RotationLine, theta2);

            Vector3D MAterialRayDirection = new Vector3D(p3d.X, p3d.Y, p3d.Z);


            Vector3D V2 = new Vector3D(posss[1].X - posss[2].X, posss[1].Y - posss[2].Y, posss[1].Z - posss[2].Z);
            Vector3D U2 = new Vector3D(posss[1].X - posss[3].X, posss[1].Y - posss[3].Y, posss[1].Z - posss[3].Z);
            Vector3D Norm2 = Light.VectorCrossProduct(U2, V2);
            Norm2.Normalize();
            MAterialRayDirection.Normalize();
            //Vector3D rayvector = new Vector3D(EncounterRay.Direction.X, EncounterRay.Direction.Y, EncounterRay.Direction.Z);
            MAterialRayDirection.Negate();
            angle = Light.AngleBetweenVectors(Norm2, MAterialRayDirection);
            angle = angle * Math.PI / 180;
            double theta3 = Math.Asin(Math.Sin(angle) * IndexOfRefraction);

            Vector3D RotationLine2 = Light.VectorCrossProduct(Norm2, MAterialRayDirection);
            
            Norm2.Negate();
            RotationLine2.Normalize();
            Point3D PtToRotate2 = new Point3D(Norm2.X, Norm2.Y, Norm2.Z);

            Point3D p3d2 = Light.RotatePointAroundAribtraryLine(PtToRotate2, new Point3D(0, 0, 0), RotationLine2, theta3);

            Vector3D OutgoingRay = new Vector3D(p3d2.X, p3d2.Y, p3d2.Z);
            
            Ray ray2 = new Ray();
            ray2.Direction = OutgoingRay;
            ray2.Origin = PointInSpace;
            ray2.RayColor = EncounterRay.RayColor;
            ray2.TraceCount = EncounterRay.TraceCount + 1;
            //double test = Light.AngleBetweenVectors(Norm2, OutgoingRay);
            return ray2;

        }
        public bool isPointInPiece(Point3D pt)
        {
            double xgran = (Granularity / 2) * Math.Cos(RotateHorrZ * Math.PI / 180);
            double zgran = (Granularity / 2) * Math.Cos(RotateDepthX * Math.PI / 180);
            return (pt.X >= PointInSpace.X - xgran &&
                pt.X <= PointInSpace.X + xgran &&
                pt.Z >= PointInSpace.Z - zgran &&
                pt.Z <= PointInSpace.Z + zgran);
        }

    }
}
