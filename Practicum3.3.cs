using Android.Widget;             // vanwege Buttons 
using Android.OS;                 // vanwege Bundle
using Android.App;                // vanwege Activity
using Android.Views;              // vanwege View
using Android.Graphics;           // vanwege OnDraw
using Android.Content;            // vanwege Context
using System;                     // vanwege EventHandlers 
using Android.Hardware;           // vanwege SensorManager
using Android.Locations;          // vanwege ILocationListener
using Android.Runtime;            // vanwege GeneratedEnum
using System.Collections.Generic; // vanwege Lists

namespace App3
{
    //Het scherm blijft op portrait mode staan, omdat anders de methode OnCreate opnieuw wordt aangeroepen en alle vooruitgang verloren gaat
    //Door 'Theme =' is de standaard titelbar weggehaald, zodat er meer plek is voor de kaart
    [Activity(Label = "Running App", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : Activity
    {
        //Declaraties die in de gehele klasse nodig zijn
        static TextView Status;
        Button Startknop, Stopknop, Centreerknop, Wisknop, Deelknop, Analyseerknop; TextView RunningApp;
        public ToggleButton Fakeknop;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RunningView kaart;
            kaart = new RunningView(this);
            kaart.SetBackgroundColor(new Color(208, 229, 158));

            //Titelbar
            RunningApp = new TextView(this);
            RunningApp.TextSize = 40;
            RunningApp.Text = "🏃 Running App! 🏃";
            RunningApp.SetTextColor(Color.Yellow);

            //Knoppen
            Startknop = new Button(this);
            Startknop.TextSize = 20;
            Startknop.Text = "Start";
            Startknop.SetTextColor(Color.Black);
            Startknop.Click += kaart.Starten;

            Stopknop = new Button(this);
            Stopknop.TextSize = 20;
            Stopknop.Text = "Stop";
            Stopknop.SetTextColor(Color.Black);
            Stopknop.Click += kaart.Stoppen;

            Centreerknop = new Button(this);
            Centreerknop.TextSize = 20;
            Centreerknop.Text = "Centreer";
            Centreerknop.SetTextColor(Color.Black);
            Centreerknop.SetHighlightColor(Color.Blue);
            Centreerknop.Click += kaart.Centreren;

            Wisknop = new Button(this);
            Wisknop.TextSize = 20;
            Wisknop.Text = "Wis";
            Wisknop.SetTextColor(Color.Black);
            Wisknop.Click += kaart.Wissen;

            Deelknop = new Button(this);
            Deelknop.TextSize = 20;
            Deelknop.Text = "Share";
            Deelknop.SetTextColor(Color.Black);
            Deelknop.Click += Delen;

            Fakeknop = new ToggleButton(this);
            Fakeknop.TextSize = 20;
            Fakeknop.Text = "Fake";
            Fakeknop.SetTextColor(Color.Black);
            Fakeknop.Click += FakeknopToggle;

            Analyseerknop = new Button(this);
            Analyseerknop.TextSize = 20;
            Analyseerknop.Text = "Analyseer";
            Analyseerknop.SetTextColor(Color.Black);
            Analyseerknop.Click += kaart.Analyseren;



            //Stapels van knoppen
            LinearLayout knoppen;
            knoppen = new LinearLayout(this);
            knoppen.AddView(Startknop);
            knoppen.AddView(Stopknop);
            knoppen.AddView(Centreerknop);
            knoppen.AddView(Wisknop);
            knoppen.Orientation = Orientation.Horizontal;

            LinearLayout knoppen2;
            knoppen2 = new LinearLayout(this);
            knoppen2.AddView(Deelknop);
            knoppen2.AddView(Fakeknop);
            knoppen2.AddView(Analyseerknop);
            knoppen2.Orientation = Orientation.Horizontal;

            //Statusbar
            Status = new TextView(this);
            Status.Text = "Je route is nog niet gestart.";
            Status.TextSize = 20;
            Status.SetTextColor(Color.Yellow);

            //Overzichtsstapel
            LinearLayout Overzicht;
            Overzicht = new LinearLayout(this);
            Overzicht.AddView(RunningApp);
            Overzicht.AddView(knoppen);
            Overzicht.AddView(knoppen2);
            Overzicht.AddView(Status);
            Overzicht.AddView(kaart);
            Overzicht.Orientation = Orientation.Vertical;
            Overzicht.SetBackgroundColor((new Color(68, 0, 0)));

            this.SetContentView(Overzicht);

            void Delen(object o, EventArgs ea)
            {
                Intent i;
                i = new Intent(Intent.ActionSend);
                i.SetType("text/plain");

                string bericht = kaart.GetRouteText();
                i.PutExtra(Intent.ExtraText, bericht);
                this.StartActivity(i);
            }

            void FakeknopToggle(object o, EventArgs ea)
            {
                if (Fakeknop.Checked)
                    kaart.Faken();

                else
                    kaart.route.Clear();

                kaart.Invalidate();
            }
        }

       
        public class RunningView : View, ILocationListener, ISensorEventListener
        {
            //Declaraties die in de gehele klasse nodig zijn. Dragstartpunt is in schermpixels.
            Bitmap geo, arrow;
            float Schaal, midx, midy, Hoek;
            PointF centrum = new PointF(138300, 454300);
            bool pinching = false;
            bool gestart = false;
            bool running = true; //dit is of hij het wel of niet analyseert, maar het wordt verwarrend als analyseren op false staat en hij toch wel analyseert
            //bool faking = true;
            public List<Meting> route = new List<Meting>();
            Context onzecontext;
            private PointF start1, start2, huidig1, huidig2, dragstartpunt, maximaal, minimaal;
            private float oudeSchaal;

            public RunningView(Context context) : base(context)
            {
                //Bitmaps ophalen 
                onzecontext = context;
                BitmapFactory.Options opt = new BitmapFactory.Options();
                opt.InScaled = false;
                geo = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.Kaart, opt);
                arrow = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.Arrow, opt);
                arrow = Bitmap.CreateScaledBitmap(arrow, arrow.Width / 4, arrow.Height / 4, false);

                /* route.Add(new Meting(DateTime.Now, new PointF(centrum.X, centrum.Y)));
                 route.Add(new Meting(DateTime.Now, new PointF(centrum.X+20, centrum.Y+20)));
                 route.Add(new Meting(new DateTime(), new PointF(centrum.X+40, centrum.Y+40)));
                 route.Add(new Meting(new DateTime(), new PointF(centrum.X+60, centrum.Y+60)));
                 route.Add(new Meting(new DateTime(), new PointF(centrum.X+80, centrum.Y+80)));
                 route.Add(new Meting(new DateTime(), new PointF(centrum.X+100, centrum.Y+100)));
                 route.Add(new Meting(new DateTime(), new PointF(centrum.X+120, centrum.Y+120)));
                 route.Add(new Meting(new DateTime(), new PointF(centrum.X+140, centrum.Y+140)));
                 route.Add(new Meting(new DateTime(), new PointF(centrum.X+160, centrum.Y+160)));
                 route.Add(new Meting(new DateTime(), new PointF(centrum.X+1000, centrum.Y+1000)));*/
                 
                this.Touch += RaakAan;

                //Locatie ophalen via Locationmanager zoals beloofd in de ILocationListener
                LocationManager lm = (LocationManager)context.GetSystemService(Context.LocationService);
                Criteria crit = new Criteria();
                crit.Accuracy = Accuracy.Fine;
                string lp = lm.GetBestProvider(crit, true);
                lm.RequestLocationUpdates(lp, 0, 3, this);

                //Sensor aanroepen zoals beloofd in de ISensorEventListener
                SensorManager sm = (SensorManager)context.GetSystemService(Context.SensorService);
                sm.RegisterListener(this, sm.GetDefaultSensor(SensorType.Orientation), SensorDelay.Ui);

                //Defaultwaarde van de variabele Schaal
                Schaal = 1;
            }

            static float Afstand(PointF p1, PointF p2)
            {
                //Stelling van Pythagoras gebruiken voor de pinch
                float a = p1.X - p2.X;
                float b = p1.Y - p2.Y;
                return (float)Math.Sqrt(a * a + b * b);
            }


            public void RaakAan(object o, TouchEventArgs tea)
            {
                huidig1 = new PointF(tea.Event.GetX(0), tea.Event.GetY(0));

                //Wanneer er twee aanrakingen tegelijkertijd plaatsvinden, wordt deze opdracht uitgevoerd en is de user dus aan het pinchen
                if (tea.Event.PointerCount == 2)
                {
                    pinching = true;
                    huidig2 = new PointF(tea.Event.GetX(1), tea.Event.GetY(1));

                    if (tea.Event.Action == MotionEventActions.Pointer2Down)
                    {
                        start1 = huidig1;
                        start2 = huidig2;
                        oudeSchaal = Schaal;
                    }

                    float oud = Afstand(start1, start2);
                    float nieuw = Afstand(huidig1, huidig2);

                    //Randwaarden van het pinchen berekenen
                    if (oud != 0 && nieuw != 0)
                    {
                        float factor = nieuw / oud;
                        this.Schaal = (float)Math.Max(Math.Min(oudeSchaal * factor, 2.5), 0.32);
                        this.Invalidate();
                    }
                }

                //Wanneer de user niet pincht, wordt deze opdracht uitgevoerd en is de user aan het draggen
                else if (!pinching)
                {
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

                        //Minimale en maximale waarden van het draggen worden bepaald
                        maximaal = new PointF();
                        minimaal = new PointF();

                        minimaal.X = (136500);
                        maximaal.X = (141500);
                        minimaal.Y = (453500);
                        maximaal.Y = (457500);

                        //Er wordt voor gezorgd dat de app zich aan de randwaarden houdt
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

                //Zodra de user het scherm loslaat, worden er geen TouchEvents meer aangeroepen
                if (tea.Event.Action == MotionEventActions.Up)
                {
                    pinching = false;
                }

                //Omdat er waarden van variabelen zijn veranderd, wordt de OnDraw-methode opnieuw aangeroepen
                this.Invalidate();
            }

            //Huidig is de exacte GPS-locatie en is in meters
            PointF huidig = null;
            DateTime t;

            public void OnLocationChanged(Location loc)
            {
                huidig = Kaart.Projectie.Geo2RD(loc);
                t = DateTime.Now;
                Meting pt = new Meting(t, huidig);


                //Elk huidige punt wordt opgeslagen in de lijst 'route' waardoor de afgelegde route getekend wordt
                if (gestart == true)
                {
                    route.Add(pt);
                }

                this.Invalidate();
            }

            public string GetRouteText()
            {
                string res = "";

                foreach (Meting pt in this.route)
                {
                    res += pt.ToString();
                }

                // Console.WriteLine(res);
                return res;
            }

            //De EventHandler van de Startknop
            public void Starten(Object o, EventArgs ea)
            {
                gestart = true;
                Status.Text = "De route is gestart.";
                this.Invalidate();
            }

            //De EventHandler van de Stopknop
            public void Stoppen(Object o, EventArgs ea)
            {
                gestart = false;
                Status.Text = "De route is gestopt.";
                this.Invalidate();
            }

            //De EventHandler van de Centreerknop
            public void Centreren(Object o, EventArgs ea)
            {
                centrum.X = huidig.X;
                centrum.Y = huidig.Y;
                this.Invalidate();
            }

            //De EventHandler van de Wisknop
            public void Wissen(Object o, EventArgs ea)
            {
                AlertDialog.Builder echtwissen = new AlertDialog.Builder(onzecontext);
                echtwissen.SetTitle("Wil je de route echt wissen?");
                echtwissen.SetPositiveButton("Ja", WelWissen);
                echtwissen.SetNegativeButton("Nee", NietWissen);
                echtwissen.Show();
            }

            //Wanneer de user kiest om de route te wissen
            protected void WelWissen(object o, EventArgs ea)
            {
                route.Clear();
            }

            //Wanneer de user kiest om de route niet te wissen
            protected void NietWissen(object o, EventArgs ea)
            {

            }

            public void Faken()
            {
                
                route.Add(new Meting(new DateTime(2018, 1, 17, 15, 27, 19), new PointF(138471, 454637)));
                route.Add(new Meting(new DateTime(2018, 1, 17, 15, 27, 20), new PointF(138460, 454644)));
                route.Add(new Meting(new DateTime(2018, 1, 17, 15, 27, 21), new PointF(138450, 454645)));
                route.Add(new Meting(new DateTime(2018, 1, 17, 15, 27, 22), new PointF(138439, 454646)));
                route.Add(new Meting(new DateTime(2018, 1, 17, 15, 27, 23), new PointF(138427, 454647)));
                route.Add(new Meting(new DateTime(2018, 1, 17, 15, 27, 24), new PointF(138417, 454646)));
                route.Add(new Meting(new DateTime(2018, 1, 17, 15, 27, 25), new PointF(138409, 454645)));
                route.Add(new Meting(new DateTime(2018, 1, 17, 15, 27, 26), new PointF(138399, 454644)));
                route.Add(new Meting(new DateTime(2018, 1, 17, 15, 27, 27), new PointF(138392, 454640)));
                route.Add(new Meting(new DateTime(2018, 1, 17, 15, 27, 28), new PointF(138386, 454639)));
                route.Add(new Meting(new DateTime(2018, 1, 17, 15, 27, 29), new PointF(138377, 454636)));
                route.Add(new Meting(new DateTime(2018, 1, 17, 15, 27, 30), new PointF(138369, 454634)));
            }

            public void Analyseren(object o, EventArgs ea)
            {
                running = false;
                this.Invalidate();
            }



            //De hoek van de rotatie van de bitmap wordt berekend
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

                if (running == true)
                {
                    
                    verf.Color = Color.DarkRed;

                    //Midx en midy staan voor het midden van de kaart
                    midx = (centrum.X - 136000) * 0.4f;
                    midy = -(centrum.Y - 458000) * 0.4f;

                    //De kaart (geo) wordt getekend met behulp van de matrix
                    Matrix mat = new Matrix();
                    mat.PostTranslate(-midx, -midy);
                    mat.PostScale(Schaal, Schaal);
                    mat.PostTranslate(canvas.Width / 2, canvas.Height / 2);

                    canvas.DrawBitmap(geo, mat, verf);

                    //Teken de afgelegde route
                    foreach (Meting pt in route)
                    {

                        float bx = pt.punt.X - centrum.X;
                        float qx = bx * 0.4f;
                        float tx = qx * Schaal;
                        float a = this.Width / 2 + tx;

                        float by = pt.punt.Y - centrum.Y;
                        float qy = by * 0.4f;
                        float ty = qy * Schaal;
                        float b = this.Height / 2 + -ty;

                        canvas.DrawCircle(a, b, 10, verf);
                    }

                    //Centrum is het midden van de kaart en huidig is het huidige punt van de GPS
                    if (huidig != null)
                    {
                        float ax = huidig.X - centrum.X;
                        float px = ax * 0.4f;
                        float sx = px * Schaal;
                        float x = this.Width / 2 + sx;

                        float ay = huidig.Y - centrum.Y;
                        float py = ay * 0.4f;
                        float sy = py * Schaal;
                        float y = this.Height / 2 + -sy;

                        //Het pijltje dat aangeeft waar de user zich bevindt wordt getekend met behulp van de matrix
                        Matrix pmat = new Matrix();
                        pmat.PostTranslate(-arrow.Width / 2, -arrow.Height / 2);
                        pmat.PostRotate(this.Hoek);
                        pmat.PostTranslate(x, y);

                        canvas.DrawBitmap(arrow, pmat, null);
                    }
                }
                else
                {

                    Meting vorige = null;
                    //Teken de grafiek
                    foreach (Meting pt in route)
                    {

                        if (vorige != null)// er is nog geen vorig punt
                        {
                            float x = Meting.Snelheid(pt, vorige);
                            //Console.WriteLine(x);
                            canvas.DrawCircle(x+200, x+200, 30, verf);
                        }
                        
                        vorige = pt;
                        
                        // Kijken hoe je aan het vorige punt komt (kijk naar pythagoras in vingerverf)
                        // tijd met timespan, m/s uitrekenen en dan omrekenen naar km/h, afstanden berekenen met pythagoras
                    }


                }



            }

            //De methodes zoals beloofd in de interface
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
        }

       
    }
}
