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
        public static void ExportCSV_original(string path, string filename, List<YoutubeVideo> items)
        {
            string pathCSV = path + @"\" + $"{filename}.csv";
            string csvString = CsvSerializer.SerializeToString(items);
            File.WriteAllText(pathCSV, csvString);
        }
        public static void ExportCSV<T>(string path, string filename, T items)
        {
            string pathCSV = path + @"\" + $"{filename}.csv";
            string csvString = CsvSerializer.SerializeToString(items);
            File.WriteAllText(pathCSV, csvString);
        }
        public static void ExportJSON<T>(string path, string filename, T items)
        {
            string pathCSV = path + @"\" + $"{filename}.json";
            string jsonString = JsonSerializer.SerializeToString(items);
            File.WriteAllText(pathCSV, jsonString);
        }
    }
}
