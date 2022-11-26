using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace CaseStudy
{
    public class YoutubeScraper
    {

        public List<YoutubeVideo> GetList()
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

                YoutubeVideosList.Add(new YoutubeVideo(link, title, channel, views));
            }

            driver.Quit();

            return YoutubeVideosList;
        }
    }
}
