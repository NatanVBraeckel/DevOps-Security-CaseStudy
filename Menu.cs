namespace CaseStudy
{
    public class Menu
    {
        //oude versie
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
        //oude versie
        public static string GetOption()
        {
            List<string> strings = new List<string> { "Scrape 5 YT videos", "Scrape 5 ictjobs", "Scrape weer van de komende 12 dagen" };
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
        public static void PrintLogo(int leftPadding = 0, ConsoleColor color = ConsoleColor.White)
        {
            string padding = new string(' ', leftPadding);
            Console.ForegroundColor = color;
            Console.WriteLine($"{padding}  __");
            Console.WriteLine($"{padding} / _\\ ___ _ __ __ _ _ __  _ __  _   _");
            Console.WriteLine($"{padding} \\ \\ / __| '__/ _` | '_ \\| '_ \\| | | |");
            Console.WriteLine($"{padding} _\\ \\ (__| | | (_| | |_) | |_) | |_| |");
            Console.WriteLine($"{padding} \\__/\\___|_|  \\__,_| .__/| .__/ \\__, |");
            Console.WriteLine($"{padding}                   |_|   |_|    |___/");
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void PrintCustomMenu(string title, List<string> main, string endline, ConsoleColor color = ConsoleColor.DarkBlue)
        {
            string[] allStrings = main.Concat(new string[] { title, endline }).ToArray();
            int longestString = allStrings.Max(w => w.Length);

            title = title.PadRight(longestString + 5);
            endline = endline.PadRight(longestString + 5);

            string streep = new string('-', longestString + 6);

            Console.ForegroundColor = color;
            Console.Write($"|{streep}|\n|");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" {title}");
            Console.ForegroundColor = color;
            Console.Write($"|\n|{streep}|\n|");

            for (int i = 0; i < main.Count(); i++)
            {
                string line = $" {i}: {main[i]}";
                line = line.PadRight(longestString + 5);

                Console.ForegroundColor = ConsoleColor.White;

                Console.Write(line);
                Console.ForegroundColor = color;
                Console.Write($" |\n|");
            }

            Console.Write($"{streep}|\n|");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" {endline}");
            Console.ForegroundColor = color;
            Console.WriteLine($"|\n|{streep}|");
            Console.ForegroundColor = ConsoleColor.White;
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
        public static ConsoleColor RandomColor()
        {
            var random = new Random();
            List<ConsoleColor> colors = new List<ConsoleColor>
            { ConsoleColor.Blue, ConsoleColor.DarkBlue, ConsoleColor.DarkGreen, ConsoleColor.Green, ConsoleColor.DarkCyan, ConsoleColor.Cyan, ConsoleColor.Red, ConsoleColor.Magenta };
            int index = random.Next(colors.Count);
            return colors[index];
        }
        public static void ConvertMenu<T>(string path, List<T>scrapeItems)
        {
            List<string> convertOptions = new List<string> { "View Data", "Convert to CSV", "Convert to JSON" };

            Console.Clear();
            PrintCustomMenu("Data retrieved", convertOptions, "q: Quit", color: ConsoleColor.Cyan);
            string option = GetOptionNew(convertOptions.Count, extra_option: "q");

            switch (option)
            {
                case "0":
                    foreach(object scrapeItem in scrapeItems)
                    {
                        Console.WriteLine($"{scrapeItem}\n");
                    }
                    Console.Write("Press key to continue");
                    Console.ReadLine();
                    ConvertMenu(path, scrapeItems);
                    break;
                case "1":
                    Console.Write("Write a name for the file: ");
                    string filenameCSV = Console.ReadLine();
                    Converter.ExportCSV(path, filenameCSV, scrapeItems);
                    ConvertMenu(path, scrapeItems);
                    break;
                case "2":
                    Console.Write("Write a name for the file: ");
                    string filenameJSON = Console.ReadLine();
                    Converter.ExportCSV(path, filenameJSON, scrapeItems);
                    ConvertMenu(path, scrapeItems);
                    break;
                case "q":
                    break;
                case "default":
                    break;
            }
        }
        public static bool IsInputIntInRange(string input, int min, int max)
        {
            int i;
            if (Int32.TryParse(input, out i))
            {
                if (min <= i && i <= max)
                {
                    return true;
                }
            }
            return false;
        }

        //code van https://stackoverflow.com/questions/4135317/make-first-letter-of-a-string-upper-case-with-maximum-performance
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
