﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Lisa
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                var translator = new Translator();
                var lang = translator.Detect(activity.Text);
                activity.Text = translator.Translate(activity.Text, lang, "en");

                var score = await Phychologist.GetSentiment(activity.Text);
                Storage.Store(activity.Text, "Hackaton Barca", score, activity.From.Id, activity.From.Name, activity.ServiceUrl);

                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                var x = connector.DeserializationSettings;

                var reply = "";
                if(score<0.34)
                    reply = "Sorry to hear that";
                if (score > 0.34 && score < 0.67)
                    reply = "Thank you";
                if (score > 0.67)
                    reply = "Very good to hear that.";

                Activity reply1 = activity.CreateReply(translator.Translate(reply, "en", lang));
                await connector.Conversations.ReplyToActivityAsync(reply1);

                var json = JsonConvert.SerializeObject(activity);
                var obj = JsonConvert.DeserializeObject<Activity>(json);

                //var dialog = new CustomLuisDialogML { OriginalLanguage = lang };
                //await Conversation.SendAsync(activity, () => dialog);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}