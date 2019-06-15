using System;
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

namespace IslaamDatabase
{
    public static class Fulfillment
    {
        [FunctionName("fulfillment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var fulfillmentRequest = JsonConvert.DeserializeObject<GoogleCloudDialogflowV2WebhookRequest>(requestBody);
            var idb = new IslaamDBClient(SECRETS.API_KEY);
            string textResponse;
            switch (fulfillmentRequest.QueryResult.Intent.DisplayName)
            {
                case "who-is":
                    var query = fulfillmentRequest.QueryResult.Parameters["person"].ToString();
                    var searchResult = idb.PersonAPI.Search(query);
                    textResponse = searchResult[0].name;
                    break;
                default:
                    textResponse = "Huh?";
                    break;
            }

            var response = new GoogleCloudDialogflowV2WebhookResponse { FulfillmentText = textResponse };

            return fulfillmentRequest != null
                ? (ActionResult)new OkObjectResult(response)
                : new BadRequestObjectResult("GIT: Please pass a name on the query string or in the request body");
        }
    }
}
