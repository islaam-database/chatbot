using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Google.Apis.Dialogflow.v2.Data;
using islaam_db_client;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using System.Globalization;
using System;
using idb_dialog_flow;
using System.ComponentModel;

namespace IslaamDatabase
{
    public static class Fulfillment
    {
        public static int MAX_DIST_FOR_SEARCH_RESULTS = 5;

        [FunctionName("fulfillment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req
        )
        {
            var googleApiKey = (string)req.Query["google-api-key"];

            // if no API KEY
            if (googleApiKey == null)
                return new BadRequestObjectResult("Missing 'google-api-key' as a parameter.");

            var idb = new IslaamDBClient(googleApiKey);
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var fulfillmentRequest = JsonConvert.DeserializeObject<GoogleCloudDialogflowV2WebhookRequest>(requestBody);
            var intent = fulfillmentRequest.QueryResult.Intent.DisplayName;
            var entities = fulfillmentRequest.QueryResult.Parameters;
            Handler handler;

            // choose the correct handler
            if (intent == Intents.WHO_IS)
                handler = new WhoIsHandler(idb, entities);

            else if (intent == Intents.GET_TEACHERS)
                handler = new GetTeachersHandler(idb, entities);

            else if (intent == Intents.GET_STUDENTS)
                handler = new GetStudentsHandler(idb, entities);

            else handler = new NotSupportedHandler();

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

    }
}
