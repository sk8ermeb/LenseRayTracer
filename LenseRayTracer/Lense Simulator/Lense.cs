using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Lense_Simulator
{
    public class Lense
    {
        public double Width = 20;
        public double Height = 20;
        public double GroundHeight = 10;
        public double HorrizontalPos = 0;
        public double DepthPos = 0;
        public double TopHorRadius = -40;
        public double TopDepthRadius = -40;
        public double BotDepthRadius = 40;
        public double BotHorRadius = 40;
        public double ZHorTilt = 0;
        public double XDepthTilt = 0;
        public double IndexOfRefraction = 2.0;

        /// <summary>
        /// Range from 0 to 255, 0 being transparent and 255 being opaque
        /// </summary>
        public byte Opacity { get; set; } = 255;
        public byte Red { get; set; } = 50;
        public byte Green { get; set; } = 50;
        public byte Blue { get; set; } = 150;

        public double Granularity { get; set; } = 1;
        public string Name {
            get;
            set; } 
            = "Lense";
        public int Rows { get; protected set; } = 0;
        public int Columns { get; protected set; } = 0;
        public List<MaterialPiece> pices { get; set; } = new List<MaterialPiece>();

        public Plane LensePlane { get; set; } = new Plane();
        public void GenerateLense()
        {
            int XNum = (int)(HorrizontalPos + Width / 2);
            int ZNum = (int)(DepthPos + Height / 2);
            int XSt = (int)(HorrizontalPos - Width / 2);
            int ZSt = (int)(DepthPos - Height / 2);
            double XCent = XSt+ Width / 2;
            double ZCent = ZSt + Height / 2;
            Color c = Color.FromArgb(Opacity, Red, Green, Blue);
            Point3D center = new Point3D(XCent, GroundHeight, ZCent);
            Columns = 0;
            for (double k = ZSt; k < ZNum; k += Granularity)
            {
                Columns++;
            }
            Rows = 0;
            for (double i = XSt; i < XNum; i += Granularity)
            {
                Rows++;
            }

            for (double i=XSt;i<XNum;i+=Granularity)
            {
                double XTSlope = CalculateLenseSlopeAtPoint(XCent - i, TopHorRadius);
                double XBSlope = CalculateLenseSlopeAtPoint(XCent - i, BotHorRadius);
                for (double k = ZSt;k< ZNum;k+=Granularity)
                {
                    MaterialPiece mp = new MaterialPiece();
                    mp.OriginalColor = c;
                    mp.Granularity = Granularity;
                    mp.PointInSpace = new Point3D(i, GroundHeight, k);

                    double ZTSlope = CalculateLenseSlopeAtPoint(ZCent - k, TopDepthRadius);
                    double ZBSlope = CalculateLenseSlopeAtPoint(ZCent - k, BotDepthRadius);

                    mp.BotXTilt = XBSlope;
                    mp.BotZTilt = ZBSlope;
                    mp.ToptXTilt = XTSlope;
                    mp.TopZTilt = ZTSlope;
                    mp.IndexOfRefraction = this.IndexOfRefraction;
                    mp.Calculate3DPoints();
                    mp.RotateXZAroundPoint(XDepthTilt, ZHorTilt, center);
                    mp.IndexOfRefraction = this.IndexOfRefraction;
                    mp.Opacity = this.Opacity/255.0;
                    pices.Add(mp);
                }
            }
            LensePlane = GetPlane();

        }
        public Plane GetPlane()
        {

            Plane pl = new Plane();
            pl.Slope_Z = Math.Tan(XDepthTilt * Math.PI / 180);
            pl.Slope_X = Math.Tan(ZHorTilt * Math.PI / 180);
            pl.Off_Y = GroundHeight - (pl.Slope_X * HorrizontalPos) - (pl.Slope_Z * DepthPos);
            pl.GranularitySize = Granularity;
            for(int i=0;i< pices.Count;i++)
            {
                if (pices[i].PointInSpace.X < pl.XMin)
                    pl.XMin = pices[i].PointInSpace.X;
                if (pices[i].PointInSpace.Z < pl.ZMin)
                    pl.ZMin = pices[i].PointInSpace.Z;
                if (pices[i].PointInSpace.X > pl.XMax)
                    pl.XMax = pices[i].PointInSpace.X;
                if (pices[i].PointInSpace.Z > pl.ZMax)
                    pl.ZMax = pices[i].PointInSpace.Z;

            }
            return pl;
        }
        public double CalculateLenseSlopeAtPoint(double X, double R)
        {
            double tmp = Granularity / 10.0;
            double Y1 = Math.Sqrt(Math.Pow(R, 2) - Math.Pow(X, 2));
            double Y2 = Math.Sqrt(Math.Pow(R, 2) - Math.Pow(X + tmp, 2));
            double slope = (Y2 - Y1) / (tmp);
            if (R < 0)
                slope *= -1;
            if (double.IsNaN(slope))
                slope = 0;
            return slope;
        }

        public override string ToString()
        {
            return this.Name;
        }
        private int GetPieceIndexForRowAndColumn(double row, double column)
        {
            int index = (int)Math.Round(row) * Columns + (int)Math.Round(column);
            if (index < 0)
                index = 0;
            if (index >= pices.Count)
                index = pices.Count - 1;
            return index;
        }
        public Ray EncounterLense(Ray ray, Point3D pt)
        {

            double xgran = (Granularity) * Math.Cos(ZHorTilt * Math.PI / 180);
            double zgran = (Granularity) * Math.Cos(XDepthTilt * Math.PI / 180);
            double xoffindex = Math.Round( (pt.X - HorrizontalPos) / xgran);
            double zoffindex = Math.Round((pt.Z - DepthPos) / zgran);

            int pieceindex = GetPieceIndexForRowAndColumn(Rows / 2.0 + xoffindex, Columns / 2.0 + +zoffindex);
            if(pices[pieceindex].isPointInPiece(pt))
            {
                pices[pieceindex].AddLight(ray.RayColor);
                return pices[pieceindex].EncounterPiece(ray);
                
            }
            if (pieceindex < pices.Count - 1 && pices[pieceindex+1].isPointInPiece(pt))
            {
                pices[pieceindex+1].AddLight(ray.RayColor);
                return pices[pieceindex].EncounterPiece(ray);
            }
            if (pieceindex > 0 && pices[pieceindex-1].isPointInPiece(pt))
            {
                pices[pieceindex-1].AddLight(ray.RayColor);
                return pices[pieceindex].EncounterPiece(ray);
            }
            if (pieceindex < pices.Count - Columns && pices[pieceindex+Columns].isPointInPiece(pt))
            {
                pices[pieceindex].AddLight(ray.RayColor);
                return pices[pieceindex].EncounterPiece(ray);
            }
            if (pieceindex > Columns && pices[pieceindex - Columns].isPointInPiece(pt))
            {
                pices[pieceindex].AddLight(ray.RayColor);
                return pices[pieceindex].EncounterPiece(ray);
            }
            for (int i=0;i<pices.Count;i++)
            {
                if(pices[i].isPointInPiece(pt))
                {
                    pices[i].AddLight(ray.RayColor);
                    return pices[pieceindex].EncounterPiece(ray);
                }
            }
            return new Ray();
        }
    }
    public class LensePoint
    {
        public LensePoint()
        { }
        public Lense lense { get; set; }
        public Point3D point { get; set; }
    }
}
