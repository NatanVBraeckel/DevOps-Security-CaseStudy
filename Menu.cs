using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseStudy
{
    public class Menu
    {
        public static void PrintCustomMenu(string title, List<string> main, string endline)
        {
            string[] allStrings = main.Concat(new string[] { title, endline }).ToArray();
            int longestString = allStrings.Max(w => w.Length);

            title = title.PadRight(longestString + 5);
            endline = endline.PadRight(longestString + 5);

            string streep = new string('-', longestString + 6);

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write($"|{streep}|\n|");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" {title}");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write($"|\n|{streep}|\n|");

            for (int i = 0; i < main.Count(); i++)
            {
                string line = $" {i}: {main[i]}";
                line = line.PadRight(longestString + 5);

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
            List<string> strings = new List<string>{ "Scrape 5 YT videos", "Scrape 5 ictjobs", "Scrape weer van de komende 12 dagen" };
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
        public static string GetOptionNew(int amount_options, string extra_option = "", string prompt = "Optie: ")
        {
            List<string> options = new List<string>();
            for (int i = 0; i < amount_options; i++)
            {
                options.Add($"{i}");
            }

            if (extra_option != "")
            {
                options.Add(extra_option);
            }

            Console.Write(prompt);
            string option = Console.ReadLine();

            while (!options.Contains(option))
            {
                Console.Write("Foute optie, kies opnieuw: ");
                option = Console.ReadLine();
            }

            return option;
        }

        //yoinked van https://stackoverflow.com/questions/4135317/make-first-letter-of-a-string-upper-case-with-maximum-performance
        public static string CapitalizeFirstLetter(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            if (s.Length == 1)
                return s.ToUpper();
            return s.Remove(1).ToUpper() + s.Substring(1);
        }
    }
}
