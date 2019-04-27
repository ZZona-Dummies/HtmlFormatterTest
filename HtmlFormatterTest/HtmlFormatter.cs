//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace HtmlFormatterTest
//{
//    public class HtmlFormatter : IFormatter
//    {
//        private string _contents;
//        private Hashtable _tagMap = new Hashtable();
//        private ArrayList _singleTags = new ArrayList();
//        private string _urlPlaceholder = "[** URL_ROOT **]";

//        public HtmlFormatter(string content)
//        {
//            this._contents = content;
//            this.Setup();
//        }

//        private void Setup()
//        {
//            // create a lookup table for tags:
//            // key = tag
//            // -1 = strip tag and contents inside tag completely
//            //  0 = allow tag, no attributes
//            //  1 = allow tag with attributes
//            //  N.B., all other tags should be stripped
//            this._tagMap.Add("head", -1);
//            this._tagMap.Add("select", -1);
//            this._tagMap.Add("input", -1);
//            this._tagMap.Add("script", -1);
//            this._tagMap.Add("noscript", -1);
//            this._tagMap.Add("xmp", -1);
//            this._tagMap.Add("style", -1);
//            this._tagMap.Add("a", 1);
//            this._tagMap.Add("table", 1);
//            this._tagMap.Add("tr", 1);
//            this._tagMap.Add("th", 1);
//            this._tagMap.Add("td", 1);
//            this._tagMap.Add("ul", 0);
//            this._tagMap.Add("ol", 0);
//            this._tagMap.Add("li", 0);
//            this._tagMap.Add("p", 1);
//            this._tagMap.Add("xml", 1);
//            this._tagMap.Add("img", 1);
//            this._tagMap.Add("br", 0);
//            this._tagMap.Add("hr", 0);
//            this._tagMap.Add("b", 0);
//            this._tagMap.Add("strong", 0);
//            this._tagMap.Add("i", 0);
//            this._tagMap.Add("u", 0);
//            this._tagMap.Add("strike", 0);
//            this._tagMap.Add("sup", 0);
//            this._tagMap.Add("sub", 0);
//            this._tagMap.Add("iframe", 1);
//        }

//        #region IFormatter Members

//        //public string Render()
//        //{
//        //    // fix links
//        //    formatMe = this.ReplaceRelativeUrlPlaceholder(formatMe);

//        //    // find assets

//        //    return formatMe;
//        //}

//        //public string Clean()
//        //{
//        //    string formatMe = this.CleanTags(this._contents);
//        //    return formatMe;
//        //}

//        #endregion IFormatter Members

//        #region HTML Markup Handling

//        //private string InsertRelativeUrlPlaceholder(string input)
//        //{
//        //    string formatMe = input;
//        //    string searchTerm = System.Configuration.ConfigurationManager.AppSettings["URL_ROOT"];

//        //    if (formatMe.IndexOf(searchTerm) > -1)
//        //    {
//        //        Regex reg = new Regex(searchTerm);
//        //        MatchCollection matches = reg.Matches(formatMe);
//        //        foreach (Match m in matches)
//        //        {
//        //            formatMe = formatMe.Replace(m.ToString(), this._urlPlaceholder);
//        //        }
//        //    }
//        //    return formatMe;
//        //}

//        //private string ReplaceRelativeUrlPlaceholder(string input)
//        //{
//        //    string formatMe = input;
//        //    formatMe = formatMe.Replace(this._urlPlaceholder, System.Configuration.ConfigurationManager.AppSettings["URL_ROOT"]);
//        //    return formatMe;
//        //}

//        private string TidyHTML(string input)
//        {
//            Tidy.Document doc = new Tidy.Document();

//            //doc.OnMessage += new Tidy.IDocumentEvents_OnMessageEventHandler(TidyDiagnostics);

//            // set some options
//            doc.SetOptBool(TidyOptionId.TidyBodyOnly, 1);
//            doc.SetOptBool(TidyOptionId.TidyXhtmlOut, 1);
//            doc.SetOptBool(TidyOptionId.TidyWord2000, 1);
//            doc.SetOptValue(TidyOptionId.TidyIndentContent, "auto");

//            int err_code = doc.ParseString(input);
//            if (err_code < 0)
//            {
//                throw new Exception("Unable to parse string: " + input);
//            }

//            err_code = doc.CleanAndRepair();

//            if (err_code < 0)
//            {
//                throw new Exception("Unable to clean/repair string: " + input);
//            }

//            //err_code = doc.RunDiagnostics();

//            //if (err_code < 0)
//            //{
//            //    throw new Exception("Unable to run diagnostics on: " + input);
//            //}

//            return (doc.SaveString().Trim());
//        }

//        //public void TidyDiagnostics(TidyATL.TidyReportLevel level, int line, int col, string message)
//        //{
//        //    Console.WriteLine("Tidy diagnostic message: " + message);
//        //}

//        private int InStrEndOfTag(string input)
//        {
//            bool attr = false;
//            int pos = 0;

//            while (pos < input.Length)
//            {
//                pos++;
//                if (!attr && (input.Substring(pos, 1) == ">"))
//                {
//                    return pos;
//                }

//                if (input.Substring(pos, 1) == "")
//                {
//                    attr = !attr;
//                }
//            }

//            return pos;
//        }

//        private string RemoveExtraTags(string input)
//        {
//            string temp = input;
//            string output = "";
//            int pos;
//            string tag, name;

//            while (temp != "")
//            {
//                if (temp.Substring(0, 1) == "<")
//                {
//                    pos = InStrEndOfTag(temp);
//                    if (pos == 0)
//                    {
//                        tag = temp.Substring(2);
//                        temp = "";
//                    }
//                    else
//                    {
//                        tag = temp.Substring(1, pos - 1);
//                        temp = temp.Substring(pos + 1);
//                    }

//                    name = tag.Split(new Char[] { ' ' })[0].ToLower();

//                    if (name.Substring(0, 1) == "/")
//                    {
//                        name = name.Substring(1);
//                    }

//                    if (this._tagMap.Contains(name))
//                    {
//                        int val = Convert.ToInt32(this._tagMap[name].ToString());
//                        switch (val)
//                        {
//                            case -1:
//                                pos = temp.ToLower().IndexOf("</" + name + ">");
//                                if (pos > 0)
//                                {
//                                    temp = temp.Substring(pos + name.Length + 3);
//                                }
//                                break;

//                            case 0:
//                                output += "<";
//                                if (tag.Substring(0, 1) == "/")
//                                {
//                                    output += "/";
//                                }
//                                output += name + ">";
//                                break;

//                            case 1:
//                                output += "<" + tag + ">";
//                                break;

//                            default:
//                                break;
//                        }
//                    }
//                }
//                else
//                {
//                    output += temp.Substring(0, 1);
//                    temp = temp.Substring(1);
//                }
//            }

//            return output;
//        }

//        private string CleanTags(string input)
//        {
//            // run HTML Tidy on content
//            string formatMe = this.TidyHTML(input.Trim());

//            // get rid of comments first to make tag balancing a little easier
//            formatMe = this.StripComments(formatMe);

//            // remove attributes that are unacceptable in any case (e.g., JavaScript attributes, CSS)
//            formatMe = this.ReplaceNastyAttributes(formatMe);

//            formatMe = RemoveExtraTags(formatMe);

//            // substitute placeholder for relative links
//            //formatMe = this.InsertRelativeUrlPlaceholder(formatMe);

//            return formatMe;
//        }

//        private string StripComments(string input)
//        {
//            Regex regex = new Regex("<!--.*-->");
//            return regex.Replace(input, "");
//        }

//        private string ReplaceNastyAttributes(string input)
//        {
//            Regex regex = new Regex("( on[a-z]{1,}|style|id)=[\"'](.*?)[\"']");
//            return regex.Replace(input, "");
//        }

//        #endregion HTML Markup Handling
//    }
//}