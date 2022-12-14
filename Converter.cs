using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseStudy
{
    public class Converter
    {
        //de eerste methode was prototype, kon enkel een list van 1 soort meegeven
        public static void ExportCSV_original(string path, string filename, List<YoutubeVideo> objects)
        {
            string pathCSV = path + @"\" + $"{filename}.csv";
            string csvString = CsvSerializer.SerializeToString(objects);
            File.WriteAllText(pathCSV, csvString);
        }
        public static void ExportCSV<T>(string path, string filename, T objects)
        {
            string pathCSV = path + @"\" + $"{filename}.csv";
            string csvString = CsvSerializer.SerializeToString(objects);
            File.WriteAllText(pathCSV, csvString);
        }
        public static void ExportJSON<T>(string path, string filename, T objects)
        {
            string pathCSV = path + @"\" + $"{filename}.json";
            string jsonString = JsonSerializer.SerializeToString(objects);
            File.WriteAllText(pathCSV, jsonString);
        }
    }
}
