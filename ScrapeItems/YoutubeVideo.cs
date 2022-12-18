namespace CaseStudy
{
    public class YoutubeVideo
    {

        public YoutubeVideo(string title, string channel, string views, string link)
        {
            Title = title;
            Channel = channel;
            Views = views;
            Link = link;
        }

        public string Title { get; set; }
        public string Channel { get; set; }
        public string Views { get; set; }
        public string Link { get; set; }

        public override string ToString()
        {
            return $"Title = {Title}\nChannel = {Channel}\nViews = {Views}\nLink = {Link}";
        }
    }
}

