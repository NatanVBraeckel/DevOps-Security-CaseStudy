using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace CaseStudy
{
    public class Scraper
    {
        //oude versie
        public static List<YoutubeVideo> YoutubeScraper()
        {
            Console.Write("enter searchterm: ");
            string searchterm = Console.ReadLine();

            WebDriver driver = new ChromeDriver();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            driver.Navigate().GoToUrl($"https://www.youtube.com/results?search_query={searchterm.Replace(' ', '+')}&sp=CAI%253D");

            WebElement acceptTerms = (WebElement)wait.Until(driver => driver.FindElement(By.XPath("//*[@id=\"content\"]/div[2]/div[6]/div[1]/ytd-button-renderer[2]/yt-button-shape/button/yt-touch-feedback-shape/div/div[2]")));
            acceptTerms.Click();

            /*3 seconden slapen, anders krijg je errors omdat pagina nog niet herladen is na accept terms*/
            Thread.Sleep(3000);

            List<YoutubeVideo> youtubeVideosList = new List<YoutubeVideo>();
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
                ////*[@id="metadata-line"]/span[1]
                /*Console.WriteLine($"link = {link}\ntitle = {title}\nchannel = {channel}\nviews = {views}\n");*/

                youtubeVideosList.Add(new YoutubeVideo(title, channel, views, link));
            }

            driver.Quit();

            return youtubeVideosList;
        }
        public static List<YoutubeVideo> YoutubeScraper2()
        {
            Console.Write("enter searchterm: ");
            string searchterm = Console.ReadLine();
            //string searchterm = "cute cat videos";

            WebDriver driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

            //&sp=CAI%253D wordt altijd in de url gezet als je op meest recente vids sorteerd
            driver.Navigate().GoToUrl($"https://www.youtube.com/results?search_query={searchterm.Replace(' ', '+')}&sp=CAI%253D");

            //accept terms klikken
            string xpathAcceptTerms = "//*[@id=\"content\"]/div[2]/div[6]/div[1]/ytd-button-renderer[2]/yt-button-shape/button/yt-touch-feedback-shape/div/div[2]";
            driver.FindElement(By.XPath(xpathAcceptTerms)).Click();

            //even wachten anders StaleElementException
            Thread.Sleep(1500);

            List<YoutubeVideo> youtubeVideosList = new List<YoutubeVideo>();
            //nu de eerste 5 vids uit de lijst pakken, dit werkt ook als er bv maar 3 videos waren
            IList<IWebElement> videosLijst = driver.FindElements(By.CssSelector("#contents ytd-video-renderer:nth-of-type(-n+5)"));
            foreach(IWebElement video in videosLijst)
            {
                //Console.WriteLine(video);
                string title = video.FindElement(By.CssSelector("#video-title yt-formatted-string")).Text;
                string channel = video.FindElement(By.CssSelector("#metadata a")).GetAttribute("innerHTML");

                //deze gaf problemen bij de vorige, de html zag er toen anders uit
                string views = video.FindElement(By.CssSelector("#metadata-line span")).Text;
                if (views == "Geen weergaven") { views = "0"; }
                else { views = views.Substring(0, views.IndexOf(' ')); }

                string link = video.FindElement(By.CssSelector("ytd-thumbnail a")).GetAttribute("href");

                //Console.WriteLine($"Title = {title}\nChannel = {channel}\nViews = {views}\nLink = {link}");
                youtubeVideosList.Add(new YoutubeVideo(title, channel, views, link));
            }

            driver.Quit();
            return youtubeVideosList;
        }
        public static List<Vacature> VacatureScraper()
        {
            Console.Write("Geef zoekterm in: ");
            string searchterm = Console.ReadLine();

            WebDriver driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            /*WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20)); oud, explicit wait*/

            driver.Navigate().GoToUrl("https://www.ictjob.be/");


            WebElement searchbar = (WebElement)driver.FindElement(By.Id("keywords-input"));
            searchbar.SendKeys($"{searchterm}");
            searchbar.Submit();

            WebElement cookies = (WebElement)driver.FindElement(By.CssSelector(".close-context-message"));
            cookies.Click();

            WebElement sortDateButton = (WebElement)driver.FindElement(By.Id("sort-by-date"));
            sortDateButton.Click();

            Thread.Sleep(10000);

            IList<IWebElement> vacatures = driver.FindElements(By.CssSelector($"li.search-item.clearfix"));
            List<IWebElement> fiveVacatures = vacatures.ToList().GetRange(0, 5);

            List<Vacature> vacaturesList = new List<Vacature>();

            foreach (IWebElement vacature in fiveVacatures)
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

                vacaturesList.Add(new Vacature(title, bedrijf, locatie, keywords, link));
                //Console.WriteLine($"titel = {title}\nbedrijf = {bedrijf}\nlocatie = {locatie}\nkeywords = {keywords}\nlink = {link}\n");
            }

            driver.Quit();
            return vacaturesList;
        }
        public static List<DailyForecast> WeatherScraper()
        {
            Console.Write("Geef locatie in: ");
            string chosenLocation = Console.ReadLine();

            WebDriver driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            /*WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20)); oud, explicit wait*/

            driver.Navigate().GoToUrl("https://www.accuweather.com/");

            WebElement consent = (WebElement)driver.FindElement(By.CssSelector(".fc-button.fc-cta-consent.fc-primary-button"));
            consent.Click();

            WebElement searchbar = (WebElement)driver.FindElement(By.ClassName("search-input"));
            searchbar.SendKeys(chosenLocation);
            searchbar.Submit();
            //hierna ga je oftewel direct naar de juiste pagina (bv Averbode), of je gaat naar pagina met verschillende opties (bv Geel)

            //moet dus eerst checken of we al direct op de juiste pagina zitten of niet?
            //=> als de dailyknop er is weten we dat we juist zitten
            WebElement daily;
            {
                try
                {
                    daily = (WebElement)driver.FindElement(By.CssSelector("a[data-qa=\'daily\']"));
                    daily.Click();
                    //soms krijg je nog advertentie eens je op de pagina terechtkomt, door refresh gaat die weg
                    driver.Navigate().Refresh();
                }
                catch (OpenQA.Selenium.NoSuchElementException) //als er geen dailyknop is:
                {
                    //haal alle opties uit de lijst
                    IList<IWebElement> locations = driver.FindElements(By.CssSelector(".locations-list a"));
                    List<string> locationsStrings = new List<string> { };
                    foreach (IWebElement location in locations)
                    {
                        locationsStrings.Add(location.Text);
                    }

                    //print alle opties
                    Console.Clear();
                    Menu.PrintCustomMenu($"{locations.Count} options found", locationsStrings, "Choose a location");
                    

                    string locationChoice = Menu.GetOption(locationsStrings.Count);
                    int intLocationChoice = int.Parse(locationChoice);
                    locations[intLocationChoice].Click();

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
                        IList<IWebElement> locationsReload = driver.FindElements(By.CssSelector(".locations-list a"));
                        locationsReload[intLocationChoice].Click();
                    }
                }
            }

            //soms komt er een advertentie... maar door refresh gaat die weg
            //stel we komen van iets dat geen verschillende keuzes had, staan we wel al op juiste pagina voordat we ad krijgen, door refresh gaat die dan weg
            //driver.Navigate().Refresh();
            //normaal gezien zijn we nu op de juiste pagina

            //WebElement daily = (WebElement)wait.Until(driver => driver.FindElement(By.CssSelector("a[data-qa=\'daily\']")));
            daily = (WebElement)driver.FindElement(By.CssSelector("a[data-qa=\'daily\']"));
            daily.Click();

            List<DailyForecast> dailyForecasts = new List<DailyForecast>();

            Console.Write("Hoeveel dagen wil je scrapen (1 - 12): ");
            string aantalDagen = Console.ReadLine();
            while (Menu.IsInputIntInRange(aantalDagen, 1, 12) == false)
            {
                Console.Write("Foute input (1 - 12): ");
                aantalDagen = Console.ReadLine();
            }

            //irritante html zorgt ervoor dat we de selector moeten aanpassen als user meer dan 4 dagen wilt scrapen
            if (int.Parse(aantalDagen) > 4)
            {
                aantalDagen = $"{int.Parse(aantalDagen) + 3}";
            }

            IList<IWebElement> dailyWrappers = driver.FindElements(By.CssSelector($"div.daily-wrapper:nth-of-type(-n+{aantalDagen})"));
            foreach (IWebElement dailyWrapper in dailyWrappers)
            {
                //je kan de hele date ophalen, maar data opsplitsen kan misschien ergens handig zijn
                string date = dailyWrapper.FindElement(By.CssSelector(".module-header.sub.date")).Text;
                string[] seperatedDate = date.Split('-');
                string day = seperatedDate[0];
                string month = seperatedDate[1];

                string temp = dailyWrapper.FindElement(By.CssSelector(".temp .high")).Text;
                string phrase = dailyWrapper.FindElement(By.CssSelector(".phrase")).Text;
                string rain = dailyWrapper.FindElement(By.CssSelector(".precip")).Text;
                //Console.WriteLine($"day = {day}\nmonth={month}\ntemp = {temp}\nphrase = {phrase}\nrain = {rain}");
                dailyForecasts.Add(new DailyForecast(day, month, temp.Replace("°", ""), phrase, rain));
            }

            driver.Quit();
            return dailyForecasts;
        }
        public static List<BusDoorkomst> LijnScraper()
        {
            Console.Write("Geef een lijnnummer in: ");
            string lijnNummer = Console.ReadLine();

            WebDriver driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

            driver.Navigate().GoToUrl("https://www.delijn.be/");

            /*WebElement consent = (WebElement)driver.FindElement(By.CssSelector("#onetrust-accept-btn-handler"));
            consent.Click();*/
            driver.FindElement(By.CssSelector("#onetrust-accept-btn-handler")).Click();

            //soms komt er een message als er een staking is
            try
            {
                WebElement popup = (WebElement)driver.FindElement(By.CssSelector("button[data-testid=\'snackbar-close-button\']"));
                popup.Click();
            }
            catch (OpenQA.Selenium.NoSuchElementException)
            {}

            WebElement zoek = (WebElement)driver.FindElement(By.CssSelector("a[title=\'Ga naar de zoekpagina\']"));
            driver.Navigate().GoToUrl(zoek.GetAttribute("href"));

            WebElement ingeefBalk = (WebElement)driver.FindElement(By.CssSelector("input[data-testid=\'search-input\']"));
            ingeefBalk.SendKeys(lijnNummer);
            ingeefBalk.Submit();

            //nu zitten we op de pagina met alle resultaten van de zoekopdracht naar lijnnummer,
            //pak alle routes en laat de user kiezen

            List<string> lijstRoutes = new List<string>();

            string routesListSelector = "ul[data-testid =\'search-results-list\'] > li:not([data-testid*=\'null\'])";
            IList<IWebElement> resultaten = driver.FindElements(By.CssSelector(routesListSelector));
            foreach (IWebElement resultaat in resultaten)
            {
                string nummerRoute = resultaat.FindElement(By.CssSelector("a div div:nth-child(1) span")).Text;
                string naamRoute = resultaat.FindElement(By.CssSelector("a div div:nth-child(2) span")).Text;
                string stringRoute = $"[{nummerRoute}] {naamRoute}";

                lijstRoutes.Add(stringRoute);
            }

            //alle opties zijn nu opgehaald, nu nog de optie van de gebruiker krijgen
            Console.Clear();
            Menu.PrintCustomMenu($"Alle lijnen met nummer {lijnNummer}", lijstRoutes, "Kies een route");
            string routeOption = Menu.GetOption(lijstRoutes.Count);

            //op de optie die de gebruiker heeft gekozen klikken
            int intRouteOption = Int32.Parse(routeOption);
            resultaten[intRouteOption].FindElement(By.CssSelector("a")).Click();

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

            List<string> haltesLijst = new List<string>{};
            //per city haltes ophalen
            //eerst lijst van alle lijsten :p
            IList<IWebElement> steden = driver.FindElements(By.CssSelector("div[data-testid=\'grouped-stops-by-town\'] > ul > li"));
            foreach(IWebElement stad in steden)
            {
                string naamStad = Menu.CapitalizeFirstLetter(stad.FindElement(By.CssSelector("h4")).Text.ToLower());
                //Console.WriteLine($"\nnaam van de stad: {naamStad}");

                //nu elke halte van deze stad pakken
                IList<IWebElement> haltesStad = stad.FindElements(By.CssSelector("ul[data-testid=\'stop-list-with-small-action\'] > li"));
                foreach (IWebElement halte in haltesStad)
                {
                    //span die direct kind is van een div is onze titel van de halte
                    string naamHalte = halte.FindElement(By.CssSelector("div > span")).Text;
                    //Console.WriteLine($"halte: {naamHalte}");

                    haltesLijst.Add($"{naamStad} - {naamHalte}");
                }
            }

            Console.Clear();
            Menu.PrintCustomMenu("Alle haltes op deze route", haltesLijst, "Kies een halte");
            string halteOption = Menu.GetOption(haltesLijst.Count);
            int intHalteOption = int.Parse(halteOption);

            IList<IWebElement> alle_haltes = driver.FindElements(By.CssSelector("ul[data-testid=\'stop-list-with-small-action\'] > li"));
            string linkHalte = alle_haltes[intHalteOption].FindElement(By.CssSelector("a")).GetAttribute("href");
            driver.Navigate().GoToUrl(linkHalte);

            //nu zitten we op de pagina van de halte, met de datum en tijd die we willen aanpassen.
            //klik op knop om tijd aan te passen
            driver.FindElement(By.CssSelector("button[data-testid=\'slideover-selector-button-stop-detail-date\']")).Click();

            //nu kunnen we de inputs van de forum pakken die date en time aanpassen
            WebElement datePicker = (WebElement)driver.FindElement(By.CssSelector("input[data-testid=\'date-picker\'")); //type = date
            WebElement timePicker = (WebElement)driver.FindElement(By.CssSelector("input[data-testid=\'time-picker\'")); //type = time

            //huidige datum
            DateTime now = DateTime.Now;

            //nu aan user vragen om datum en tijd in te geven
            //eerst datum vragen aan persoon. ik ga al wat predefined datums meegeven, maar geef ook optie om zelf te kiezen
            List<string> datums = new List<string> { };
            //aantal datums maken om uit te kiezen
            for(int i = 0; i <= 6; i++)
            {
                datums.Add($"{now.AddDays(i).Day}-{now.AddDays(i).Month}-{now.AddDays(i).Year}");
            }
            //alle datums tonen + optie om zelf een datum te schrijven
            Console.Clear();
            Menu.PrintCustomMenu("Kies een datum", datums, "s: Schrijf zelf een datum (format: dd-mm-yyyy)");
            string dateOptionPicked = Menu.GetOption(datums.Count, extra_option: "s");

            //kijken welke optie, zelf 1 laten schrijven als gekozen
            string datePicked;
            if(dateOptionPicked == "s")
            {
                Console.Write("Eigen datum: ");
                datePicked = Console.ReadLine();
            } else
            {
                datePicked = datums[int.Parse(dateOptionPicked)];
            }
            //date invullen
            datePicker.SendKeys(datePicked);

            //nu voor het uur, zelfde als date maar dan met uur
            List<string> uren = new List<string> { };
            //aantal uren maken om uit te kiezen
            for (int i = 0; i <= 6; i++)
            {
                uren.Add($"{now.AddHours(i).Hour.ToString().PadLeft(2, '0')}:{now.AddHours(i).Minute.ToString().PadLeft(2, '0')}");
            }
            //alle uren tonen + optie om zelf een uur te schrijven
            Console.Clear();
            Menu.PrintCustomMenu("Kies een uur", uren, "s: Schrijf zelf een uur (format: hh:mm)");
            string uurOptionPicked = Menu.GetOption(datums.Count, extra_option: "s");

            string uur_picked;
            if (uurOptionPicked == "s")
            {
                Console.Write("Eigen uur: ");
                uur_picked = Console.ReadLine();
            }
            else
            {
                uur_picked = uren[int.Parse(uurOptionPicked)];
            }
            //uur invullen en form submitten
            timePicker.SendKeys(uur_picked);
            timePicker.Submit();

            //nu nog enkel de data ophalen :D
            //eerst wat meer doorkomsten opvragen
            try
            {
                Thread.Sleep(500);
                driver.FindElement(By.CssSelector("button[data-testid=\'load-more-recent-stops-button\']")).Click();
                //even wachten tot ze geladen zijn
                Thread.Sleep(1000);
            } catch (OpenQA.Selenium.NoSuchElementException) { }

            string halteNaam = driver.FindElement(By.CssSelector("h1")).Text;

            List<BusDoorkomst> busDoorkomsten = new List<BusDoorkomst>();

            IList<IWebElement> doorkomsten = driver.FindElements(By.CssSelector("ul li ul li.list-none"));
            foreach(IWebElement doorkomst in doorkomsten)
            {
                Console.WriteLine(doorkomst.Text);

                string busnummer = doorkomst.FindElement(By.CssSelector("span > div > span")).Text; ;
                //route is een ambetante, omdat het een div is met nog een span in met de busnummer, dus ga ik eerst de hele html pakken van dat element
                //en dan splitten op > zodat ik dan de tekst die overblijft op het einde kan pakken
                string routeHtml = doorkomst.FindElement(By.CssSelector("a div div div:nth-child(2) div div span")).GetAttribute("innerHTML");
                string[] splitsing = routeHtml.Split('>');
                string routeNaam = splitsing[splitsing.Length - 1];
                //
                string meerInfoLink = doorkomst.FindElement(By.CssSelector("a")).GetAttribute("href");
                string tijdstip = doorkomst.FindElement(By.CssSelector("span[data-testid=\'realtime-display-theoretic-time\']")).Text;


                //Console.WriteLine($"busnr: {busnummer}\nroute_naam = {route_naam}\nhaltenaam= {halte_naam}\ndatum={date_picked}\ntijdstip = {tijdstip}\ninfolink = {meer_info}");
                busDoorkomsten.Add(new BusDoorkomst(busnummer, routeNaam, halteNaam, datePicked, tijdstip, meerInfoLink));
                
            }           

            driver.Quit();
            return busDoorkomsten;
        }
    }
}
