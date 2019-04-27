using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TidyManaged;

namespace HtmlFormatterTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var client = new RestClient("https://steamcommunity.com/workshop/browse/?appid=4000");
            string dirtyHtml = client.Get(new RestRequest()).Content;

            string cleanHtml = CleanHtml(dirtyHtml);

            string filePath = Path.Combine(Assembly.GetExecutingAssembly().Location, "test.html");
            File.WriteAllText(filePath, cleanHtml);

            Console.Read();
        }

        private static string CleanHtml(string dirtyHtml)
        {
            using (Document doc = Document.FromString(dirtyHtml))
            {
                doc.OutputBodyOnly = AutoBool.Yes;
                doc.Quiet = true;
                doc.CleanAndRepair();

                return doc.Save();
            }
        }
    }
}