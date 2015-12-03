using System;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace WebRipper
{
    class Program
    {
        static void Main(string[] args)
        {
            GetDoctors("http://nepalhealthpages.com/doctors/urology");
            Console.ReadLine();
        }

        static void GetDoctors(string url)
        {
            var nodes = GetNode(url, "//div", "class", "hospital-listing doctor-listing specialty-listing");

            foreach (HtmlNode pNode in nodes.Descendants("li"))
            {
                foreach (HtmlNode cNode in pNode.Descendants("a"))
                {
                    if (cNode.Attributes.Contains("href")){
                        //Console.WriteLine(cNode.Attributes["href"].Value);
                        var profile = GetNode(cNode.Attributes["href"].Value, "//div", "id", "doctor-profile-wrapper");
                        foreach (HtmlNode list in profile.Descendants("div"))
                        {
                            if (list.Attributes.Count > 0 && list.ChildNodes["div"].Attributes.Contains("class"))
                            {
                                if (list.ChildNodes["div"].Attributes["class"].Value == "doctor-profile-field")
                                {
                                    var docName = list.InnerText;
                                }
                            }                           
                        }
                    }
                }
            }
        }

        static string Extract(string url){
            return (new WebClient().DownloadString(url));
        }

        static HtmlNode GetNode(string url, string xpath, string attr="", string value="")
        {
            HtmlNode returnNode = null; ; 
            var str = Extract(url);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(str);

            foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes(xpath)) //"//div"
            {
                if (!string.IsNullOrWhiteSpace(attr)){
                    if (node.Attributes.Contains(attr)){
                        if (node.Attributes[attr].Value == value){
                            returnNode = node;
                        }
                    }
                }
                else{
                    returnNode = node;
                }
            }
            return returnNode;
        }
    }
}
