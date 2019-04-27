using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TidyManaged;

//using Tidy.Core;

//using HTMLTidy = Tidy.Core.Tidy;
using TidyNet;

namespace HtmlFormatterTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var client = new RestClient("https://steamcommunity.com/workshop/browse/?appid=4000");
            string dirtyHtml = client.Get(new RestRequest()).Content;

            string cleanHtml = CleanHtml(dirtyHtml);

            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "test.html");
            File.WriteAllText(filePath, cleanHtml);

            Console.Read();
        }

        private static string CleanHtml(string dirtyHtml)
        {
            var tidy = new TidyNet.Tidy();
            var messages = new TidyMessageCollection();

            using (var inStream = new MemoryStream(Encoding.Default.GetBytes(dirtyHtml)))
            using (var outStream = new MemoryStream())
            {
                tidy.Parse(inStream, outStream, messages);
                return Encoding.Default.GetString(outStream.ToArray());
            }

            //using (Document doc = Document.FromString(dirtyHtml))
            //{
            //    doc.OutputBodyOnly = AutoBool.Yes;
            //    doc.Quiet = true;
            //    doc.CleanAndRepair();

            //    return doc.Save();
            //}
        }
    }
}