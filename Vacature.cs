using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CaseStudy
{
    public class Vacature
    {

        public Vacature(string titel, string bedrijf, string locatie, string keywords, string link)
        {
            Titel = titel;
            Bedrijf = bedrijf;
            Locatie = locatie;
            Keywords = keywords;
            Link = link;
        }

        //titel bedrijf locatie keywords link
        public string Titel { get; set; }
        public string Bedrijf { get; set; }
        public string Locatie { get; set; }
        public string Keywords { get; set; }
        public string Link { get; set; }

        public override string ToString()
        {
            return $"Titel = {Titel}\nBedrijf = {Bedrijf}\nLocatie = {Locatie}\nKeywords = {Keywords}\nLink = {Link}";
        }
    }
}
