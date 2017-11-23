using Android.App;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using System;

namespace App2
{
    [Activity(Label = "Practicum1.2.2", MainLauncher = true)]
    public class MainActivity : Activity
    {
        Button knop;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            TextView uitleg;
            uitleg = new TextView(this);
            uitleg.Text = "Vul hier je geboortedag, -maand en -jaar in (DD/MM/JJJJ).";


            EditText dag;
            dag = new EditText(this);
            dag.Text = "DD";

            EditText maand;
            maand = new EditText(this);
            maand.Text = "MM";

            EditText jaar;
            jaar = new EditText(this);
            jaar.Text = "JJJJ";

            knop = new Button(this);
            knop.Text = "Bereken je leeftijd!";
            knop.Click += klik;

            LinearLayout geboortedatum;
            geboortedatum = new LinearLayout(this);
            geboortedatum.Orientation = Orientation.Horizontal;

            geboortedatum.AddView(dag);
            geboortedatum.AddView(maand);
            geboortedatum.AddView(jaar);


            LinearLayout hoofdstapel;
            hoofdstapel = new LinearLayout(this);
            hoofdstapel.Orientation = Orientation.Vertical;

            hoofdstapel.AddView(uitleg);
            hoofdstapel.AddView(geboortedatum);
            SetContentView(hoofdstapel);

        }

        public void klik(object o, EventArgs ea);
        {
            
        }
          
    }
}

