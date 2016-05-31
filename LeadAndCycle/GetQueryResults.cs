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
using Npgsql;
using System.Collections.Generic;

namespace CodeEvaler
{
    public class Rootobject2
    {
        public int count { get; set; }
        public Value[] value { get; set; }
    }

    public class Value
    {
        public int id { get; set; }
        public int rev { get; set; }
        public Fields fields { get; set; }
        public string url { get; set; }
    }

    public class Fields
    {
        public string SystemAreaPath { get; set; }
        public string SystemTeamProject { get; set; }
        public string SystemIterationPath { get; set; }
        public string SystemWorkItemType { get; set; }
        public string SystemState { get; set; }
        public string SystemReason { get; set; }
        public DateTime SystemCreatedDate { get; set; }
        public string SystemCreatedBy { get; set; }
        public DateTime SystemChangedDate { get; set; }
        public string SystemChangedBy { get; set; }
        public string SystemTitle { get; set; }
        public string SystemBoardColumn { get; set; }
        public bool SystemBoardColumnDone { get; set; }
        public DateTime MicrosoftVSTSCommonStateChangeDate { get; set; }
        public int MicrosoftVSTSCommonPriority { get; set; }
        public float MicrosoftVSTSCommonStackRank { get; set; }
        public string MicrosoftVSTSCommonValueArea { get; set; }
        public int MicrosoftVSTSCommonBusinessValue { get; set; }
        public float MicrosoftVSTSCommonTimeCriticality { get; set; }
        public string WEF_04C522F1C6B34DEEB816CB65C9F6E00A_KanbanColumn { get; set; }
        public bool WEF_04C522F1C6B34DEEB816CB65C9F6E00A_KanbanColumnDone { get; set; }
        public string SystemDescription { get; set; }
        public string ConnectionsSuccessCriteria { get; set; }
        public string WEF_3F7199A41B50441999DB29F6135D3C64_KanbanColumn { get; set; }
        public bool WEF_3F7199A41B50441999DB29F6135D3C64_KanbanColumnDone { get; set; }
        public string SystemAssignedTo { get; set; }
        public string SystemBoardLane { get; set; }
        public DateTime MicrosoftVSTSCommonActivatedDate { get; set; }
        public string MicrosoftVSTSCommonActivatedBy { get; set; }
        public string WEF_CC7F3CAD0F354434A6BEFD36292F86CE_KanbanColumn { get; set; }
        public bool WEF_CC7F3CAD0F354434A6BEFD36292F86CE_KanbanColumnDone { get; set; }
        public string WEF_CC7F3CAD0F354434A6BEFD36292F86CE_KanbanLane { get; set; }
        public DateTime MicrosoftVSTSSchedulingStartDate { get; set; }
        public string WEF_B704F54BA21A4FB680EB0D12088EE0A7_KanbanColumn { get; set; }
        public bool WEF_B704F54BA21A4FB680EB0D12088EE0A7_KanbanColumnDone { get; set; }
        public string MicrosoftVSTSTCMReproSteps { get; set; }
        public string ConnectionsGoalType { get; set; }
        public string MicrosoftVSTSCommonSeverity { get; set; }
        public string MicrosoftVSTSCommonActivity { get; set; }
        public float ConnectionsTimebox { get; set; }
    }

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
            List<string> wiList = new List<string>() { };

            if (Request_privatepreview_visualstudio_com(out response))
            {
                responseText = ReadResponse(response);
                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(responseText);

                foreach (var item in results.workItems)
                    {
                        string wid = item.id.ToString();
                        wiList.Add(wid);
                        response.Close();
                    }

                string wiListConcat = string.Join(",", wiList);
                string wiDetails = "https://privatepreview.visualstudio.com/DefaultCollection/_apis/wit/workitems?ids=" + wiListConcat.ToString() + "&api-version=1.0";

                    //breakout into work item details
                    response = null;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(wiDetails);

                    request.UserAgent = "Fiddler";
                    request.Headers.Set(HttpRequestHeader.Authorization, "Basic c2dsaWRld2VsbEBjb25uZWN0aW9uc2VkdWNhdGlvbi5jb206cmlvczV3aTZ5NjJidWhpN2Y3dmZqeDZoeWRheXZtdnQyZW9ha2tkeWY2ZGNkdXU2bWwycQ==");

                    response = (HttpWebResponse)request.GetResponse();

                    responseText = ReadResponse(response);
                    var wiResults = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject2>(responseText);
                    Console.WriteLine(responseText.ToString());
                //insert work items and details into database or other consumable form for Power BI
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