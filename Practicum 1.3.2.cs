using Android.OS;         // vanwege Bundle
using Android.App;        // vanwege Activity
using Android.Views;      // vanwege View
using Android.Graphics;   // vanwege Color, Paint, Canvas
using Android.Content;    // vanwege Context
using Android.Content.PM; // vanwege ScreenOrientation

[ActivityAttribute(Label = "Smiley", MainLauncher = true)]

public class SmileyApp : Activity
{
    protected override void OnCreate(Bundle b)
    {
        base.OnCreate(b);
        SmileyView smiley;
        smiley = new SmileyView(this);
        this.SetContentView(smiley);
    }
}
public class SmileyView : View
{
    public SmileyView(Context c) : base(c)
    {
        this.SetBackgroundColor(Color.Black);
    }

    protected override void OnDraw(Canvas canvas)
    {
        base.OnDraw(canvas);
            
        // breedte, hoogte, balk, x1, x2, x3, y1, y2;
        // breedte = this.Width;
        // hoogte = this.Height;
        // x1 = 50; x2 = 250; x3 = 450;
        // y1 = 150; y2 = 350;
        // balk = 50;

        Paint verf;
        verf = new Paint();


        // verf.StrokeWidth(3);
        // Paint.setStrokeWidth(3);


        // tekenen smiley
        verf.Color = Color.Yellow;
        canvas.DrawCircle(200, 200, 200, verf);
        verf.Color = Color.Black;
        canvas.DrawCircle(150, 150, 20, verf);
        canvas.DrawCircle(250, 150, 20, verf);
        verf.Color = Color.Red;
        canvas.DrawLine(150, 270, 250, 270, verf);

        // canvas.DrawRect(x1, 0, x1 + balk, hoogte, verf);
        // canvas.DrawRect(x2, 0, x2 + balk, hoogte, verf);
        // canvas.DrawRect(x3, 0, x3 + balk, hoogte, verf);
        // canvas.DrawRect(0, y1, breedte, y1 + balk, verf);
        // canvas.DrawRect(0, y2, breedte, y2 + balk, verf); 

        // gekleurde vlakken
        // verf.Color = Color.Blue;
        // canvas.DrawRect(0, y1 + balk, x1, y2, verf);
        // verf.Color = Color.Red;
        // canvas.DrawRect(x3 + balk, 0, breedte, y1, verf);
    }
}
