using Android.OS;         // vanwege Bundle
using Android.App;        // vanwege Activity
using Android.Views;      // vanwege View
using Android.Graphics;   // vanwege Color, Paint, Canvas
using Android.Content;    // vanwege Context
using Android.Content.PM; // vanwege ScreenOrientation
using System;             // vanwege Math

[ActivityAttribute(Label = "Ogen", MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]

public class OgenApp : Activity
{
    protected override void OnCreate(Bundle b)
    {
        // Maak de View voor de ogen en zorg dat deze op het scherm te zien zijn.
        base.OnCreate(b);
        OgenView ogen;
        ogen = new OgenView(this);
        this.SetContentView(ogen);
    }
}
public class OgenView : View
{
    // Declareer punt P die nodig is in meerdere methoden deze klasse.
    PointF p;

    public OgenView(Context c) : base(c)
    {
        // De achtergrondkleur is geel en wanneer de gebruiker op het scherm tikt, wordt de methode Tik aangeroepen. 
        this.SetBackgroundColor(Color.Yellow);
        this.Touch += Tik;
    }

    protected override void OnDraw(Canvas canvas)
    {
        // Hier wordt canvas aangeroepen om op te tekenen.
        base.OnDraw(canvas);

        // De methode DrawOog wordt hier twee keer aangeroepen, zodat er twee ogen op het scherm komen. Op elk scherm komen de ogen naast elkaar in het midden. 
        DrawOog(canvas, this.Width / 4, this.Height / 2);
        DrawOog(canvas, this.Width / 4 * 3, this.Height / 2);
    }

    public void DrawOog(Canvas canvas, int oogX, int oogY)
    {
        // Hierdoor kan in elke opdracht in deze methode verf aangeroepen worden, waarmee tekeningen gemaakt kunnen worden.
        Paint verf;
        verf = new Paint();

        // Hier worden de variabelen voor de straal gedeclareerd. De straal van het oog is straal en de straal van de iris en pupil staan hiermee in verhouding.
        int straal, straaliris, straalpupil;
        straal = this.Height / 3;
        straaliris = straal / 3;
        straalpupil = straal /8;

        // Maak de buitenrand en de binnenkant van het oog. De buitenrand is net iets groter, dus die wordt vermenigvuldigd met 1.05.
        verf.Color = Color.Black;
        canvas.DrawCircle(oogX, oogY, straal * 1.05f, verf);
        verf.Color = Color.White;
        canvas.DrawCircle(oogX, oogY, straal, verf);

        // De positie van de iris wordt gedeclareerd.
        float irisX, irisY;

        // Als het scherm nog niet aangeraakt wordt, staat de iris in het midden van het oog.
        if (p == null)
        {
            irisX = oogX;
            irisY = oogY;
        }

        // Wanneer het scherm wel wordt aangeraakt, wordt de positie van de iris berekend. 
        else
        {
            // Enkele afstandsvariabelen en de stelling van Pythagoras worden ingezet voor het bepalen van de uiteindelijke positie van de iris.
            float dx = p.X - oogX;
            float dy = p.Y - oogY;
            float afstand = (float)Math.Sqrt(dx * dx + dy * dy);

            // De plaats van de iris wordt bepaald wanneer het scherm buiten de ogen wordt aangeraakt.
            if (afstand > straal)
            {
                irisX = oogX + dx / afstand * (straal - straaliris);
                irisY = oogY + dy / afstand * (straal - straaliris);
            }

            // De plaats van de iris wordt bepaald wanneer het scherm binnen een oog wordt aangeraakt. 
            else
            {
                irisX = p.X;
                irisY = p.Y;
            }
        }

        // De iris en de pupil worden getekend. De pupil staat altijd in het midden van de iris.
        verf.Color = Color.Blue;
        canvas.DrawCircle(irisX, irisY, straaliris, verf);
        verf.Color = Color.Black;
        canvas.DrawCircle(irisX, irisY, straalpupil, verf);
    }

    public void Tik(object o, TouchEventArgs tea)
    {
        // Hier wordt van de plaats waar de gebruiker het scherm aanraakt, een coördinaat gemaakt.
        float x, y;
        x = tea.Event.GetX();
        y = tea.Event.GetY();
        p = new PointF(x, y);
        this.Invalidate();

    }

}
