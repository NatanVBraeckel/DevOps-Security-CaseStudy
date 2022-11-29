using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseStudy
{
    public class Scraper
    {
        public static List<YoutubeVideo> YoutubeScraper()
        {
            Console.Write("enter searchterm: ");
            string searchterm = Console.ReadLine();

            WebDriver driver = new ChromeDriver();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

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
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            driver.Navigate().GoToUrl("https://www.ictjob.be/");


            WebElement searchbar = (WebElement)wait.Until(driver => driver.FindElement(By.Id("keywords-input")));
            searchbar.SendKeys($"{searchterm}");
            searchbar.Submit();

            WebElement cookies = (WebElement)wait.Until(driver => driver.FindElement(By.CssSelector(".close-context-message")));
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
        public static List<DailyForecast> WeatherScraper()
        {
            Console.Write("Geef locatie in: ");
            string chosen_location = Console.ReadLine();

            WebDriver driver = new ChromeDriver();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            driver.Navigate().GoToUrl("https://www.accuweather.com/");

            WebElement consent = (WebElement)wait.Until(driver => driver.FindElement(By.CssSelector(".fc-button.fc-cta-consent.fc-primary-button")));
            consent.Click();

            WebElement searchbar = (WebElement)wait.Until(driver => driver.FindElement(By.ClassName("search-input")));
            searchbar.SendKeys(chosen_location);
            searchbar.Submit();
            //hierna ga je oftewel direct naar de juiste pagina (bv Averbode), of je gaat naar pagina met verschillende opties

            //moet dus eerst checken of we al direct op de juiste pagina zitten of niet?
            //=> als de dailyknop er is weten we dat we juist zitten
            WebElement daily;
            {
                try
                {
                    daily = (WebElement)driver.FindElement(By.CssSelector("a[data-qa=\'daily\']"));
                    daily.Click();
                    //soms krijg je nog stomme ad, door refresh gaat die weg
                    driver.Navigate().Refresh();
                }
                catch (OpenQA.Selenium.NoSuchElementException)
                {
                    IList<IWebElement> locations = wait.Until(driver => driver.FindElements(By.CssSelector(".locations-list a")));
                    Console.WriteLine($"{locations.Count} options found");
                    for (int i = 0; i < locations.Count; i++)
                    {
                        Console.WriteLine($"{i}: {locations[i].Text}");
                    }
                    Console.Write("Choose an option: ");
                    int choice = int.Parse(Console.ReadLine());
                    locations[choice].Click();

                    //soms komt er een advertentie na het klikken van de optie
                    //gewoon refreshen van de pagina voldoet niet, want je blijft gewoon op de pagina
                    //dus na het klikken => check of we op juiste pagina zitten => zo niet doe een refresh en klik opnieuw
                    //na het opnieuw klikken krijg je geen advertentie meer, maar je mag enkel opnieuw klikken als je nog op dezelfde pagina bent natuurlijk
                    try
                    {
                        daily = (WebElement)driver.FindElement(By.CssSelector("a[data-qa=\'daily\']"));
                    }
                    catch (OpenQA.Selenium.NoSuchElementException)
                    {
                        driver.Navigate().Refresh();
                        IList<IWebElement> locations_reload = wait.Until(driver => driver.FindElements(By.CssSelector(".locations-list a")));
                        locations_reload[choice].Click();
                    }
                }
            }

            //soms komt er een advertentie... maar door refresh gaat die weg
            //stel we komen van iets dat geen verschillende keuzes had, staan we wel al op juiste pagina voordat we ad krijgen, door refresh gaat die dan weg
            //driver.Navigate().Refresh();
            //normaal gezien zijn we nu op de juiste pagina

            //WebElement daily = (WebElement)wait.Until(driver => driver.FindElement(By.CssSelector("a[data-qa=\'daily\']")));
            daily = (WebElement)wait.Until(driver => driver.FindElement(By.CssSelector("a[data-qa=\'daily\']")));
            daily.Click();

            List<DailyForecast> dailyForecasts = new List<DailyForecast>();

            IList<IWebElement> dailyWrappers = wait.Until(driver => driver.FindElements(By.ClassName("daily-wrapper")));
            foreach (IWebElement dailyWrapper in dailyWrappers)
            {
                string date = dailyWrapper.FindElement(By.CssSelector(".module-header.sub.date")).Text;
                string[] seperated_date = date.Split('-');
                string day = seperated_date[0];
                string month = seperated_date[1];
                string temp = dailyWrapper.FindElement(By.CssSelector(".temp .high")).Text;
                string phrase = dailyWrapper.FindElement(By.CssSelector(".phrase")).Text;
                string rain = dailyWrapper.FindElement(By.CssSelector(".precip")).Text;
                Console.WriteLine($"day = {day}\nmonth={month}\ntemp = {temp}\nphrase = {phrase}\nrain = {rain}");
                dailyForecasts.Add(new DailyForecast(day, month, temp.Replace("°", ""), phrase, rain));
            }

            driver.Quit();
            return dailyForecasts;
        }
    }
}
