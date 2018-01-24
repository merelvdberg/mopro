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
    public class Meting
    {
        public DateTime dt;
        public PointF punt;

        public Meting(DateTime dt, PointF punt) 
        {
            this.dt = dt;
            this.punt = punt;
            //hier gebruiken we nog ergens ToString voor het bericht (deelknop)
        }

        public override string ToString()
        {
            string res;

            DateTime huidigTijd = this.dt;
            float huidigTextX = this.punt.X;
            float huidigTextY = this.punt.Y;

            res = $"🏃 {huidigTijd.ToString("H:mm:ss")}-{huidigTextX} ; {huidigTextY}\n";
            return res;
        }

        public static float Snelheid(Meting pt, Meting vorige)
        {
            //float snelheid = 0;
            //Afstand berekenen in km
            float dx = pt.punt.X - vorige.punt.X;
            float dy = pt.punt.Y - vorige.punt.Y;
            float afstand = (float)Math.Sqrt(dx * dx + dy * dy) / 1000;

            //Tijd in uren
            //verschil = new TimeSpan();
            TimeSpan verschil = pt.dt - vorige.dt;
            //Console.WriteLine(verschil);
            //float.Parse(verschil);

            //Snelheid in km/uur
            float snelheid = afstand / (float)verschil.TotalHours;
            //Console.WriteLine(snelheid);
            //Console.WriteLine((float)verschil.TotalHours);
            

            return snelheid;
        }

        public static float Afstand(Meting pt, Meting vorige)
        {
            float dx = pt.punt.X - vorige.punt.X;
            float dy = pt.punt.Y - vorige.punt.Y;
            float afstand = (float)Math.Sqrt(dx * dx + dy * dy) / 1000;
            return afstand;
        }

        public static float TotaleAfstand(List<Meting> route)
        {
            float res=0;

            Meting vorige = null;

            foreach (Meting pt in route)
            { 
                if (vorige != null)
                {
                    res += Meting.Afstand(pt, vorige);
                }
                vorige = pt;
            }

            return res;
        }

        
    }
}
