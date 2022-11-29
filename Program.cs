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
        public static void PrintCustomMenu(string title, string[] main, string endline)
        {
            string[] allStrings = main.Concat(new string[] { title, endline }).ToArray();
            int longestString = allStrings.Max(w => w.Length);

            title = title.PadRight(longestString + 4);
            endline = endline.PadRight(longestString + 4);

            string streep = new string('-', longestString + 5);

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write($"|{streep}|\n|");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" {title}");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write($"|\n|{streep}|\n|");

            for(int i = 0; i < main.Count(); i++)
            {
                string line = $" {i}: {main[i]}";
                line = line.PadRight(longestString + 4);

                Console.ForegroundColor = ConsoleColor.White;

                Console.Write(line);
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.Write($" |\n|");
            }

            Console.Write($"{streep}|\n|");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" {endline}");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"|\n|{streep}|");
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void PrintMenu()
        {
            //attempt keuzemenu :)
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write("|-----------------------|\n|");
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
            Console.Write($"|\n|");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" 3: Scrape 10 weather  ");
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
            string[] strings = { "Scrape 5 YT videos", "Scrape 5 ictjobs", "Scrape weer van de komende 12 dagen" };
            Console.Clear();
            PrintCustomMenu("Kies een optie", strings, "q: Quit programma");
            //PrintMenu();

            string[] options = { "0", "1", "2", "q" };

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

        static void Main(string[] args)
        {

            /*dit haalt de huidige dir op, zonder de bin en debugmappen. hier kan ik dan de csv en json bestanden naar zetten*/
            string exportDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + @"\Exports\";

            void Program()
            {
                string option = GetOption();

                switch (option)
                {
                    case "0":
                        List<YoutubeVideo> YoutubeVideosList = Scraper.YoutubeScraper();
                        Converter.ExportCSV(exportDirectory, "YoutubeVids", YoutubeVideosList);
                        Converter.ExportJSON(exportDirectory, "YoutubeVids", YoutubeVideosList);
                        Program();
                        break;
                    case "1":
                        List<Vacature> VacaturesList = Scraper.VacatureScraper();
                        Converter.ExportCSV(exportDirectory, "Vacatures", VacaturesList);
                        Converter.ExportJSON(exportDirectory, "Vacatures", VacaturesList);
                        Program();
                        break;
                    case "2":
                        List<DailyForecast> DailyForecastList = Scraper.WeatherScraper();
                        Converter.ExportCSV(exportDirectory, "Weather", DailyForecastList);
                        Converter.ExportJSON(exportDirectory, "Weather", DailyForecastList);
                        Program();
                        break;
                    case "q":
                        break;
                    default:
                        break;
                }
            }

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