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
            //titel
            RunningApp = new TextView(this);
            RunningApp.TextSize = 40;
            RunningApp.Text = "Running App! ";
            RunningApp.SetTextColor(Color.Blue);

            // start+stop knop
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

        public class ARView : View, ScaleGestureDetector.IOnScaleGestureListener
        {

            Bitmap geo; float Schaal; ScaleGestureDetector det;

            public ARView(Context context) : base(context)
            {
                BitmapFactory.Options opt = new BitmapFactory.Options();
                opt.InScaled = false;
                geo = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.Kaart, opt);
                this.Touch += RaakAan;
                det = new ScaleGestureDetector(context, this);
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

                    float oud = Afstand(start1, start2);
                    float nieuw = Afstand(huidig1, huidig2);
                    if (oud != 0 && nieuw != 0)
                    {
                        float factor = nieuw / oud;
                        this.Schaal = (float)Math.Max(Math.Min(oudeSchaal * factor, 2.5), 0.32);
                        this.Invalidate();
                    }
                }
            }

            public void OnScale(ScaleGestureDetector d)
            {
                this.Schaal = Schaal * d.ScaleFactor;
                this.Invalidate();
            }


            protected override void OnDraw(Canvas canvas)
            {
                base.OnDraw(canvas);
                Paint verf = new Paint();

               // Schaal = this.Width / geo.Width;
                Matrix mat = new Matrix();
                mat.PostTranslate(-geo.Width / 2, -geo.Height / 2);
                mat.PostScale(Schaal, Schaal);

                mat.PostTranslate(canvas.Width / 2, canvas.Height / 2);

                canvas.DrawBitmap(geo, mat, verf);
            }

            bool ScaleGestureDetector.IOnScaleGestureListener.OnScale(ScaleGestureDetector detector)
            {
                throw new NotImplementedException();
            }

            public bool OnScaleBegin(ScaleGestureDetector detector)
            {
                throw new NotImplementedException();
            }

            public void OnScaleEnd(ScaleGestureDetector detector)
            {
                throw new NotImplementedException();
            }
        }


    }
}
