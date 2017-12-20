using Android.Widget;
using Android.OS;
using Android.App;
using Android.Views;
using Android.Graphics;
using Android.Content;
using System;
using Android.Hardware;   // vanwege SensorManager
using Android.Locations;
using Android.Runtime;
using System.Collections.Generic; // vanwege Lists

namespace App3
{
    [Activity(Label = "Running App", MainLauncher = true)]
    public class MainActivity : Activity
    {
        static TextView Status;
        Button Startknop, Stopknop, Centreerknop, Wisknop; TextView RunningApp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RunningView kaart;
            kaart = new RunningView(this);
            //Titel
             RunningApp = new TextView(this);
             RunningApp.TextSize = 40;
             RunningApp.Text = "Running App! ";
             RunningApp.SetTextColor(Color.Blue);

            //Knoppen
            Startknop = new Button(this); // bool maken 'aan het verzamelen' default false, if toevoegen zodat hij alleen verzamelt als je op start drukt
            Startknop.TextSize = 20;
            Startknop.Text = "Start";
            Startknop.SetTextColor(Color.Pink);
            Startknop.Click += kaart.Starten;

            Stopknop = new Button(this);
            Stopknop.TextSize = 20;
            Stopknop.Text = "Stop";
            Stopknop.SetTextColor(Color.Pink);
            Stopknop.Click += kaart.Stoppen;

            Centreerknop = new Button(this); //centrum=huidig, this.invalidate();
            Centreerknop.TextSize = 20;
            Centreerknop.Text = "Centreer";
            Centreerknop.SetTextColor(Color.Pink);
            Centreerknop.Click += kaart.Centreren;

            //wisknop
            Wisknop = new Button(this);
            Wisknop.TextSize = 20;
            Wisknop.Text = "Wis";
            Wisknop.SetTextColor(Color.Pink);
            Wisknop.Click += kaart.Wissen;

            //De knoppen
            LinearLayout knoppen;
            knoppen = new LinearLayout(this);
            knoppen.AddView(Startknop);
            knoppen.AddView(Stopknop);
            knoppen.AddView(Centreerknop);
            knoppen.AddView(Wisknop);
            knoppen.Orientation = Orientation.Horizontal;

            Status = new TextView(this);
            Status.Text = "Je route is nog niet gestart.";
            Status.TextSize = 20;
            Status.SetTextColor(Color.Green);

            //Overzicht
            LinearLayout Overzicht;
            Overzicht = new LinearLayout(this);
            Overzicht.AddView(RunningApp);
            Overzicht.AddView(knoppen);
            Overzicht.AddView(Status);
            Overzicht.AddView(kaart);
            Overzicht.Orientation = Orientation.Vertical;

            this.SetContentView(Overzicht);
        }

        public class RunningView : View, ILocationListener, ISensorEventListener
        {
            Bitmap geo, arrow;
            float Schaal, midx, midy, Hoek;
            PointF centrum = new PointF(138300, 454300);
            bool pinching = false;
            bool gestart = false;
            List<PointF> route = new List<PointF>();
            Context onzecontext;
            private PointF start1, start2, huidig1, huidig2, dragstartpunt, maximaal, minimaal; //dragstartpunt = in schermpixels
            private float oudeSchaal;

            public RunningView(Context context) : base(context)
            {
                onzecontext = context;
                BitmapFactory.Options opt = new BitmapFactory.Options();
                opt.InScaled = false;
                geo = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.Kaart, opt);

                arrow = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.Arrow, opt);
                arrow = Bitmap.CreateScaledBitmap(arrow, arrow.Width / 4, arrow.Height / 4, false);

                this.Touch += RaakAan;

                LocationManager lm = (LocationManager)context.GetSystemService(Context.LocationService);
                Criteria crit = new Criteria();
                crit.Accuracy = Accuracy.Fine;
                string lp = lm.GetBestProvider(crit, true);
                lm.RequestLocationUpdates(lp, 0, 5, this);

                SensorManager sm = (SensorManager)context.GetSystemService(Context.SensorService);
                sm.RegisterListener(this, sm.GetDefaultSensor(SensorType.Orientation), SensorDelay.Ui);

                Schaal = 1;
                maximaal = new PointF();
                minimaal = new PointF();
            }

            static float Afstand(PointF p1, PointF p2)
            {
                float a = p1.X - p2.X;
                float b = p1.Y - p2.Y;
                return (float)Math.Sqrt(a * a + b * b);
            }


            public void RaakAan(object o, TouchEventArgs tea)
            {
                huidig1 = new PointF(tea.Event.GetX(0), tea.Event.GetY(0));

                if (tea.Event.PointerCount == 2)
                {
                    pinching = true;
                    Console.WriteLine("wel aan het pinchen");
                    huidig2 = new PointF(tea.Event.GetX(1), tea.Event.GetY(1));

                    if (tea.Event.Action == MotionEventActions.Pointer2Down)
                    {
                        start1 = huidig1;
                        start2 = huidig2;
                        oudeSchaal = Schaal;
                    }

                    float oud = Afstand(start1, start2);
                    float nieuw = Afstand(huidig1, huidig2);

                    if (oud != 0 && nieuw != 0)
                    {
                        float factor = nieuw / oud;
                        this.Schaal = (float)Math.Max(Math.Min(oudeSchaal * factor, 2.5), 0.32);
                        this.Invalidate();
                    }
                }

                else if (!pinching)
                {
                    Console.WriteLine("niet aan het pinchen");
                    if (tea.Event.Action == MotionEventActions.Down)
                    {
                        dragstartpunt = new PointF(tea.Event.GetX(), tea.Event.GetY());
                    }

                    else if (dragstartpunt != null)
                    {
                        float x = tea.Event.GetX();
                        float sx = x - dragstartpunt.X;
                        float px = sx / Schaal;
                        float ax = px / 0.4f;
                        centrum.X -= ax;

                        float y = tea.Event.GetY();
                        float sy = y - dragstartpunt.Y;
                        float py = sy / Schaal;
                        float ay = -py / 0.4f;
                        centrum.Y -= ay;

                        dragstartpunt = new PointF(tea.Event.GetX(), tea.Event.GetY());

                        minimaal.X = (136500);
                        maximaal.X = (141500);
                        minimaal.Y = (453500);
                        maximaal.Y = (457500);

                        if (!(centrum.X > minimaal.X && centrum.X < maximaal.X))
                        {
                            if (!(centrum.X > minimaal.X))
                                centrum.X = minimaal.X;

                            if (!(centrum.X < maximaal.X))
                                centrum.X = maximaal.X;
                        }

                        if (!(centrum.Y > minimaal.Y && centrum.Y < maximaal.Y))
                        {
                            if (!(centrum.Y > minimaal.Y))
                                centrum.Y = minimaal.Y;

                            if (!(centrum.Y < maximaal.Y))
                                centrum.Y = maximaal.Y;
                        }
                    }
                }

                if (tea.Event.Action == MotionEventActions.Up)
                {
                    pinching = false;
                }

                this.Invalidate();
            }

            PointF huidig = null; // in meters!
            //string info;

            public void OnLocationChanged(Location loc)
            {
                huidig = Kaart.Projectie.Geo2RD(loc);
                // double noord = loc.Latitude;
                //double oost = loc.Longitude;
                ///info = $"{noord} graden noorderbreedte, {oost} graden oosterlengte";

                if (gestart == true)
                {
                    route.Add(huidig);
                }

                this.Invalidate();
            }

            public void Starten(Object o, EventArgs ea)
            {
                gestart = true;
                Status.Text = "De route is gestart.";
                this.Invalidate();
            }

            public void Stoppen(Object o, EventArgs ea)
            {
                gestart = false;
                Status.Text = "De route is gestopt.";
                this.Invalidate();
            }

            public void Centreren(Object o, EventArgs ea)
            {
                // huidig = new PointF(140000, 454000);
                centrum.X = huidig.X;
                centrum.Y = huidig.Y;
                this.Invalidate();
            }

            public void Wissen(Object o, EventArgs ea)
            {
                AlertDialog.Builder echtwissen = new AlertDialog.Builder(onzecontext);
                echtwissen.SetTitle("Wil je de route echt wissen?");
                echtwissen.SetPositiveButton("Ja", WelWissen);
                echtwissen.SetNegativeButton("Nee", NietWissen);
                echtwissen.Show();
            }

            protected void WelWissen(object o, EventArgs ea)
            {
                route.Clear();
            }

            protected void NietWissen(object o, EventArgs ea)
            {

            }

            public void OnSensorChanged(SensorEvent e)
            {
                if (e.Sensor.Type == SensorType.Orientation)
                    this.Hoek = e.Values[0];
                this.Invalidate();
            }

            protected override void OnDraw(Canvas canvas)
            {
                base.OnDraw(canvas);
                Paint verf = new Paint();
                verf.Color = Color.DarkRed;

                // midx en midy staan voor het midden van de bitmap
                midx = (centrum.X - 136000) * 0.4f;
                midy = -(centrum.Y - 458000) * 0.4f;

                Matrix mat = new Matrix();
                mat.PostTranslate(-midx, -midy);
                mat.PostScale(Schaal, Schaal);

                mat.PostTranslate(canvas.Width / 2, canvas.Height / 2);

                canvas.DrawBitmap(geo, mat, verf);

                if (huidig != null)
                {
                    float ax = huidig.X - centrum.X;
                    //float px = ax * 0.4f;
                    //float sx = px * Schaal;
                    //float x = this.Width / 2 + sx;
                    float x = meterNaarPixels(huidig.X - centrum.X, true);

                    float ay = huidig.Y - centrum.Y;
                    /*float py = ay * 0.4f;
                    float sy = py * Schaal;
                    float y = this.Width / 2 + -sy;*/
                    float y = meterNaarPixels(huidig.Y - centrum.Y, false);

                    // this.Invalidate();

                    //canvas.DrawText($"{info}", 100, 100, verf);

                    //draw afgelegde route
                    foreach (PointF huidig in route)
                    {
                        float bx = huidig.X - centrum.X;
                        //float px = ax * 0.4f;
                        //float sx = px * Schaal;
                        //float x = this.Width / 2 + sx;
                        float a = meterNaarPixels(huidig.X - centrum.X, true);

                        float by = huidig.X - centrum.X;
                        //float px = ax * 0.4f;
                        //float sx = px * Schaal;
                        //float x = this.Width / 2 + sx;
                        float b = meterNaarPixels(huidig.X - centrum.X, false);

                        canvas.DrawCircle(a, b, 10, verf);
                    }

                    Matrix pmat = new Matrix();
                    pmat.PostTranslate(-arrow.Width / 2, -arrow.Height / 2);
                    pmat.PostRotate(-this.Hoek);
                    //pmat.PostTranslate(arrow.Width / 2, arrow.Height / 2);
                    pmat.PostTranslate(x, y);
                    canvas.DrawBitmap(arrow, pmat, null);

                    // this.Invalidate();
                }
            }

            public void OnProviderDisabled(string provider)
            {
                throw new NotImplementedException();
            }

            public void OnProviderEnabled(string provider)
            {
                throw new NotImplementedException();
            }

            public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
            {
                throw new NotImplementedException();
            }

            public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
            {

            }

            float meterNaarPixels(float input, bool plus)
            {
                float px = input * 0.4f;
                float sx = px * Schaal;
                float x = this.Width / 2;
                if (plus)
                    return x + sx;
                else
                    return x - sx;
            }
        }
    }
}
