using Android.OS;         // vanwege Bundle
using Android.App;        // vanwege Activity
using Android.Views;      // vanwege View
using Android.Graphics;   // vanwege Color, Paint, Canvas
using Android.Content;    // vanwege Context
using Android.Content.PM; // vanwege ScreenOrientation

[ActivityAttribute(Label = "Ogen", MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]

public class OgenApp : Activity
{
    protected override void OnCreate(Bundle b)
    {
        base.OnCreate(b);
        OgenView ogen;
        ogen = new OgenView(this);
        this.SetContentView(ogen);
    }
}
public class OgenView : View
{
    PointF p;

    public OgenView(Context c) : base(c)
    {
        this.SetBackgroundColor(Color.White);
        this.Touch += Tik;
    }

    protected override void OnDraw(Canvas canvas)
    {
        base.OnDraw(canvas);

        DrawOog(canvas, 250, 220);
        DrawOog(canvas, 600, 220);

        //waarden voor a en b moeten berekend worden!
        
    }

    public void DrawOog(Canvas canvas, int c, int d)
    {
        Paint verf;
        verf = new Paint();

        // buitenkant oog
        verf.Color = Color.Black;
        canvas.DrawCircle(c, d, 150*1.05f, verf);
        verf.Color = Color.White;
        canvas.DrawCircle(c, d, 150, verf);

        // pupil
        int a, b;

        if (p == null)
        {
            a = c;
            b = d;
        }
        else
        {
            a = 20;
            b = 50;
        }

        verf.Color = Color.Blue;
        canvas.DrawCircle(a, b, 20, verf);
    }

    public void Tik(object o, TouchEventArgs tea)
    {
        float x, y;
        x = tea.Event.GetX();
        y = tea.Event.GetY();
        p = new PointF(x, y);
        this.Invalidate();

    }

}
