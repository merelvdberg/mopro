using Android.Widget;
using Android.OS;
using Android.App;
using Android.Views;
using Android.Graphics;
using Android.Content;
using Android.Content.PM;
using System;
using Android;
using Android.Hardware;   // vanwege SensorManager
using Android.Locations;



namespace App3
{
    [Activity(Label = "App3", MainLauncher = true)]
    public class MainActivity : Activity
    {
        TextView RunningApp;
        Button Startknop; Button Stopknop;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ARView kaart;
            kaart = new ARView(this);
            //Titel
            RunningApp = new TextView(this);
            RunningApp.TextSize = 40;
            RunningApp.Text = "Running App! ";
            RunningApp.SetTextColor(Color.Blue);

            //Start + stop knop
            Startknop = new Button(this);
            Startknop.TextSize = 20;
            Startknop.Text = "Start";
            Startknop.SetTextColor(Color.Pink);
            Stopknop = new Button(this);
            Stopknop.TextSize = 20;
            Stopknop.Text = "Stop";
            Stopknop.SetTextColor(Color.Pink);

            //De knoppen
            LinearLayout knoppen;
            knoppen = new LinearLayout(this);
            knoppen.AddView(Startknop);
            knoppen.AddView(Stopknop);
            knoppen.Orientation = Orientation.Horizontal;

            //Overzicht
            LinearLayout Overzicht;
            Overzicht = new LinearLayout(this);
            Overzicht.AddView(RunningApp);
            Overzicht.AddView(knoppen);
            Overzicht.AddView(kaart);
            Overzicht.Orientation = Orientation.Vertical;

            this.SetContentView(Overzicht);
        }

        public class ARView : View
        {

            Bitmap geo; float Schaal; PointF centrum = new PointF(138300, 454300); float midx; float midy;
            bool pinching = false; 

            public ARView(Context context) : base(context)
            {
                BitmapFactory.Options opt = new BitmapFactory.Options();
                opt.InScaled = false;
                geo = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.Kaart, opt);
                this.Touch += RaakAan;
                
                Schaal = 1;
            }

            static float Afstand(PointF p1, PointF p2)
            {
                float a = p1.X - p2.X;
                float b = p1.Y - p2.Y;
                return (float)Math.Sqrt(a * a + b * b);
            }
            private PointF start1;
            private PointF start2;
            private PointF huidig1;
            private PointF huidig2;
            private PointF dragstartpunt; //in schermpixels
            private float oudeSchaal;

            public void RaakAan(object o, TouchEventArgs tea)
            {
                huidig1 = new PointF(tea.Event.GetX(0), tea.Event.GetY(0));
                if (tea.Event.PointerCount == 2)
                {
                    huidig2 = new PointF(tea.Event.GetX(1), tea.Event.GetY(1));
                    if (tea.Event.Action == MotionEventActions.Pointer2Down)
                    {
                        start1 = huidig1;
                        start2 = huidig2;
                        oudeSchaal = Schaal;
                    }

                    pinching = true;
                    float oud = Afstand(start1, start2);
                    float nieuw = Afstand(huidig1, huidig2);

                    if (oud != 0 && nieuw != 0)
                    {
                        float factor = nieuw / oud;
                        this.Schaal = (float)Math.Max(Math.Min(oudeSchaal * factor, 2.5), 0.32);
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 this.Invalidate();
                    }

                    pinching = false;
                }

                else if (!pinching)
                {
                    

                    if (tea.Event.Action == MotionEventActions.Down)
                    {
                        dragstartpunt = new PointF(tea.Event.GetX(), tea.Event.GetY());
                    }

                    else if (dragstartpunt!=null)
                    {
                        float x = tea.Event.GetX();
                        float sx = x - dragstartpunt.X;
                        float px = sx / Schaal;
                        float ax = px / 0.4f;
                        centrum.X -= ax;
                        // of met if of met Math.Max/Min beperken hoe ver naar links en rechts draggen

                        float y = tea.Event.GetY();
                        float sy = y - dragstartpunt.Y;
                        float py = sy / Schaal;
                        float ay = -py / 0.4f;
                        centrum.Y -= ay;
                        // of met if of met Math.Max/Min beperken hoe ver omlaag en omhoog draggen

                        dragstartpunt = new PointF(tea.Event.GetX(), tea.Event.GetY());

                        
                    }
                }

                this.Invalidate();
            }

            /* PointF huidig= new PointF(139, 456); //in meters!

            public void OnLocationChanged(Location loc)
            {
                huidig = Kaart.Projectie.Geo2RD(loc);
                this.Invalidate();
            }*/

            protected override void OnDraw(Canvas canvas)
            {
                base.OnDraw(canvas);
                Paint verf = new Paint();
                midx = (centrum.X - 136000) * 0.4f;
                midy = -(centrum.Y - 458000) * 0.4f;

               /* float ax = huidig.X - centrum.X;
                float px = ax * 0.4f;
                float sx = px * Schaal;
                float x = this.Width / 2 + sx;

                float ay = huidig.Y - centrum.Y;
                float py = ay * 0.4f;
                float sy = py * Schaal;
                float y = this.Width / 2 + sy;

                canvas.DrawCircle(x, y, 5, verf);*/

                // Schaal = this.Width / geo.Width;
                Matrix mat = new Matrix();
                mat.PostTranslate(-midx, -midy);
                mat.PostScale(Schaal, Schaal);

                mat.PostTranslate(canvas.Width / 2, canvas.Height / 2);

                canvas.DrawBitmap(geo, mat, verf);
            }
        }
    }
}

