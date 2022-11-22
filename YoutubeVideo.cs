using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraperYoutube
{
    public class YoutubeVideo
    {

        public YoutubeVideo(string link, string title, string channel, string views)
        {
            Link = link;
            Title = title;
            Channel = channel;
            Views = views;
        }

        public string Link { get; set; }
        public string Title { get; set; }
        public string Channel { get; set; }
        public string Views { get; set; }
    }
}

