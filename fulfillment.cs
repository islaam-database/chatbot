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
            if (googleApiKey == null) return new BadRequestObjectResult("Missing 'google-api-key' as a parameter.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var fulfillmentRequest = JsonConvert.DeserializeObject<GoogleCloudDialogflowV2WebhookRequest>(requestBody);
            var idb = new IslaamDBClient(googleApiKey);
            var responseObj = new List<string>() { };
            switch (fulfillmentRequest.QueryResult.Intent.DisplayName)
            {
                case "get-teachers":
                    {
                        var query = fulfillmentRequest.QueryResult.Parameters["person"].ToString();
                        var searchResult = idb.PersonAPI.Search(query);
                        var person = searchResult[0];
                        var teachers = idb.StudentsAPI
                            .GetData()
                            .Where(s => s.studentId == person.id)
                            .Select(s => s.teacherName)
                            .ToList();
                        var response = teachers.Count > 0
                            ? $"{person.name}'s teachers include: \n\n {string.Join(", ", teachers)}"
                            : $"Sorry. I don't know of any teachers of {person.name} at the moment.";
                        return new OkObjectResult(new GoogleCloudDialogflowV2WebhookResponse { FulfillmentText = response });
                    }
                case "who-is":
                    {
                        var query = fulfillmentRequest.QueryResult.Parameters["person"].ToString();
                        var searchResult = idb.PersonAPI.Search(query);
                        var person = searchResult[0];

                        // create response
                        responseObj.Add($"{person.name}.");
                        responseObj.Add(person.BioIntro(idb));
                        var responseAsText = string.Join(" ", responseObj);
                        var response = new GoogleCloudDialogflowV2WebhookResponse
                        {
                            FulfillmentText = responseAsText,
                        };
                        return new OkObjectResult(response);
                    }
            }
            return new OkObjectResult(new GoogleCloudDialogflowV2WebhookResponse { FulfillmentText = "Huh?" });
        }
    }
}
