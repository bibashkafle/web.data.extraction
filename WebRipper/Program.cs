using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace WebRipper
{
    class Program
    {
        static void Main(string[] args){
            GetDoctors("http://nepalhealthpages.com/doctors/urology");
            GetDoctors("http://nepalhealthpages.com/doctors/urology/20");
            GetDoctors("http://nepalhealthpages.com/doctors/urology/40");
            Console.ReadLine();
        }

        static void GetDoctors(string url)
        {
            Console.WriteLine("\n_________________________________________________________________________\n");
            var nodes = GetNode(url, "//div[@class='hospital-listing doctor-listing specialty-listing']");
            var sb = new StringBuilder();
            sb.AppendLine("Hospital Name, Hospital Contact, Doctor Name, Degree, Department, Speciality");
            foreach (HtmlNode pNode in nodes.Descendants("li"))
            {
                foreach (HtmlNode cNode in pNode.Descendants("a"))
                {
                    if (cNode.Attributes.Contains("href")){
                        Console.WriteLine(cNode.Attributes["href"].Value);
                        var profile = GetNode(cNode.Attributes["href"].Value, "//div[@id='doctor-profile-wrapper']");

                        foreach (HtmlNode list in profile.Descendants())
                        {
                            var hospitalName = list.SelectNodes("//div[@class='doctor-hospital-name']")[0].InnerText.Trim();
                            var hospitalContact = list.SelectNodes("//div[@class='doctor-hospital-details']")[0].InnerText.Trim().Replace(",", " ").Replace(" Tel:", ""); 
                            var doctorProfile = list.SelectNodes("//div[@class='doctor-profile-field']");
                            var doctorName = "";
                            var degree = "";
                             var department = "";
                            var speciality = "";

                            if(doctorProfile.Count>0)
                                doctorName = doctorProfile[0].InnerText.Trim().Replace("Name : ", "");

                            if(doctorProfile.Count>1)
                                degree = doctorProfile[1].InnerText.Trim().Replace("Academic Degree  : ", "");

                            if(doctorProfile.Count>2)
                                department = doctorProfile[2].InnerText.Trim().Replace("Department : ", "").Replace("\t","");

                             if(doctorProfile.Count>3)
                                 speciality = doctorProfile[3].InnerText.Trim().Replace("Speciality  : ","").Replace("\t","");

                             sb.AppendLine(hospitalName + " , " + hospitalContact + " , " + doctorName + " , " + degree + " , " + department + " , " + speciality);
                             //Console.Write(hospitalName + " , " + hospitalContact + " , " + doctorName + " , " + degree + " , " + department + " , " + speciality);
                        
                            //File.WriteAllText(@"D:\Bibash\Project\web.data.extraction\profile.csv",sb.ToString());
                             WriteFile(sb.ToString());
                            break;
                        }
                    }
                }
            }
        }

        static HtmlNode GetNode(string url, string xpath)
        {
            var nodeList = GetNodeList(url, xpath);
            if (nodeList.Count > 0)
                return nodeList[0];

            return null;
        }

        static HtmlNodeCollection GetNodeList(string url, string xpath)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            var str = Extract(url);
            htmlDoc.LoadHtml(str);
            return htmlDoc.DocumentNode.SelectNodes(xpath);
        }

        static string Extract(string url)
        {
            return (new WebClient().DownloadString(url));
        }

        static void WriteFile(string value)
        {
            var path = @"D:\Bibash\Project\web.data.extraction\profile.csv";
            if (!File.Exists(path)){
                File.WriteAllText(path, value);
            }
            else{
                File.AppendAllText(path, value);
            }
        }
    }
}
