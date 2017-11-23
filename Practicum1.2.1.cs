using Android.App;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using System;

[Activity(Label = "Hallo", MainLauncher = true)]

public class Hallo : Activity
{
    Button Knop; EditText Edit; 

    protected override void OnCreate(Bundle b)
    {
        base.OnCreate(b);

        TextView Tekst;
        Tekst = new TextView(this);
        Tekst.Text = "Hoe heet je?";
        Tekst.TextSize = 30;
        Tekst.SetBackgroundColor(Color.White);
        Tekst.SetTextColor(Color.Green);

        Edit = new EditText(this);
        Edit.TextSize = 40;
        Edit.SetBackgroundColor(Color.Black);
        Edit.SetTextColor(Color.White);

        Knop = new Button(this);
        Knop.Text = "Bevestig";
        Knop.TextSize = 40;
        Knop.Click += klik;
                      
        LinearLayout stapel;
        stapel = new LinearLayout(this);
        stapel.Orientation = Orientation.Vertical;

        stapel.AddView(Tekst);
        stapel.AddView(Edit);
        stapel.AddView(Knop);
       
        this.SetContentView(stapel);
    }

    public void klik(object o, EventArgs ea)
    {
        TextView naam2;
        naam2 = new TextView(this);
        naam2.Text = "Hallo, " + Edit.Text +"!";
        
        int NumberOfLetters = Edit.Text.Length;
        TextView naam3;
        naam3 = new TextView(this);
        naam3.Text = "Je naam bestaat uit " + NumberOfLetters + " letters.";

        LinearLayout stapel2;
        stapel2 = new LinearLayout(this);
        stapel2.Orientation = Orientation.Vertical;

        stapel2.AddView(naam2);
        stapel2.AddView(naam3);
        SetContentView(stapel2);
    }
}
