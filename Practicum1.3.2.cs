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
        this.SetBackgroundColor(Color.White);
    }

    protected override void OnDraw(Canvas canvas)
    {
        base.OnDraw(canvas);

        Paint verf;
        verf = new Paint();

        // tekenen smiley

        // variabelen
        int breedte, hoogte, straal, o1, o2, o3, o4, o5, m1, m2, m3, m4;
        breedte = this.Width;
        hoogte = this.Height;
        straal = this.Width / 2;
        o1 = 200;
        o2 = 340;
        o3 = 350;
        o4 = 50;
        o5 = 20;
        m1 = 190;
        m2 = 450;
        m3 = 350;
        m4 = 500;
       

        // buitenkant
        verf.Color = Color.Black;
        canvas.DrawCircle(breedte/2, hoogte/2, straal, verf);
        verf.Color = Color.Yellow;
        canvas.DrawCircle(breedte/2, hoogte/2, straal*0.95f, verf);

        // ogen
        verf.Color = Color.White;
        canvas.DrawCircle(o1, o3, o4, verf);
        canvas.DrawCircle(o2, o3, o4, verf);
        verf.Color = Color.Black;
        canvas.DrawCircle(o1, o3, o5, verf);
        canvas.DrawCircle(o2, o3, o5, verf);

        // mond
        verf.Color = Color.Red;
        canvas.DrawArc(m1, m2, m3, m4, 0, 180, true, verf);
        
     }
}
