using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;

namespace ChickenSoftware.StockAnalyzer.Bot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                // calculate something for us to return
                int length = (message.Text ?? string.Empty).Length;

                // return our reply to the user
                //return message.CreateReplyMessage($"You sent {length} characters");
                var stockAnalyzer = new StockAnalyzer();

                var ticker = stockAnalyzer.GetTicker(message.Text);
                var closingPrice = stockAnalyzer.GetStockClose(ticker);

                //var closingPrice = stockAnalyzer.GetStockClose(message.Text);

                if (closingPrice == -1.0)
                {
                    return message.CreateReplyMessage("Could not find");
                }
                else
                {
                    return message.CreateReplyMessage(closingPrice.ToString());
                }
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}