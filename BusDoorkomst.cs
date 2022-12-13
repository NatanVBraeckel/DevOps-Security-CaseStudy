﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseStudy
{
    public class BusDoorkomst
    {

        public BusDoorkomst(string busnummer, string routeNaam, string halteNaam, string datum, string tijdstip, string infoLink)
        {
            Busnummer = busnummer;
            RouteNaam = routeNaam;
            HalteNaam = halteNaam;
            Datum = datum;
            Tijdstip = tijdstip;
            InfoLink = infoLink;
        }

        public string Busnummer { get; set; }
        public string RouteNaam { get; set; }
        public string HalteNaam { get; set; }
        public string Datum { get; set; }
        public string Tijdstip { get; set; }
        public string InfoLink { get; set; }

    }
}