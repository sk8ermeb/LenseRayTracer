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
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Threading;

namespace Lense_Simulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Lense> AddedLenses = new List<Lense>();
        List<Light> AddedLights = new List<Light>();
        public static MainWindow MainWindowHandle;
        Light NewInfiniteLight = new Light();
        public static bool UpdateRealTime { get; set; } = false;
        public MainWindow()
        {
            InitializeComponent();  
            this.Loaded += MainWindow_Loaded;
            GenerateExampleNewLight();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindowHandle = this;
            SetSimulatingMenu(false);
        }

        ModelVisual3D gndmodel = new ModelVisual3D();
        
        
        public void CreateBox()
        {
            ModelVisual3D mv3 = new ModelVisual3D();
            GeometryModel3D gm3 = new GeometryModel3D();
            MeshGeometry3D mg3 = new MeshGeometry3D();
            Point3DCollection posss = new Point3DCollection();
            posss.Add(new Point3D(-5, 10, -5));
            posss.Add(new Point3D(5, 10, -5));
            posss.Add(new Point3D(5, 10, 5));
            posss.Add(new Point3D(-5, 10, 5));
            posss.Add(new Point3D(-5, 20, -5));
            posss.Add(new Point3D(5, 20, -5));
            posss.Add(new Point3D(5, 20, 5));
            posss.Add(new Point3D(-5, 20, 5));
            mg3.Positions = posss;
            Int32Collection triss = new Int32Collection(new int[] { 2, 0, 1, 0, 2, 3, 2, 1, 6, 5, 6, 1, 6, 7, 2, 3, 2, 7, 7, 6, 5, 7, 5, 4, 0, 4, 1, 5, 1, 4, 3, 7, 0, 4, 0, 7 });           
            mg3.TriangleIndices = triss;

            gm3.Geometry = mg3;


            MaterialGroup MGR = new MaterialGroup();
            DiffuseMaterial dmtr = new DiffuseMaterial();
            dmtr.Brush = Brushes.Green;

            SpecularMaterial spmtr = new SpecularMaterial();
            spmtr.Brush = Brushes.Wheat;

            MGR.Children.Add(dmtr);
            MGR.Children.Add(spmtr);
            

            gm3.Material = MGR;

            mv3.Content = gm3;

            viewport3D1.Children.Add(mv3);

        }
        public static double RadToDeg(double rad)
        {
            return rad *  180 / Math.PI;
        }
        public static double DegToRad(double degree)
        {
            return degree * Math.PI / 180;
        }

        double MouseX, MouseY;
        private Point3D LookAt = new Point3D(0, 0, 0);
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseactivated)
                return;
            if (!ShiftPressed)
            {
                LookAt.X = camMain.Position.X + camMain.LookDirection.X;
                LookAt.Y = camMain.Position.Y + camMain.LookDirection.Y;
                LookAt.Z = camMain.Position.Z + camMain.LookDirection.Z;

                double MouseTX = e.GetPosition(MainGrid).X;
                double MouseTY = e.GetPosition(MainGrid).Y;
                double mag = Math.Sqrt(Math.Pow(camMain.LookDirection.Z, 2) + Math.Pow(camMain.LookDirection.X, 2) + Math.Pow(camMain.LookDirection.Y, 2));

                double theta = RadToDeg(Math.Acos(-camMain.LookDirection.Y / mag));
                double phi = RadToDeg(Math.Atan2(-camMain.LookDirection.X, -camMain.LookDirection.Z));

                phi -= (MouseTX - MouseX);
                theta -= (MouseTY - MouseY);

                Point3D p3dn = new Point3D();
                double newz = mag * Math.Sin(DegToRad(theta)) * Math.Cos(DegToRad(phi));
                double newx = mag * Math.Sin(DegToRad(theta)) * Math.Sin(DegToRad(phi));
                double newy = Math.Cos(DegToRad(theta)) * mag;

                p3dn.X = newx;
                p3dn.Y = newy;
                p3dn.Z = newz;
                camMain.Position = new Point3D(newx + LookAt.X, newy + LookAt.Y, newz + LookAt.Z);
                camMain.LookDirection = new Vector3D(-p3dn.X, -p3dn.Y, -p3dn.Z);

                MouseX = MouseTX;
                MouseY = MouseTY;
            }
            else
            {
                double MouseTX = e.GetPosition(MainGrid).X;
                double MouseTY = e.GetPosition(MainGrid).Y;

                double dz = camMain.LookDirection.Z;
                double dx = camMain.LookDirection.X;
                double dy = camMain.LookDirection.Y;
                double dirmag = Math.Sqrt(dx * dx + dz * dz +dy*dy);
                double phi = Math.Acos(dy / dirmag);
                double theta = Math.Atan2(-dx, dz);
                double CX = Math.Cos(theta) * Math.Sin(phi);
                double CZ = Math.Sin(theta) * Math.Sin(phi);
                CX *= (MouseTX - MouseX);
                CZ *= (MouseTX - MouseX);
                double CY = Math.Sin(phi);
                CY *= (MouseTY - MouseY);

                double distance = Math.Sqrt(Math.Pow(camMain.Position.X, 1) + Math.Pow(camMain.Position.Y, 2) + Math.Pow(camMain.Position.Z, 2));
                CX *= distance;
                CY *= distance;
                CZ *= distance;
                CX /= 1000.0;
                CY /= 1000.0;
                CZ /= 1000.0;
               
                camMain.Position = new Point3D(camMain.Position.X + CX, camMain.Position.Y + CY, camMain.Position.Z + CZ);
                MouseX = MouseTX;
                MouseY = MouseTY;

            }
        }
        bool mouseactivated = false;
        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseactivated = false;
        }

        private void MainGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseactivated = false;
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            LookAt.X = camMain.Position.X + camMain.LookDirection.X;
            LookAt.Y = camMain.Position.Y + camMain.LookDirection.Y;
            LookAt.Z = camMain.Position.Z + camMain.LookDirection.Z;
            double mag = Math.Sqrt(Math.Pow(camMain.Position.Z, 2) + Math.Pow(camMain.Position.X, 2) + Math.Pow(camMain.Position.Y, 2));

            Point3D pos = new Point3D(camMain.Position.X, camMain.Position.Y, camMain.Position.Z);
            Vector3D dir = new Vector3D(camMain.LookDirection.X, camMain.LookDirection.Y, camMain.LookDirection.Z);


            //double theta = (Math.Acos(camMain.Position.Y / mag));
            //double phi = (Math.Atan2(camMain.Position.X, camMain.Position.Z));

            double change = e.Delta * 1.0;

            double xd = camMain.LookDirection.X / change;
            double yd = camMain.LookDirection.Y / change;
            double zd = camMain.LookDirection.Z / change;
            pos.X += xd;
            pos.Y += yd;
            pos.Z += zd;

            dir.X -= xd;
            dir.Y -= yd;
            dir.Z -= zd;

            //Point3D p3dn = new Point3D();
            //double newz = mag * Math.Sin((theta)) * Math.Cos((phi));
            //double newx = mag * Math.Sin((theta)) * Math.Sin((phi));
            //double newy = Math.Cos((theta)) * mag;

            //p3dn.X = newx;
            //p3dn.Y = newy;
            //p3dn.Z = newz;



            camMain.Position = pos;
            camMain.LookDirection = dir;
        }

        private void menuAdd_Click(object sender, RoutedEventArgs e)
        {
            LenseListGroup.Items.Clear();
            for (int i=0; i< AddedLenses.Count;i++)
            {
                LenseListGroup.Items.Add(AddedLenses[i]);
            }
            NewLense.Name = "New Lense";
            LenseListGroup.Items.Add(NewLense);
            LenseListGroup.SelectedItem = NewLense;
            AddLenseScreen.Visibility = Visibility.Visible;
        }
        int lensenamecounter = 0;
        private void AddLense_Click(object sender, RoutedEventArgs e)
        {
            
            Lense li = LenseListGroup.SelectedItem as Lense;
            string name = "";
            Lense Lense1 = new Lense();
            if (GroundLense != null && li == GroundLense)
            {
                MessageBox.Show("Cannot change ground plance lense!");
                return;
            }
            else
            {

                //Lense Lense1 = new Lense();
                Lense1.Granularity = 1;
                double val = 0;
                if (double.TryParse(LenseWidth.Text, out val))
                    Lense1.Width = val;
                else
                {
                    MessageBox.Show("Failed to parse Lense Width");
                    return;
                }
                if (double.TryParse(LenseHeight.Text, out val))
                    Lense1.Height = val;
                else
                {
                    MessageBox.Show("Failed to parse Lense Height");
                    return;
                }
                if (double.TryParse(HeightAboveGround.Text, out val))
                    Lense1.GroundHeight = val;
                else
                {
                    MessageBox.Show("Failed to parse Height Above Ground");
                    return;
                }
                if (double.TryParse(HorrPoss.Text, out val))
                    Lense1.HorrizontalPos = val;
                else
                {
                    MessageBox.Show("Failed to parse horizontal position");
                    return;
                }
                if (double.TryParse(DepthPoss.Text, out val))
                    Lense1.DepthPos = val;
                else
                {
                    MessageBox.Show("Failed to parse depth position");
                    return;
                }
                if (double.TryParse(RotationX.Text, out val))
                    Lense1.XDepthTilt = val;
                else
                {
                    MessageBox.Show("Failed to parse Rotation around X axis");
                    return;
                }
                if (double.TryParse(RotationsZ.Text, out val))
                    Lense1.ZHorTilt = val;
                else
                {
                    MessageBox.Show("Failed to parse Rotation around Z axis");
                    return;
                }
                if (double.TryParse(TopHorrRadius.Text, out val))
                    Lense1.TopHorRadius = -val;
                else
                {
                    MessageBox.Show("Failed to parse Top Horizontal Radius");
                    return;
                }
                if (double.TryParse(TopDepthRadius.Text, out val))
                    Lense1.TopDepthRadius = -val;
                else
                {
                    MessageBox.Show("Failed to parse Top Depth Radius");
                    return;
                }
                if (double.TryParse(BottDepthRadius.Text, out val))
                    Lense1.BotDepthRadius = val;
                else
                {
                    MessageBox.Show("Failed to parse Bottom Depth Radius");
                    return;
                }
                if (double.TryParse(BottHorrRadius.Text, out val))
                    Lense1.BotHorRadius = val;
                else
                {
                    MessageBox.Show("Failed to parse Bottom Horrizontal Radius");
                    return;
                }
                if (double.TryParse(Granularity.Text, out val))
                    Lense1.Granularity = val;
                else
                {
                    MessageBox.Show("Failed to parse Bottom Horrizontal Radius");
                    return;
                }
                if (double.TryParse(IndexOfRefraction.Text, out val))
                    Lense1.IndexOfRefraction = val;
                else
                {
                    MessageBox.Show("Failed to parse index of refraction");
                    return;
                }



                if (li == NewLense)
                {
                    Lense1.Name = "Lense " + lensenamecounter.ToString();
                    lensenamecounter++;
                }
                else
                {

                    for (int i = 0; i < li.pices.Count; i++)
                    {
                        if (li.pices[i].Generated3DPiece != null)
                            viewport3D1.Children.Remove(li.pices[i].Generated3DPiece);
                    }
                    AddedLenses.Remove(li);
                    LenseListGroup.Items.Remove(li);
                    Lense1.Name = li.Name;
                }
            }
           


           
            
            //Lense1.Granularity = 0.5;
            Lense1.GenerateLense();
            for(int i=0;i< Lense1.pices.Count;i++)
            {
                viewport3D1.Children.Add(Lense1.pices[i].Generate3DMaterialPiece());
            }
            
            AddedLenses.Add(Lense1);
            LenseListGroup.Items.Add(Lense1);
            LenseListGroup.SelectedItem = Lense1;
            //CreateBox();
            //AddLenseScreen.Visibility = Visibility.Hidden;
        }
       
        private void CancelLense_Click(object sender, RoutedEventArgs e)
        {
            AddLenseScreen.Visibility = Visibility.Hidden;
        }
        Lense NewLense = new Lense();
       

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Lense li = LenseListGroup.SelectedItem as Lense;
            if(li == null)
            {
                MessageBox.Show("Must select lense from the list to remove!");
                return;
            }
            else if(li == NewLense)
            {
                MessageBox.Show("Cannot Remove \"New Lense\"");
                return;
            }
            else if(li == GroundLense)
            {
                for (int i = 0; i < li.pices.Count; i++)
                {
                    if (li.pices[i].Generated3DPiece != null)
                        viewport3D1.Children.Remove(li.pices[i].Generated3DPiece);
                }
                AddedLenses.Remove(li);
                LenseListGroup.Items.Remove(li);
            }
            else
            {
                for(int i=0;i<li.pices.Count;i++)
                {
                    if (li.pices[i].Generated3DPiece != null)
                        viewport3D1.Children.Remove(li.pices[i].Generated3DPiece);
                }
                AddedLenses.Remove(li);
                LenseListGroup.Items.Remove(li);
            }
        }

       

       

        

        private void RemoveLightButton_Click(object sender, RoutedEventArgs e)
        {
            Light li = LightListGroup.SelectedItem as Light;
            if(li == null)
            {
                MessageBox.Show("Must Select light to delete");
                return;
            }
            if(li == NewInfiniteLight || li == NewSpotLight)
            {
                MessageBox.Show("Cannot Delete \"New Lights\"");
                return;
            }
            viewport3D1.Children.Remove(li.Model3D);
            //viewport3D1.Children.Remove(li.LightSource);
            LightListGroup.Items.Remove(li);
            AddedLights.Remove(li);
        }

       
        private void menuGndPlane_Click(object sender, RoutedEventArgs e)
        {
            GndPlaneMenu.Visibility = Visibility.Visible;
        }
        Lense GroundLense;
        private void GndSave_Click(object sender, RoutedEventArgs e)
        {
            //double val = 0;
            double gndwidth = 0;
            if(!double.TryParse(GndWidth.Text, out gndwidth))
            {
                MessageBox.Show("Failed to parse Ground Width!");
                return;
            }
            double gnddepth = 0;
            if (!double.TryParse(GndDepth.Text, out gnddepth))
            {
                MessageBox.Show("Failed to parse Ground Depth!");
                return;
            }
            double gndgranularity = 0;
            if (!double.TryParse(GndGranularity.Text, out gndgranularity))
            {
                MessageBox.Show("Failed to parse Ground Granularity!");
                return;
            }
            if(GroundLense != null)
            {
                for (int i = 0; i < GroundLense.pices.Count; i++)
                {
                    if (GroundLense.pices[i].Generated3DPiece != null)
                        Ground.Children.Remove(GroundLense.pices[i].Generated3DPiece);
                }
                AddedLenses.Remove(GroundLense);
                LenseListGroup.Items.Remove(GroundLense);
                
            }
            
            Ground.Children.Clear();
            GroundLense = new Lense();
            GroundLense.Blue = 100;
            GroundLense.Red = 100;
            GroundLense.Green = 100;
            GroundLense.BotDepthRadius = double.MaxValue;
            GroundLense.BotHorRadius = double.MaxValue;
            GroundLense.TopDepthRadius = double.MaxValue;
            GroundLense.TopHorRadius = double.MaxValue;
            GroundLense.GroundHeight = 0;
            GroundLense.DepthPos = 0;
            GroundLense.Height = gnddepth;
            GroundLense.Width = gndwidth;
            GroundLense.XDepthTilt = 0;
            GroundLense.ZHorTilt = 0;
            GroundLense.Name = "Ground";
            GroundLense.IndexOfRefraction = double.MaxValue;
            GroundLense.Granularity = gndgranularity;
            GroundLense.GenerateLense();
            for(int i=0;i<GroundLense.pices.Count;i++)
            {
                Ground.Children.Add(GroundLense.pices[i].Generate3DMaterialPiece(false));
            }

            GndPlaneMenu.Visibility = Visibility.Hidden;
            AddedLenses.Add(GroundLense);
        }

        private void GndCancel_Click(object sender, RoutedEventArgs e)
        {
            GndPlaneMenu.Visibility = Visibility.Hidden;
        }

        //private void buttonAddLight_Click(object sender, RoutedEventArgs e)
        //{
            
        //}

        private void buttonCancelAddLight_Click(object sender, RoutedEventArgs e)
        {
            AddLightPanel.Visibility = Visibility.Hidden;
        }
        private void SetSimulatingMenu(bool simulating)
        {
            if(MainWindowHandle.Dispatcher.Thread != Thread.CurrentThread)
            {
                MainWindowHandle.Dispatcher.Invoke(()=>
                {
                    SetSimulatingMenu(simulating);
                    
                });
                return;
            }
            if(simulating)
            {
                simulationmanu.IsEnabled = false;
                menuLense.IsEnabled = false;
                menuLight.IsEnabled = false;
                menuGndPlane.IsEnabled = false;
                menuRunRayTrace.IsEnabled = false;
                StopRayTrace.Visibility = Visibility.Visible;
                StopRayTrace.IsEnabled = true;
            }
            else
            {
                simulationmanu.IsEnabled = true;
                menuLense.IsEnabled = true;
                menuLight.IsEnabled = true;
                menuGndPlane.IsEnabled = true;
                menuRunRayTrace.IsEnabled = true;
                StopRayTrace.Visibility = Visibility.Hidden;
                StopRayTrace.IsEnabled = false;
                
            }
        }

        private void TraceRay(Ray ray)
        {
            if (ct.IsCancellationRequested)
            {
                SetSimulatingMenu(false);
                return;
            }
            List<LensePoint> PossibleEncounters = new List<LensePoint>();
            for (int n = 0; n < AddedLenses.Count; n++)
            {
                if (ct.IsCancellationRequested)
                {
                    SetSimulatingMenu(false);
                    return;
                }
                
                Point3D inter = ray.GetRayPlaneIntersect(AddedLenses[n].LensePlane);
                if (!Ray.isPoint3DBad(inter))
                {
                    if (ray.OriginObj == AddedLenses[n])
                    {
                        //inter = ray.GetRayPlaneIntersect(AddedLenses[n].LensePlane);

                    }

                    else
                        PossibleEncounters.Add(new LensePoint() { lense = AddedLenses[n], point = inter });
                }
                //inter = ray.GetRayPlaneIntersect(AddedLenses[n].LensePlane);
            }
            double EncounterDistance = double.MaxValue;
            int index = -1;
            for(int i=0;i< PossibleEncounters.Count;i++)
            {
                double distance = Math.Sqrt(Math.Pow(PossibleEncounters[i].point.X - ray.Origin.X, 2) +
                    Math.Pow(PossibleEncounters[i].point.Y - ray.Origin.Y, 2) +
                    Math.Pow(PossibleEncounters[i].point.Z - ray.Origin.Z, 2));
                if (distance < EncounterDistance)
                {
                    index = i;
                    EncounterDistance = distance;
                }

            }
            if(index!=-1)
            {
                Ray newray = PossibleEncounters[index].lense.EncounterLense(ray, PossibleEncounters[index].point);
                newray.OriginObj = PossibleEncounters[index].lense;
                if (newray.HasBeenSet() && newray.TraceCount < 20)
                {
                    TraceRay(newray);
                }

            }
        }
        CancellationTokenSource ct;
        Thread SimulationThread;
        private void menuRunRayTrace_Click(object sender, RoutedEventArgs e)
        {
            
            if(SimulationThread != null && SimulationThread.ThreadState == ThreadState.Running)
            {
                ct.Cancel();
                MessageBox.Show("Previous simulation was still running. Trying to stop cancel last simulation. Try running again.");
                return;
            }
            
            ct = new CancellationTokenSource();

            SimulationThread = new Thread(() =>
            {
                SetSimulatingMenu(true);
                
                for (int i = 0; i < AddedLights.Count; i++)
                {
                    List<Ray> lightrays = AddedLights[i].Rays;
                    for (int k = 0; k < lightrays.Count; k++)
                    {
                        TraceRay(lightrays[k]);
                    }
                }
                SetSimulatingMenu(false);
                if(!UpdateRealTime)
                {
                    for(int i=0;i<AddedLenses.Count;i++)
                    {
                        for(int k =0;k<AddedLenses[i].pices.Count;k++)
                        {
                            AddedLenses[i].pices[k].updaterendering();
                        }
                    }
                }
                MainWindowHandle.Dispatcher.Invoke(()=>
                {
                    MessageBox.Show("All Done!");
                });
            });
            SimulationThread.Name = "Simulation Thread";
            SimulationThread.Start();
            
        }

        private void StopRayTrace_Click(object sender, RoutedEventArgs e)
        {
            if (ct != null)
                ct.Cancel();
        }

        private void manuAddLight_Click(object sender, RoutedEventArgs e)
        {
            LightListGroup.Items.Clear();
            for (int i = 0; i < AddedLights.Count; i++)
            {
                LightListGroup.Items.Add(AddedLights[i]);
            }
            LightListGroup.Items.Add(NewInfiniteLight);
            LightListGroup.Items.Add(NewSpotLight);
            LightListGroup.SelectedItem = NewSpotLight;
            AddLightPanel.Visibility = Visibility.Visible;

        }

        private void menuRemoveLight_Click(object sender, RoutedEventArgs e)
        {
            LightListGroup.Items.Clear();
            for (int i = 0; i < AddedLights.Count; i++)
            {
                LightListGroup.Items.Add(AddedLights[i]);
            }
            LightListGroup.Items.Add(NewInfiniteLight);
            LightListGroup.Items.Add(NewSpotLight);
            LightListGroup.SelectedItem = NewSpotLight;
            //LightList.Visibility = Visibility.Visible;
        }

        private void menuResetSimulation_Click(object sender, RoutedEventArgs e)
        {
            for(int i=0;i<AddedLenses.Count;i++)
            {
                for(int n=0;n<AddedLenses[i].pices.Count;n++)
                {
                    AddedLenses[i].pices[n].ResetLight();
                }
            }
        }
        private bool ShiftPressed = false;
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                ShiftPressed = true;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                ShiftPressed = false;
            }
        }
        int lightcounter = 0;
        private void buttonInfiniteAdd_Click(object sender, RoutedEventArgs e)
        {
            string name = "";
            bool newlight = false;
            Light sli = LightListGroup.SelectedItem as Light;
            if (sli == NewInfiniteLight)
            {
                name = "Light " + lightcounter.ToString();
                lightcounter++;
                newlight = true;
            }
            else if(sli == NewSpotLight)
            {
                name = "Light " + lightcounter.ToString();
                lightcounter++;
                newlight = true;
            }
            else
            {
                name = sli.Name;
            }
            if (LightTabControl.SelectedItem == InfiniteTab)
            {
                if (GroundLense == null || GroundLense.pices == null || GroundLense.pices.Count < 1)
                {
                    MessageBox.Show("Must have a ground plan in first for an infinite light source. Ray tracing is strictly dependant on ground plane size and granularity");
                    return;
                }
                double theta = 0;
                if (!double.TryParse(InfiniteTheta.Text, out theta))
                {
                    MessageBox.Show("Failed to parse Theta");
                    return;
                }
                double phi = 0;
                if (!double.TryParse(InfinitePhi.Text, out phi))
                {
                    MessageBox.Show("Failed to parse Phi");
                    return;
                }
                double intensity = 0;
                if (!double.TryParse(InfiniteBrightness.Text, out intensity))
                {
                    MessageBox.Show("Failed to parse brightness");
                    return;
                }

                string colortxt = InfiniteColor.Text;
                byte r = 0;
                byte g = 0;
                byte b = 0;
                try
                {
                    r = byte.Parse(colortxt.Substring(0, colortxt.IndexOf("-")));
                    colortxt = colortxt.Substring(colortxt.IndexOf("-") + 1);
                    g = byte.Parse(colortxt.Substring(0, colortxt.IndexOf("-")));
                    colortxt = colortxt.Substring(colortxt.IndexOf("-") + 1);
                    b = byte.Parse(colortxt);
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Failed To parse color. Needs to be in the form of r-g-b. each one is value 0 - 255. Example: 0-255-0 will make the brightes green possible");
                    return;
                }
                Light l1 = new Light();
                l1.Name = name;
               
                Color c = Color.FromRgb(r, g, b);
                if (!newlight)
                {
                    viewport3D1.Children.Remove(sli.Model3D);
                    //viewport3D1.Children.Remove(li.LightSource);
                    LightListGroup.Items.Remove(sli);
                    AddedLights.Remove(sli);
                }
                l1.GenerateInfiniteLight(GroundLense, theta, phi, c, intensity);
                //l1.GenerateSpotLight(new Point3D(X, Y, Z), new Vector3D(VX, VY, VZ), coneangle, Color.FromRgb(r, g, b), lightgranularity, intens);
                AddedLights.Add(l1);
                //viewport3D1.Children.Add(l1.Model3D);
                //viewport3D1.Children.Add(l1.LightSource);
                LightListGroup.Items.Add(l1);
                LightListGroup.SelectedItem = l1;
                //AddLightPanel.Visibility = Visibility.Hidden;
            }
            else
            {
                double Y = 0;
                if (!double.TryParse(LightHeight.Text, out Y))
                {
                    MessageBox.Show("Failed to parse light height");
                    return;
                }
                double X = 0;
                if (!double.TryParse(LightXOff.Text, out X))
                {
                    MessageBox.Show("Failed to parse horizontal offset X");
                    return;
                }
                double Z = 0;
                if (!double.TryParse(LightZOff.Text, out Z))
                {
                    MessageBox.Show("Failed to parse depth offset Z");
                    return;
                }
                double VX = 0;
                if (!double.TryParse(LightXMag.Text, out VX))
                {
                    MessageBox.Show("Failed to parse X Direction");
                    return;
                }
                double VY = 0;
                if (!double.TryParse(LightYMag.Text, out VY))
                {
                    MessageBox.Show("Failed to parse Y Direction");
                    return;
                }
                double VZ = 0;
                if (!double.TryParse(LightZMag.Text, out VZ))
                {
                    MessageBox.Show("Failed to parse Z Direction");
                    return;
                }
                double coneangle = 0;
                if (!double.TryParse(LightConeAngle.Text, out coneangle))
                {
                    MessageBox.Show("Failed to parse cone angle");
                    return;
                }
                double intens = 1.0;
                if (!double.TryParse(LightBrightness.Text, out intens))
                {
                    MessageBox.Show("Failed to parse brightness");
                    return;
                }
                double lightgranularity = 0;
                if (!double.TryParse(LightGranularity.Text, out lightgranularity))
                {
                    MessageBox.Show("Failed to parse granularity");
                    return;
                }
                string colortxt = LightColor.Text;
                byte r = 0;
                byte g = 0;
                byte b = 0;
                try
                {
                    r = byte.Parse(colortxt.Substring(0, colortxt.IndexOf("-")));
                    colortxt = colortxt.Substring(colortxt.IndexOf("-") + 1);
                    g = byte.Parse(colortxt.Substring(0, colortxt.IndexOf("-")));
                    colortxt = colortxt.Substring(colortxt.IndexOf("-") + 1);
                    b = byte.Parse(colortxt);
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Failed To parse color. Needs to be in the form of r-g-b. each one is value 0 - 255. Example: 0-255-0 will make the brightes green possible");
                    return;
                }
                Light l1 = new Light();
                l1.Name = name;
                if (!newlight)
                {
                    viewport3D1.Children.Remove(sli.Model3D);
                    //viewport3D1.Children.Remove(li.LightSource);
                    LightListGroup.Items.Remove(sli);
                    AddedLights.Remove(sli);
                }

                //double maxintensity = 442;
                //intens = Math.Sqrt(Math.Pow(r, 2) + Math.Pow(g, 2) + Math.Pow(b, 2));
                //intens = (intens / maxintensity) * 10;
                l1.GenerateSpotLight(new Point3D(X, Y, Z), new Vector3D(VX, VY, VZ), coneangle, Color.FromRgb(r, g, b), lightgranularity, intens);
                AddedLights.Add(l1);
                viewport3D1.Children.Add(l1.Model3D);
                //viewport3D1.Children.Add(l1.LightSource);
                LightListGroup.Items.Add(l1);
                LightListGroup.SelectedItem = l1;
                //AddLightPanel.Visibility = Visibility.Hidden;
            }
        }
        
        private void GenerateExampleNewLight()
        {
            NewSpotLight.Name = "New Spot Light";
            NewSpotLight.GenerateSpotLight(new Point3D(0, 40, 0), new Vector3D(0, -1.0, 0), 20, Color.FromRgb(255, 100, 100), 3, 100);
            NewInfiniteLight.Name = "New Infinite Light";
            Color c = Color.FromRgb(100, 255, 255);
            NewInfiniteLight.GenerateInfiniteLight(GroundLense, 0, 180, c, 100);

        }


        private void LenseListGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Lense Lense1 = LenseListGroup.SelectedItem as Lense;
            if (Lense1 == null)
                return;
            LenseWidth.Text = Lense1.Width.ToString();            
            LenseHeight.Text = Lense1.Height.ToString();            
            HeightAboveGround.Text = Lense1.GroundHeight.ToString();           
            HorrPoss.Text = Lense1.HorrizontalPos.ToString();           
            DepthPoss.Text = Lense1.DepthPos.ToString();           
            RotationX.Text = Lense1.XDepthTilt.ToString();
            RotationsZ.Text = Lense1.ZHorTilt.ToString();
            TopHorrRadius.Text = (-Lense1.TopHorRadius).ToString();
            TopDepthRadius.Text = (-Lense1.TopDepthRadius).ToString();
            BottDepthRadius.Text = Lense1.BotDepthRadius.ToString();
            BottHorrRadius.Text = Lense1.BotHorRadius.ToString();
            Granularity.Text = Lense1.Granularity.ToString();
            IndexOfRefraction.Text = Lense1.IndexOfRefraction.ToString();            
            

            if(Lense1 == NewLense)
            {
                AddLense.Content = "Add Lense";
            }
            else
            {
                AddLense.Content = "Update Lense";
            }

        }
        Light NewSpotLight = new Light();
        private void LightListGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Light sli = LightListGroup.SelectedItem as Light;
            if(sli == null)
            {
                return;
            }
            if(sli == NewInfiniteLight || sli == NewSpotLight)
            {
                buttonInfiniteAdd.Content = "Add Light";
            }
            else
            {
                buttonInfiniteAdd.Content = "Update Light";
            }
            if(sli.CurrentType == Light.SourceType.Spot)
            {
                LightTabControl.SelectedItem = SpotLightTabe;
                SpotLight spl = sli.LightSource.Content as SpotLight;
                LightHeight.Text = spl.Position.Y.ToString();
                LightXOff.Text = spl.Position.X.ToString();
                LightZOff.Text = spl.Position.Z.ToString();
                LightXMag.Text = spl.Direction.X.ToString();
                LightYMag.Text = spl.Direction.Y.ToString();
                LightZMag.Text = spl.Direction.Z.ToString();
                LightConeAngle.Text = spl.InnerConeAngle.ToString();
                LightBrightness.Text = sli.Intensity.ToString();
                LightGranularity.Text = sli._Granularity.ToString();
                LightColor.Text = sli._Color.R.ToString() + "-" + sli._Color.G.ToString() + "-" + sli._Color.B.ToString();

            }
            else if(sli.CurrentType == Light.SourceType.Infinite)
            {
                LightTabControl.SelectedItem = InfiniteTab;
                InfiniteTheta.Text = sli._Theta.ToString();
                InfinitePhi.Text = sli._Phi.ToString();
                InfiniteBrightness.Text = sli.Intensity.ToString();
                InfiniteColor.Text = sli._Color.R.ToString() + "-" + sli._Color.G.ToString() + "-" + sli._Color.B.ToString();

            }
            




           
        }

        

        private void RealTime_Checked(object sender, RoutedEventArgs e)
        {
            //UpdateRealTime = RealTime.IsChecked;
        }

        private void RealTime_Click(object sender, RoutedEventArgs e)
        {
            UpdateRealTime = RealTime.IsChecked;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            MouseX = e.GetPosition(MainGrid).X;
            MouseY = e.GetPosition(MainGrid).Y;
            
            mouseactivated = true;
        }
    }
}
