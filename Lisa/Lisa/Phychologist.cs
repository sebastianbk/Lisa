using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

namespace Lisa
{
    public class Phychologist
    {
        public static async Task<double> GetSentiment(string inputMsg)
        {
            var sentimentScore = 0.5;
            string queryUri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";
            string apiKey = ConfigurationManager.AppSettings["MicrosoftCSTextAppKey"];

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            BatchInput sentimentInput = new BatchInput();
            sentimentInput.documents = new List<Lisa.DocumentInput>();
            sentimentInput.documents.Add(new DocumentInput()
                {
                    id = 1,
                    text = inputMsg
                }
            );

            var sentimentJsonInput = JsonConvert.SerializeObject(sentimentInput);
            byte[] byteData = Encoding.UTF8.GetBytes(sentimentJsonInput);

            var content = new ByteArrayContent(byteData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var sentimentPost = await client.PostAsync(queryUri, content);

            var sentimentRawResponse = await sentimentPost.Content.ReadAsStringAsync();
            var sentimentJsonResponse = JsonConvert.DeserializeObject<BatchResult>(sentimentRawResponse);

            sentimentScore = sentimentJsonResponse.documents[0].score;
            return sentimentScore;
        }
    }
}