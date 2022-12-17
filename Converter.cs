using ServiceStack.Text;
/*using JsonSerializer = System.Text.Json.JsonSerializer; NIET MEER NODIG OMDAT JSSERIALIZER OOK IN SERVICESTACK.TEXT ZIT*/

namespace CaseStudy
{
    public class Converter
    {
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
