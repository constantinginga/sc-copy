using System.IO;
using System.Text;
using System.Xml;
//using TidyNet;

namespace StartupCentral.Extensions
{
    public static partial class StringExtensions
    {
        /// <summary>
        /// Shortens a HTML formatted string, while keeping HTML formatting and complete words (also removes line-breakes at the end of the shortened string)
        /// </summary>
        /// <param name="input">The HTML formatted string</param>
        /// <param name="inputIsShortened">Output boolean telling if the input string has been shortened</param>
        /// <param name="length">The approximate length of the output string (default: 300)</param>
        /// <param name="elipsis">Elipsis text to append to the output string (use string.Empty when elipsis should not be added, default: ...)</param>
        /// <returns>The shortened input string with HTML formatting</returns>
        //public static string ElipsisHtml(this string input, out bool inputIsShortened, int length = 300, string elipsis = "...")
        //{
        //    inputIsShortened = false;

        //    if (input.Length <= length)
        //        return input;

        //    input = input.Replace("<br />", "<br/>");

        //    string substring = input.Substring(0, length);
        //    string leftover = input.Substring(length);
        //    while (!leftover.StartsWith(" ") && leftover != string.Empty)
        //    {
        //        substring += leftover.Substring(0, 1);
        //        leftover = leftover.Substring(1);
        //    }
        //    substring = substring.Trim();
        //    while (substring.EndsWith("<br/>"))
        //    {
        //        substring = substring.Substring(0, substring.Length - 5);
        //        substring = substring.Trim();
        //    }

        //    if (input.Length > substring.Length)
        //        inputIsShortened = true;

        //    substring = substring.Replace("<br/>", "<br />");

        //    Tidy tidy = new Tidy();
        //    tidy.Options.DocType = DocType.Omit;
        //    tidy.Options.CharEncoding = CharEncoding.UTF8;
        //    tidy.Options.Xhtml = true;
        //    tidy.Options.NumEntities = true;

        //    TidyMessageCollection tmc = new TidyMessageCollection();
        //    MemoryStream inputStream = new MemoryStream();
        //    MemoryStream outputStream = new MemoryStream();

        //    byte[] bytes = Encoding.UTF8.GetBytes(substring);
        //    inputStream.Write(bytes, 0, bytes.Length);
        //    inputStream.Position = 0;
        //    tidy.Parse(inputStream, outputStream, tmc);

        //    string tidyResult = Encoding.UTF8.GetString(outputStream.ToArray());
        //    XmlDocument xmlDoc = new XmlDocument();
        //    xmlDoc.LoadXml(tidyResult);
        //    tidyResult = xmlDoc.SelectSingleNode("//body").InnerXml;

        //    if (!string.IsNullOrEmpty(elipsis))
        //    {
        //        if (tidyResult.EndsWith("</p>"))
        //            return string.Concat(tidyResult.Substring(0, tidyResult.Length - 4), elipsis, "</p>");
        //        return string.Concat(tidyResult, elipsis);
        //    }
        //    return tidyResult;
        //}

        /// <summary>
        /// Shortens a HTML formatted string, while keeping HTML formatting and complete words (also removes line-breakes at the end of the shortened string)
        /// </summary>
        /// <param name="input">The HTML formatted string</param>
        /// <param name="length">The approximate length of the output string (default: 300)</param>
        /// <param name="elipsis">Elipsis text to append to the output string (use string.Empty when elipsis should not be added, default: ...)</param>
        /// <returns>The shortened input string with HTML formatting</returns>
        //public static string ElipsisHtml(string input, int length = 300, string elipsis = "...")
        //{
        //    bool dummy;
        //    return ElipsisHtml(input, out dummy, length, elipsis);
        //}

        /// <summary>
        /// Determins how well two strings compare.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="compareWith"></param>
        /// <returns></returns>
        public static int MatchesStartToPoints(this string str, string compareWith)
        {
            char[] bStr = str.ToLower().ToCharArray();
            char[] cStr = compareWith.ToLower().ToCharArray();
            int len = str.Length <= compareWith.Length ? str.Length : compareWith.Length;
            int points = 0;

            for (int i = 0; i < len; i++)
            {
                if (bStr[i] == cStr[i]) points++;
            }

            return points;
        }

        /// <summary>
        /// Removes all html-tags from HTML, leaving only text.
        /// </summary>
        /// <param name="strHTML"></param>
        /// <returns></returns>
        public static string StripHTML(this string strHTML)
        {
            try
            {
                string strOutput = null;
                if ((strHTML != null) && !string.IsNullOrEmpty(strHTML))
                {
                    System.Text.RegularExpressions.Regex objRegExp = new System.Text.RegularExpressions.Regex("<(.|\\n)+?>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                    //Replace all HTML tag matches with the empty string
                    strOutput = objRegExp.Replace(strHTML, "");

                    //Replace all < and > with &lt; and &gt;
                    strOutput = strOutput.Replace("<", "&lt;");
                    strOutput = strOutput.Replace(">", "&gt;");
                    strOutput = strOutput.Replace("&nbsp;", " ");

                    return strOutput;
                    //Return the value of strOutput
                }
                else
                {
                    return "";
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}