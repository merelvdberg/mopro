using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab3
{
    public class Track
    {
        string startStation;
        string endStation;
        int tariefeenheid;

        public Track (string start, string end, int tarief)
        {
            startStation = start;
            endStation = end;
            tariefeenheid = tarief;
        }

        public void SwitchStations(string begin)
        {
            if (begin == endStation)
            {
                string swapStation = startStation;
                startStation = endStation;
                endStation = swapStation;

            }
        }

        public int getTariefeenheden()
        {
            return tariefeenheid;
        }

        public string getStationFrom()
        {
            return startStation;
        }

        public string getStationTo()
        {
            return endStation;
        }
    }  
}
