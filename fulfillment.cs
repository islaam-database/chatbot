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
                    var praises = idb.PraisesAPI
                        .GetData()
                        .Where(p => p.recommendeeId == person.id)
                        .Select(p => p.title)
                        .Distinct();

                    // create response
                    textResponse = person.BioIntro(idb);
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
