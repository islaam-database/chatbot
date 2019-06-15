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
using Google.Apis;
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
            string googleApiKey = req.Query["google-api-key"];
            if (googleApiKey == null) return new BadRequestObjectResult("Missing 'google-api-key' as a parameter");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var fulfillmentRequest = JsonConvert.DeserializeObject<GoogleCloudDialogflowV2WebhookRequest>(requestBody);
            var idb = new IslaamDBClient(googleApiKey);
            string textResponse;
            switch (fulfillmentRequest.QueryResult.Intent.DisplayName)
            {
                case "who-is":
                    var query = fulfillmentRequest.QueryResult.Parameters["person"].ToString();
                    var searchResult = idb.PersonAPI.Search(query);
                    var person = searchResult[0];

                    // create response
                    textResponse = searchResult[0].name + ".";
                    if (person.kunya != null) textResponse += $" He is {person.kunya}.";
                    if (person.birthYear != null) textResponse += $" He was born in the year {person.birthYear}.";
                    if (person.deathYear != null) textResponse += $" He passed away in the year {person.deathYear}.";
                    if (person.birthYear != null && person.deathYear != null) textResponse += " Corresponding to the Hijri calendar.";
                    if (person.location != null) textResponse += $" He is from {person.location}.";
                    if (person.source != null) textResponse += $" Some or all of this information was gathered from ${person.source}";
                    break;
                default:
                    textResponse = "Huh?";
                    break;
            }

            var response = new GoogleCloudDialogflowV2WebhookResponse { FulfillmentText = textResponse };
            return new OkObjectResult(response);
        }
    }
}
