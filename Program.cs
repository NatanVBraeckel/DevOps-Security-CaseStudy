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
using System.IO;
/*using JsonSerializer = System.Text.Json.JsonSerializer; NIET MEER NODIG OMDAT JSSERIALIZER OOK IN SERVICESTACK.TEXT ZIT*/

namespace CaseStudy
{
    class Program
    {

        static void Main(string[] args)
        {

            /*dit haalt de huidige dir op, zonder de bin en debugmappen. hier kan ik dan de csv en json bestanden naar zetten*/
            string exportDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + @"\Exports\";

            void Program()
            {
                Console.Clear();
                
                List<string> scrapeOpties = new List<string>{ "Scrape 5 YT videos", "Scrape 5 ictjobs", "Scrape weer van de komende 12 dagen", "Scrape doorkomende bussen van een halte" };
                Menu.PrintLogo(leftPadding: 4, color: Menu.RandomColor());
                Menu.PrintCustomMenu("Kies een optie", scrapeOpties, "q: Quit programma", color: Menu.RandomColor());
                string option = Menu.GetOptionNew(scrapeOpties.Count, extra_option: "q");

                switch (option)
                {
                    case "0":
                        List<YoutubeVideo> YoutubeVideosList = Scraper.YoutubeScraper2();
                        //Converter.ExportCSV(exportDirectory, "YoutubeVids", YoutubeVideosList);
                        //Converter.ExportJSON(exportDirectory, "YoutubeVids", YoutubeVideosList);
                        Menu.ConvertMenu(exportDirectory, YoutubeVideosList);
                        Program();
                        break;
                    case "1":
                        List<Vacature> VacaturesList = Scraper.VacatureScraper();
                        //Converter.ExportCSV(exportDirectory, "Vacatures", VacaturesList);
                        //Converter.ExportJSON(exportDirectory, "Vacatures", VacaturesList);
                        Menu.ConvertMenu(exportDirectory, VacaturesList);
                        Program();
                        break;
                    case "2":
                        List<DailyForecast> DailyForecastList = Scraper.WeatherScraper();
                        //Converter.ExportCSV(exportDirectory, "Weather", DailyForecastList);
                        //Converter.ExportJSON(exportDirectory, "Weather", DailyForecastList);
                        Menu.ConvertMenu(exportDirectory, DailyForecastList);
                        Program();
                        break;
                    case "3":
                        List<BusDoorkomst> BusDoorkomstList = Scraper.LijnScraper();
                        //Converter.ExportCSV(exportDirectory, "Busdoorkomsten", BusDoorkomstList);
                        //Converter.ExportJSON(exportDirectory, "Busdoorkomsten", BusDoorkomstList);
                        Menu.ConvertMenu(exportDirectory, BusDoorkomstList);
                        Program();
                        break;
                    case "q":
                        break;
                    default:
                        break;
                }
            }

            //Scraper.LijnScraper();

            //Scraper.VacatureScraper();

            //Scraper.YoutubeScraper2();

            Program();

            //string option = GetOption();

            /*
            string[] testing = { "optie een", "optie twee", "optie dit is een lange" };
            PrintCustomMenu("test voor lengte", testing, "eindzin");
            PrintCustomMenu("een te lange startzin dit gaat fout lopen", testing, "eindzin");
            PrintCustomMenu("test voor lengte", testing, "een te lange eindezin dit gaat ook fout lopen");
            */

            //hieronder deel van Youtube
            /*List<YoutubeVideo> YoutubeVideosList = YoutubeScraper();
          
            string pathJSON = currentDirectory + @"\YoutubeVids.json";
            string jsonString = JsonSerializer.SerializeToString(YoutubeVideosList);
            File.WriteAllText(pathJSON, jsonString);
            ExportCSV_original(currentDirectory, "YoutubeVids", YoutubeVideosList);*/


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