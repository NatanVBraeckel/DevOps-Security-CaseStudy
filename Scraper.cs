using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.DevTools.V105.Storage;

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

        public static void LijnScraper()
        {
            Console.Write("Geef een lijnnummer in: ");
            string lijn_nummer = Console.ReadLine();

            WebDriver driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

            driver.Navigate().GoToUrl("https://www.delijn.be/");

            WebElement consent = (WebElement)driver.FindElement(By.CssSelector("#onetrust-accept-btn-handler"));
            consent.Click();

            try
            {
                WebElement popup = (WebElement)driver.FindElement(By.CssSelector("button[data-testid=\'snackbar-close-button\']"));
                popup.Click();
            }
            catch (OpenQA.Selenium.NoSuchElementException)
            {}

            WebElement zoek = (WebElement)driver.FindElement(By.CssSelector("a[title=\'Ga naar de zoekpagina\']"));
            driver.Navigate().GoToUrl(zoek.GetAttribute("href"));

            WebElement ingeef_balk = (WebElement)driver.FindElement(By.CssSelector("input[data-testid=\'search-input\']"));
            ingeef_balk.SendKeys(lijn_nummer);
            ingeef_balk.Submit();

            /*string diewegmoeten = "ul[data-testid=\'search-results-list\'] > li[data-testid*='null']";
            string alle = "ul[data-testid=\'search-results-list\'] > li";*/
            // "ul[data-testid='search-results-list'] li:not([data-testid*='null'])" werkte in de browser

            /*IList<IWebElement> alle_resultaten = driver.FindElements(By.CssSelector(alle));
            IList<IWebElement> weg_resulaten = driver.FindElements(By.CssSelector(diewegmoeten));

            List<IWebElement> lijst_alle_resulaten = alle_resultaten.ToList();
            List<IWebElement> lijst_weg_resultaten = weg_resulaten.ToList();

            List<IWebElement> alleen_die_ik_wil = lijst_alle_resulaten.Except(lijst_weg_resultaten).ToList();

            Console.WriteLine(lijst_alle_resulaten.Count);
            Console.WriteLine(lijst_weg_resultaten.Count);
            Console.WriteLine(alleen_die_ik_wil.Count);*/


            List<string> lijst_routes = new List<string>();

            string juiste_list_items = "ul[data-testid =\'search-results-list\'] > li:not([data-testid*=\'null\'])";
            IList<IWebElement> resultaten = driver.FindElements(By.CssSelector(juiste_list_items));
            foreach (IWebElement resultaat in resultaten)
            {
                string nummer_route = resultaat.FindElement(By.CssSelector("a div div:nth-child(1) span")).Text;
                string naam_route = resultaat.FindElement(By.CssSelector("a div div:nth-child(2) span")).Text;
                string string_route = $"[{nummer_route}] {naam_route}";

                lijst_routes.Add(string_route);
            }

            //alle opties zijn nu opgehaald, nu nog de optie van de gebruiker krijgen
            Console.Clear();
            Menu.PrintCustomMenu($"Alle lijnen met nummer {lijn_nummer}", lijst_routes, "Kies een route");
            string route_option = Menu.GetOptionNew(lijst_routes.Count);

            //op de optie die de gebruiker heeft gekozen klikken
            int route_option_int = Int32.Parse(route_option);
            resultaten[route_option_int].FindElement(By.CssSelector("a")).Click();

            //nu alle haltes opvragen van de route
            //eerst button klikken om alle haltes te krijgen
            try
            {
                driver.FindElement(By.CssSelector("button[data-testid=\'load-more-line-stops-button\']")).Click();
            } catch (OpenQA.Selenium.NoSuchElementException)
            {
                //geen button om op te klikken? geen probleem :D
            }

            //even wachten tot alle haltes zijn ingeladen voor je ze allemaal ophaald
            Thread.Sleep(1500);

            //nu alle haltes ophalen
            /*IList<IWebElement> haltes = driver.FindElements(By.CssSelector("ul[data-testid=\'stop-list-with-small-action\'] > li"));
            foreach(IWebElement halte in haltes)
            {
                //span die direct kind is van div is onze titel van de halte
                string titel_halte = halte.FindElement(By.CssSelector("div > span")).Text;
                Console.WriteLine(titel_halte);
            }*/

            List<string> haltes_lijst = new List<string>{};
            //per city haltes ophalen
            //eerst lijst van alle lijsten :p
            IList<IWebElement> steden = driver.FindElements(By.CssSelector("div[data-testid=\'grouped-stops-by-town\'] > ul > li"));
            foreach(IWebElement stad in steden)
            {
                string naam_stad = Menu.CapitalizeFirstLetter(stad.FindElement(By.CssSelector("h4")).Text.ToLower());
                Console.WriteLine($"\nnaam van de stad: {naam_stad}");

                //nu elke halte van deze stad pakken
                IList<IWebElement> haltes_stad = stad.FindElements(By.CssSelector("ul[data-testid=\'stop-list-with-small-action\'] > li"));
                foreach (IWebElement halte in haltes_stad)
                {
                    //span die direct kind is van een div is onze titel van de halte
                    string naam_halte = halte.FindElement(By.CssSelector("div > span")).Text;
                    Console.WriteLine($"halte: {naam_halte}");

                    haltes_lijst.Add($"{naam_stad} - {naam_halte}");
                }
            }

            Console.Clear();
            Menu.PrintCustomMenu("Alle haltes op deze route", haltes_lijst, "Kies een halte");
            string halte_option = Menu.GetOptionNew(haltes_lijst.Count);
            int halte_option_int = int.Parse(halte_option);

            IList<IWebElement> alle_haltes = driver.FindElements(By.CssSelector("ul[data-testid=\'stop-list-with-small-action\'] > li"));
            alle_haltes[halte_option_int].FindElement(By.CssSelector("a")).Click();

            //nu zitten we op de pagina van de halte, met de datum en tijd die we willen aanpassen.
            //klik op knop om tijd aan te passen
            driver.FindElement(By.CssSelector("button[data-testid=\'slideover-selector-button-stop-detail-date\']")).Click();

            //nu kunnen we de inputs van de forum pakken die date en time aanpassen
            WebElement date_picker = (WebElement)driver.FindElement(By.CssSelector("input[data-testid=\'date-picker\'")); //type = date
            WebElement time_picker = (WebElement)driver.FindElement(By.CssSelector("input[data-testid=\'time-picker\'")); //type = time

            //huidige datum
            DateTime nu = DateTime.Now;

            //nu aan user vragen om datum in te geven, hoe ga ik dit doen?!
            //eerst datum vragen aan persoon. ik ga al wat predefined datums meegeven, maar geef ook optie om zelf te kiezen
            List<string> datums = new List<string> { };
            //aantal datums maken om uit te kiezen
            for(int i = 0; i <= 6; i++)
            {
                datums.Add($"{nu.AddDays(i).Day}-{nu.AddDays(i).Month}-{nu.AddDays(i).Year}");
            }
            //alle datums tonen + optie om zelf een datum te schrijven
            Console.Clear();
            Menu.PrintCustomMenu("Kies een datum", datums, "s: Schrijf zelf een datum (format: dd-mm-yyyy)");
            string date_option_picked = Menu.GetOptionNew(datums.Count, extra_option: "s");

            //kijken welke optie, zelf 1 laten schrijven als gekozen
            string date_picked;
            if(date_option_picked == "s")
            {
                Console.Write("Eigen datum: ");
                date_picked = Console.ReadLine();
            } else
            {
                date_picked = datums[int.Parse(date_option_picked)];
            }
            //date invullen
            date_picker.SendKeys(date_picked);

            //nu voor het uur, zelfde als date maar dan met uur
            List<string> uren = new List<string> { };
            //aantal uren maken om uit te kiezen
            for (int i = 0; i <= 6; i++)
            {
                uren.Add($"{nu.AddHours(i).Hour.ToString().PadLeft(2, '0')}:{nu.AddHours(i).Minute.ToString().PadLeft(2, '0')}");
            }
            //alle uren tonen + optie om zelf een uur te schrijven
            Console.Clear();
            Menu.PrintCustomMenu("Kies een uur", uren, "s: Schrijf zelf een uur (format: hh:mm)");
            string uur_option_picked = Menu.GetOptionNew(datums.Count, extra_option: "s");

            string uur_picked;
            if (uur_option_picked == "s")
            {
                Console.Write("Eigen uur: ");
                uur_picked = Console.ReadLine();
            }
            else
            {
                uur_picked = uren[int.Parse(uur_option_picked)];
            }
            //uur invullen en form submitten
            time_picker.SendKeys(uur_picked);
            time_picker.Submit();

            //nu nog enkel de data ophalen :D
            //eerst wat meer doorkomsten opvragen
            try
            {
                driver.FindElement(By.CssSelector("button[data-testid=\'load-more-recent-stops-button\']")).Click();
                //even wachten tot ze geladen zijn
                Thread.Sleep(1000);
            } catch (OpenQA.Selenium.NoSuchElementException) { }

            string halte_naam = driver.FindElement(By.CssSelector("h1")).Text;

            List<BusDoorkomst> bus_doorkomsten = new List<BusDoorkomst>();

            IList<IWebElement> doorkomsten = driver.FindElements(By.CssSelector("ul li ul li"));
            foreach(IWebElement doorkomst in doorkomsten)
            {
                string busnummer = doorkomst.FindElement(By.CssSelector("span div > span")).Text;
                //route is een ambetante, omdat het een div is met nog een span in met de busnummer, dus ga ik eerst de hele html pakken van dat element
                //en dan splitten op > zodat ik dan de tekst die overblijft kan pakken
                string route_html = doorkomst.FindElement(By.CssSelector("a div div div:nth-child(2) div div span")).GetAttribute("innerHTML");
                string[] splitsing = route_html.Split('>');
                string route_naam = splitsing[splitsing.Length - 1];
                //
                string meer_info = doorkomst.FindElement(By.CssSelector("a")).GetAttribute("href");
                string tijdstip = doorkomst.FindElement(By.CssSelector("span[data-testid=\'realtime-display-theoretic-time\']")).Text;

                Console.WriteLine($"busnr: {busnummer}\nroute_naam = {route_naam}\nhaltenaam= {halte_naam}\ndatum={date_picked}\ntijdstip = {tijdstip}\ninfolink = {meer_info}");
                bus_doorkomsten.Add(new BusDoorkomst(busnummer, route_naam, halte_naam, date_picked, tijdstip, meer_info));
            }
            

            Console.ReadLine();
            driver.Quit();
        }
    }
}
