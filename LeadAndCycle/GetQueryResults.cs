using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace CodeEvaler
{
    public class Rootobject
    {
        public string queryType { get; set; }
        public string queryResultType { get; set; }
        public DateTime asOf { get; set; }
        public Column[] columns { get; set; }
        public Workitem[] workItems { get; set; }
    }

    public class Column
    {
        public string referenceName { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Workitem
    {
        public int id { get; set; }
        public string url { get; set; }
    }

    public static class Program
    {
        public static void Main()
        {
            (new CodeEvaler()).Eval();
        }
    }

    public class CodeEvaler
    {
        public void Eval()
        {
            MakeRequests();
        }
        private void MakeRequests()
        {
            HttpWebResponse response;
            string responseText;

            if (Request_privatepreview_visualstudio_com(out response))
            {
                responseText = ReadResponse(response);
                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(responseText);
                foreach (var item in results.workItems)
                {
                    int wid = item.id;
                    Console.WriteLine(wid);
                    response.Close();
                }
            }
        }
        
        private static string ReadResponse(HttpWebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            {
                Stream streamToRead = responseStream;
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    streamToRead = new GZipStream(streamToRead, CompressionMode.Decompress);
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    streamToRead = new DeflateStream(streamToRead, CompressionMode.Decompress);
                }

                using (StreamReader streamReader = new StreamReader(streamToRead, Encoding.UTF8))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        private bool Request_privatepreview_visualstudio_com(out HttpWebResponse response)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://privatepreview.visualstudio.com/DefaultCollection/0e3ce5b3-30a5-495c-bf8e-85cda5a72462/_apis/wit/wiql/9a100a9e-5e61-4afd-acd4-4c9150d7c3c5");

                request.UserAgent = "Fiddler";
                request.Headers.Set(HttpRequestHeader.Authorization, "Basic c2dsaWRld2VsbEBjb25uZWN0aW9uc2VkdWNhdGlvbi5jb206cmlvczV3aTZ5NjJidWhpN2Y3dmZqeDZoeWRheXZtdnQyZW9ha2tkeWY2ZGNkdXU2bWwycQ==");

                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
                else return false;
            }
            catch (Exception)
            {
                if (response != null) response.Close();
                return false;
            }

            return true;
        }
    }
}