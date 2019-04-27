using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

            if (!string.IsNullOrEmpty(cleanHtml))
            {
                File.WriteAllText(filePath, cleanHtml);
                Console.WriteLine("Succesfully formatted HTML!");
            }
            else
                Console.WriteLine("There was a problem parsing the HTML!");

            Console.Read();
        }

        private static string CleanHtml(string dirtyHtml)
        {
            //try
            //{
            //    return System.Xml.Linq.XElement.Parse(dirtyHtml).ToString();
            //}
            //catch (Exception ex)
            //{
            //    // isn't well-formed xml
            //    Console.WriteLine(ex.ToString());
            //    return string.Empty;
            //}

            var tidy = new TidyNet.Tidy();
            tidy.Options.SmartIndent = true;
            tidy.Options.IndentAttributes = false;
            tidy.Options.WrapLen = 0;
            tidy.Options.Spaces = 4;
            //tidy.Options.XmlOut = false;
            //tidy.Options.XmlTags = false;
            //tidy.Options.Xhtml = false;

            //tidy.Options.WrapLen = 0;

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

        //public static String PrettyPrint(String XML)
        //{
        //    String Result = "";

        //    using (MemoryStream MS = new MemoryStream())
        //    {
        //        using (XmlTextWriter W = new XmlTextWriter(MS, Encoding.Unicode))
        //        {
        //            XmlDocument D = new XmlDocument();

        //            try
        //            {
        //                // Load the XmlDocument with the XML.
        //                D.LoadXml(XML);

        //                W.Formatting = Formatting.Indented;

        //                // Write the XML into a formatting XmlTextWriter
        //                D.WriteContentTo(W);
        //                W.Flush();
        //                MS.Flush();

        //                // Have to rewind the MemoryStream in order to read
        //                // its contents.
        //                MS.Position = 0;

        //                // Read MemoryStream contents into a StreamReader.
        //                StreamReader SR = new StreamReader(MS);

        //                // Extract the text from the StreamReader.
        //                String FormattedXML = SR.ReadToEnd();

        //                Result = FormattedXML;
        //            }
        //            catch (XmlException ex)
        //            {
        //                Result = ex.ToString();
        //            }

        //            W.Close();
        //        }
        //        MS.Close();
        //    }
        //    //Debug.WriteLine(Result);
        //    return Result;
        //}
    }
}