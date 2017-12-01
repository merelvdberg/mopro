using Android.OS;         // vanwege Bundle
using Android.App;        // vanwege Activity
using Android.Views;      // vanwege View
using Android.Graphics;   // vanwege Color, Paint, Canvas
using Android.Content;    // vanwege Context
using Android.Content.PM; // vanwege ScreenOrientation
using System;             // vanwege Math
using System.Collections.Generic; // vanwege Lists

[ActivityAttribute(Label = "Vingerverven", MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]

public class Vingerverf : Activity
{
    MijnView scherm;

    protected override void OnCreate(Bundle b)
    {
        base.OnCreate(b);
        scherm = new MijnView(this);
        scherm.SetBackgroundColor(Color.White);
        this.SetContentView(scherm);

    }
}

public class MijnView : View
{
    
    List<PointF> alles = new List<PointF>();

    public MijnView(Context c):base(c)
    {
        this.Touch += Tik;
    }

    protected override void OnDraw(Canvas c)
    {
        float vx=0, vy=0;

        base.OnDraw(c);
        //if (p != null)
        //{
            foreach (PointF p in alles)
            {
                c.DrawLine(p.X, p.Y, vx, vy, new Paint());
            vx = p.X;
            vy = p.Y;
            }
        //}    
    }

    public void Tik(object o, TouchEventArgs tea)
    {
        float x, y;
        PointF q;
        x = tea.Event.GetX();
        y = tea.Event.GetY();
        q = new PointF(x,y);
        alles.Add(q);
        this.Invalidate();

    }
}
// https://developer.xamarin.com/guides/xamarin-forms/advanced/skiasharp/paths/finger-paint/
