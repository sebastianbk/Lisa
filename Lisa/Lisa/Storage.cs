using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        }
    }
}