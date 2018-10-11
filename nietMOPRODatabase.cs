using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Lab3
{
	public class Database
	{
        public static int totaalTarief;
        public static List<Track> alleTracks = new List<Track>();

		public static String[] getStations()
		{
            return new String[] {
                "Utrecht Centraal",
                "Gouda",
                "Geldermalsen",
                "Hilversum",
                "Duivendrecht",
                "Weesp"
            };
		}

        public static string[] alleStations = new[]
        {
                "Utrecht Centraal",     //0
                "Gouda",                //1
                "Geldermalsen",         //2
                "Hilversum",            //3
                "Duivendrecht",         //4
                "Weesp"                 //5
        };

        public static void addTracks() //manier vinden om een error te geven als begin en eind gelijk zijn like 2 integers en dan aan elkaar gelijkstellen
        {
            //Utrecht Centraal
            alleTracks.Add(new Track(alleStations[0], alleStations[1], 32)); //gouda
            alleTracks.Add(new Track(alleStations[0], alleStations[2], 26)); //geldermalsen
            alleTracks.Add(new Track(alleStations[0], alleStations[3], 18)); //hilversum
            alleTracks.Add(new Track(alleStations[0], alleStations[4], 31)); //duivendrecht
            alleTracks.Add(new Track(alleStations[0], alleStations[5], 33)); //weesp

            //Gouda
            alleTracks.Add(new Track(alleStations[1], alleStations[2], 58)); //geldermalsen
            alleTracks.Add(new Track(alleStations[1], alleStations[3], 50)); //hilversum
            alleTracks.Add(new Track(alleStations[1], alleStations[4], 54)); //duivendrecht
            alleTracks.Add(new Track(alleStations[1], alleStations[2], 57)); //weesp

            //Geldermalsen
            alleTracks.Add(new Track(alleStations[2], alleStations[3], 44)); //hilversum
            alleTracks.Add(new Track(alleStations[2], alleStations[4], 57)); //duivendrecht
            alleTracks.Add(new Track(alleStations[2], alleStations[5], 59)); //weesp

            //Hilversum
            alleTracks.Add(new Track(alleStations[3], alleStations[4], 18)); //duivendrecht
            alleTracks.Add(new Track(alleStations[3], alleStations[5], 15)); //weesp

            //Duivendrecht
            alleTracks.Add(new Track(alleStations[4], alleStations[5], 3)); //weesp
        }

        public static int getTariefeenheden(List<Track> Tracks, string beginStation, string endStation)
        {
            if (beginStation == endStation)
            {
                MessageBox.Show("You have selected the same start station and destination.");
                return 0;
            }

            foreach (Track station in Tracks)
            {
                if (station.getStationFrom() == beginStation || station.getStationTo() == beginStation)
                {
                    station.SwitchStations(beginStation);

                    if (station.getStationTo() == endStation)
                    {
                        totaalTarief = station.getTariefeenheden();
                        return totaalTarief;
                    }
                }
            }
            return 0;
        }

       
    }

}
