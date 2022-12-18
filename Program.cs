namespace CaseStudy
{
    class Program
    {

        static void Main(string[] args)
        {

            //dit haalt de huidige dir op, zonder de bin en debugmappen. Hier kan ik dan de csv en json files in een map zetten
            string exportDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + @"\Exports\";

            void Program()
            {
                Console.Clear();
                
                List<string> scrapeOpties = new List<string>{ "Scrape 5 YT videos", "Scrape 5 ictjobs", "Scrape weer van de komende dagen", "Scrape doorkomende bussen van een halte" };
                Menu.PrintLogo(leftPadding: 4, color: Menu.RandomColor());
                Menu.PrintCustomMenu("Kies een optie", scrapeOpties, "q: Quit programma", color: Menu.RandomColor());
                string option = Menu.GetOption(scrapeOpties.Count, extra_option: "q");

                switch (option)
                {
                    case "0":
                        List<YoutubeVideo> youtubeVideosList = Scraper.YoutubeScraper2();
                        Menu.ConvertMenu(exportDirectory, youtubeVideosList);
                        Program();
                        break;
                    case "1":
                        List<Vacature> vacaturesList = Scraper.VacatureScraper();
                        Menu.ConvertMenu(exportDirectory, vacaturesList);
                        Program();
                        break;
                    case "2":
                        List<DailyForecast> dailyForecastList = Scraper.WeatherScraper();
                        Menu.ConvertMenu(exportDirectory, dailyForecastList);
                        Program();
                        break;
                    case "3":
                        List<BusDoorkomst> busDoorkomstList = Scraper.LijnScraper();
                        Menu.ConvertMenu(exportDirectory, busDoorkomstList);
                        Program();
                        break;
                    case "q":
                        break;
                    default:
                        break;
                }
            }

            Program();

        }
    }
}