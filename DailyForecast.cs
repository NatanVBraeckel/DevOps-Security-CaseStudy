using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseStudy
{
    public class DailyForecast
    {
        public DailyForecast(string day, string month, string temperature, string phrase, string rain_percentage)
        {
            Day = day;
            Month = month;
            Temperature = temperature;
            Phrase = phrase;
            RainPercentage = rain_percentage;
        }

        public string Day { get; set; }
        public string Month { get; set; }
        public string Temperature { get; set; }
        public string Phrase { get; set; }
        public string RainPercentage { get; set; }
        //public string Link { get; set; }
    }
}
