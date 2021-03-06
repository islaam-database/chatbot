using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Google.Apis.Dialogflow.v2.Data;
using System.Collections.Generic;
using idb_dialog_flow;

namespace IslaamDatabase
{
    public static class Fulfillment
    {
        public static int MAX_DIST_FOR_SEARCH_RESULTS = 5;

        [FunctionName("fulfillment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req
        )
        {
            var idb = new Islaam.Database();
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var fulfillmentRequest = JsonConvert.DeserializeObject<GoogleCloudDialogflowV2WebhookRequest>(requestBody);
            var intent = fulfillmentRequest.QueryResult.Intent.DisplayName;
            var entities = fulfillmentRequest.QueryResult.Parameters;
            var handler = GetHandler(idb, intent, entities);

            // if person found
            var response = new GoogleCloudDialogflowV2WebhookResponse
            {
                FulfillmentMessages = new List<GoogleCloudDialogflowV2IntentMessage>
                    {
                        // facebook
                        new GoogleCloudDialogflowV2IntentMessage
                        {
                            Platform = "FACEBOOK",
                            QuickReplies = new GoogleCloudDialogflowV2IntentMessageQuickReplies
                            {
                                QuickReplies = handler.QuickReplies,
                                Title = handler.TextResponse
                            },
                        },
                        // web
                        new GoogleCloudDialogflowV2IntentMessage
                        {
                            Platform = null,
                            Text = new GoogleCloudDialogflowV2IntentMessageText
                            {
                                Text = new List<string>{ handler.TextResponse },
                            }
                        },
                    }
            };
            return new OkObjectResult(response);
        }

        private static Handler GetHandler(Islaam.Database idb, string intent, IDictionary<string, object> entities)
        {
            if (intent == Intents.WHO_IS) return new WhoIsHandler(idb, entities);
            if (intent == Intents.GET_TEACHERS) return new GetTeachersHandler(idb, entities);
            if (intent == Intents.GET_STUDENTS) return new GetStudentsHandler(idb, entities);
            if (intent == Intents.GET_PRAISERS) return new GetPraisersHandler(idb, entities);
            if (intent == Intents.GET_PRAISEES) return new GetPraiseesHandler(idb, entities);
            if (intent == Intents.GET_DEATH_YEAR) return new GetDeathYearHandler(idb, entities);
            return new NotSupportedHandler();
        }
    }
}
