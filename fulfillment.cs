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
        public static int MAX_DIFF_FOR_SEARCH_RESULTS = 5;

        [FunctionName("fulfillment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var googleApiKey = (string)req.Query["google-api-key"];
            if (googleApiKey == null)
                return new BadRequestObjectResult("Missing 'google-api-key' as a parameter.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var fulfillmentRequest = JsonConvert.DeserializeObject<GoogleCloudDialogflowV2WebhookRequest>(requestBody);
            var idb = new IslaamDBClient(googleApiKey);
            var responseObj = new List<string>() { };
            var query = fulfillmentRequest.QueryResult.Parameters["person"].ToString();

            var searchResults = idb.PersonAPI
                .Search(query)
                .FindAll(x => x.lavDistance <= MAX_DIFF_FOR_SEARCH_RESULTS)
                .OrderBy(x => x.lavDistance)
                .ToList();

            // if no search results
            if (searchResults.Count == 0)
            {
                return new OkObjectResult(
                    new GoogleCloudDialogflowV2WebhookResponse {
                        FulfillmentText = $"Sorry. I couldn't find anyone named {query}.",
                    }
                );
            }

            var person = searchResults[0].person;
            var intent = fulfillmentRequest.QueryResult.Intent.DisplayName;

            switch (intent)
            {
                case "who-is":
                    {
                        responseObj.Add(person.BioIntro(idb));
                        var responseAsText = string.Join(" ", responseObj);
                        var response = new GoogleCloudDialogflowV2WebhookResponse
                        {
                            FulfillmentText = responseAsText,
                        };
                        return new OkObjectResult(response);
                    }
                case "get-teachers":
                    {
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
            }
            return new OkObjectResult(new GoogleCloudDialogflowV2WebhookResponse { FulfillmentText = "Huh?" });
        }
    }
}
