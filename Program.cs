using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Support.UI;
using System;
using System.Reflection.Metadata;
using ServiceStack.Text;
using CaseStudy;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Tracing;
using System.Security.Cryptography.X509Certificates;
using OpenQA.Selenium.DevTools.V105.Emulation;
/*using JsonSerializer = System.Text.Json.JsonSerializer; NIET MEER NODIG OMDAT JSSERIALIZER OOK IN SERVICESTACK.TEXT ZIT*/

namespace CaseStudy
{
    class Program
    {
        public static void PrintMenu()
        {
            //attempt keuzemenu :)
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("|-----------------------|");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" Kies een Optie        ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"|\n|-----------------------|");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" 1: Scrape 5 YT videos ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write($"|\n|");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" 2: Scrape 5 ictjobs   ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write($"|\n|-----------------------|\n|");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" q: Quit               ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"|\n|-----------------------|\n");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static string GetOption()
        {
            string[] options = { "1", "2", "q" };
            Console.Clear();
            PrintMenu();
            Console.Write("Optie: ");
            string option = Console.ReadLine();

            while (!options.Contains(option))
            {
                Console.Clear();
                PrintMenu();
                Console.Write("Foute optie, kies opnieuw: ");
                option = Console.ReadLine();
            }

            return option;
        }
        public static List<YoutubeVideo> YoutubeScraper()
        {
            Console.Write("enter searchterm: ");
            string searchterm = Console.ReadLine();

            WebDriver driver = new ChromeDriver();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            driver.Navigate().GoToUrl($"https://www.youtube.com/results?search_query={searchterm.Replace(' ', '+')}&sp=CAI%253D");

            WebElement accept_terms = (WebElement)wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"content\"]/div[2]/div[6]/div[1]/ytd-button-renderer[2]/yt-button-shape/button/yt-touch-feedback-shape/div/div[2]")));
            accept_terms.Click();

            /*3 seconden slapen, anders krijg je errors omdat pagina nog niet herladen is na accept terms*/
            Thread.Sleep(3000);

            List<YoutubeVideo> YoutubeVideosList = new List<YoutubeVideo>();
            for (int i = 1; i <= 5; i++)
            {
                var video = wait.Until(driver => driver.FindElement(By.CssSelector($"#contents > ytd-video-renderer:nth-of-type({i})")));

                string link = video.FindElement(By.CssSelector("ytd-thumbnail a")).GetAttribute("href");
                string title = video.FindElement(By.CssSelector("#video-title yt-formatted-string")).Text;
                /*Ik snap niet waarom maar .Text werkt niet op de channel, maar innerhtml opvragen wel*/
                string channel = video.FindElement(By.CssSelector("#metadata a")).GetAttribute("innerHTML");
                string views = video.FindElement(By.CssSelector("#metadata-line > span:nth-child(2)")).Text;

                if (views == "Geen weergaven")
                {
                    views = "0";
                }
                else
                {
                    views = views.Substring(0, views.IndexOf(' '));
                }

                /*Console.WriteLine($"link = {link}\ntitle = {title}\nchannel = {channel}\nviews = {views}\n");*/

                YoutubeVideosList.Add(new YoutubeVideo(title, channel, views, link));
            }

            driver.Quit();

            return YoutubeVideosList;
        }

        public static List<Vacature> VacatureScraper()
        {
            Console.Write("Geef zoekterm in: ");
            string searchterm = Console.ReadLine();

            WebDriver driver = new ChromeDriver();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.Navigate().GoToUrl("https://www.ictjob.be/");

            WebElement searchbar = (WebElement)wait.Until(driver => driver.FindElement(By.Id("keywords-input")));
            searchbar.SendKeys($"{searchterm}");
            searchbar.Submit();

            WebElement cookies = (WebElement)wait.Until(driver => driver.FindElement(By.CssSelector("#body-ictjob > div.context-message-ctn > a")));
            cookies.Click();

            WebElement sortDateButton = (WebElement)wait.Until(driver => driver.FindElement(By.Id("sort-by-date")));
            sortDateButton.Click();

            Thread.Sleep(15000);

            IList<IWebElement> vacatures = wait.Until(driver => driver.FindElements(By.CssSelector($"li.search-item.clearfix")));
            List<IWebElement> newvacatures = vacatures.ToList().GetRange(0, 5);

            List<Vacature> VacaturesList = new List<Vacature>();

            foreach (IWebElement vacature in newvacatures)
            {
                string title = vacature.FindElement(By.CssSelector("h2.job-title")).Text;
                string bedrijf = vacature.FindElement(By.ClassName("job-company")).Text;
                string locatie = vacature.FindElement(By.CssSelector("span[itemprop=addressLocality]")).Text;
                string keywords;
                try
                {
                    keywords = vacature.FindElement(By.ClassName("job-keywords")).Text;
                }
                catch (OpenQA.Selenium.NoSuchElementException)
                {
                    keywords = "";
                }
                string link = vacature.FindElement(By.CssSelector("a")).GetAttribute("href");

                VacaturesList.Add(new Vacature(title, bedrijf, locatie, keywords, link));
                //Console.WriteLine($"titel = {title}\nbedrijf = {bedrijf}\nlocatie = {locatie}\nkeywords = {keywords}\nlink = {link}\n");
            }

            driver.Quit();
            return VacaturesList;
        }

        public static void ExportCSV(string path, string filename, List<YoutubeVideo> items)
        {
            string pathCSV = path + @"\" + $"{filename}.csv";
            string csvString = CsvSerializer.SerializeToString(items);
            File.WriteAllText(pathCSV, csvString);
        }

        static void Main(string[] args)
        {
            
            /*dit haalt de huidige dir op, zonder de bin en debugmappen. hier kan ik dan de csv en json bestanden naar zetten*/
            string currentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

            void test()
            {
                string option = GetOption();

                switch (option)
                {
                    case "1":
                        List<YoutubeVideo> YoutubeVideosList = YoutubeScraper();
                        string pathJSON = currentDirectory + @"\YoutubeVids.json";
                        string jsonString = JsonSerializer.SerializeToString(YoutubeVideosList);
                        File.WriteAllText(pathJSON, jsonString);
                        ExportCSV(currentDirectory, "YoutubeVids", YoutubeVideosList);
                        test();
                        break;
                    case "2":
                        List<Vacature> VacaturesList = VacatureScraper();

                        string pathJsonVacature = currentDirectory + @"\Vacatures.json";
                        string jsonStringVacature = JsonSerializer.SerializeToString(VacaturesList);
                        File.WriteAllText(pathJsonVacature, jsonStringVacature);

                        string pathCsvVacature = currentDirectory + @"\Vactures.csv";
                        string csvStringVacature = CsvSerializer.SerializeToString(VacaturesList);
                        File.WriteAllText(pathCsvVacature, csvStringVacature);
                        test();
                        break;
                    case "q":
                        break;
                    default:
                        break;
                }
            }

            test();



            //hieronder deel van Youtube
            /*List<YoutubeVideo> YoutubeVideosList = YoutubeScraper();
          
            string pathJSON = currentDirectory + @"\YoutubeVids.json";
            string jsonString = JsonSerializer.SerializeToString(YoutubeVideosList);
            File.WriteAllText(pathJSON, jsonString);
            ExportCSV(currentDirectory, "YoutubeVids", YoutubeVideosList);*/


            //hieronder deel van ictjobs
            /*List<Vacature> VacaturesList = VacatureScraper();

            string pathJsonVacature = currentDirectory + @"\Vacatures.json";
            string jsonStringVacature = JsonSerializer.SerializeToString(VacaturesList);
            File.WriteAllText(pathJsonVacature, jsonStringVacature);

            string pathCsvVacature = currentDirectory + @"\Vactures.csv";
            string csvStringVacature = CsvSerializer.SerializeToString(VacaturesList);
            File.WriteAllText(pathCsvVacature, csvStringVacature);*/

        }
    }
}