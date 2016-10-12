using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Diagnostics;

namespace Lisa
{
    public class  SentimentScore : TableEntity
    {
        public SentimentScore()
        {
            this.PartitionKey = "SentimentScores";
            this.RowKey = DateTime.Now.Ticks.ToString();
        }
        public string Message { get; set; }
        public string Session { get; set; }
        public string Id { get; set; }
        public double Score { get; set; }
        public string User { get; set; }
        public string UserId { get; set; }
        public string UserServiceUrl { get; set; }

    }
    public class Storage
    {
        public static void Store(string msg, string session, double score, string user, string userId, string userServiceUrl)
        {
            var sentimentScore = new SentimentScore()
            {
                Message = msg,
                Session = session,
                Id = Guid.NewGuid().ToString(),
                Score = score,
                User = user,
                UserId = userId,
                UserServiceUrl = userServiceUrl          
            };

            var connectionString = CloudConfigurationManager.GetSetting("AzureWebJobsStorage");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("SessionSentiment");
            table.CreateIfNotExists();

            TableOperation insertOperation = TableOperation.Insert(sentimentScore);
            table.Execute(insertOperation);


            #region powerbi
            var streamingDataUrl = "https://api.powerbi.com/beta/72f988bf-86f1-41af-91ab-2d7cd011db47/datasets/6b48c5da-43d4-4eac-8bb3-323a27e18e49/rows?key=73oi5Z3r7UElJD5ldjTFOz5oLzC4tuFTGO8bOUspVDeQ4HtyL82rywNK2FFNXugM9qa9Wz9PO3l81lt%2B451lSg%3D%3D";


            var sentimentObject = new
            {
                date = DateTime.Now.ToString("o"),
                sentiment = score
            };

            var json = JsonConvert.SerializeObject(sentimentObject);

            var client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            client.PostAsync(streamingDataUrl, content);

            #endregion

        }
    }
}