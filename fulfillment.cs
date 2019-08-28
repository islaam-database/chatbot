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

namespace IslaamDatabase
{
    public static class Fulfillment
    {
        public static int MAX_DIFF_FOR_SEARCH_RESULTS = 5;

        [FunctionName("fulfillment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
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
                .OrderBy(x => x.lavDistance)
                .ToList();
            var possibleResults = searchResults.Take(4);
            var acceptableResults = searchResults
                    .FindAll(x => x.lavDistance <= MAX_DIFF_FOR_SEARCH_RESULTS);
            var person = acceptableResults.FirstOrDefault()?.person;
            var intent = fulfillmentRequest.QueryResult.Intent.DisplayName;

            // if person found
            switch (intent)
            {
                case "who-is":
                    {
                        // if no search results
                        if (person == null)
                        {
                            var firstNameCapitalized = new CultureInfo("en-US", false).TextInfo.ToTitleCase(query);
                            return new OkObjectResult(
                                new GoogleCloudDialogflowV2WebhookResponse
                                {
                                    FulfillmentMessages = new List<GoogleCloudDialogflowV2IntentMessage>
                                    {
                                new GoogleCloudDialogflowV2IntentMessage
                                {
                                    Platform = "FACEBOOK",
                                    QuickReplies = new GoogleCloudDialogflowV2IntentMessageQuickReplies
                                    {
                                        Title = NO_PERSON_FOUND_MESSAGE(firstNameCapitalized),
                                        QuickReplies = possibleResults
                                            .Select(p => Intents.WHO_IS.DefaultResponse(p.person))
                                            .ToList()
                                    }
                                },
                                new GoogleCloudDialogflowV2IntentMessage
                                    {
                                        Platform = null,
                                        Text = new GoogleCloudDialogflowV2IntentMessageText
                                        {
                                            Text = new List<string>{ NO_PERSON_FOUND_MESSAGE(firstNameCapitalized) },
                                        }
                                    },
                                        }
                                }
                            );
                        }
                        var bio = person.GetBio(idb);
                        var textResponse = bio.text;
                        if (bio.amountOfInfo <= 2)
                        {
                            textResponse += " That's all the information I have at the moment.";
                        }
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
                                        QuickReplies = new string[]
                                        {
                                            "test",
                                            "test2"
                                        },
                                        Title = textResponse
                                    },
                                },
                                // web
                                new GoogleCloudDialogflowV2IntentMessage
                                {
                                    Platform = null,
                                    Text = new GoogleCloudDialogflowV2IntentMessageText
                                    {
                                        Text = new List<string>{ textResponse },
                                    }
                                },
                            }
                        };
                        return new OkObjectResult(response);
                    }
                case "get-teachers":
                    {
                        List<string> teachers = GetTeachers(idb, person);
                        var response = teachers.Count > 0
                            ? $"{person.name}'s teachers include: \n\n {string.Join(", ", teachers)}"
                            : $"Sorry. I don't know of any teachers of {person.name} at the moment.";
                        return new OkObjectResult(new GoogleCloudDialogflowV2WebhookResponse { FulfillmentText = response });
                    }
            }
            return new OkObjectResult(new GoogleCloudDialogflowV2WebhookResponse { FulfillmentText = "Huh?" });
        }

        private static string NO_PERSON_FOUND_MESSAGE(string query)
        {
            return $"Sorry. I couldn't find anyone named \"{query}\"";
        }

        private static List<string> GetTeachers(IslaamDBClient idb, Person person)
        {
            return idb.StudentsAPI
                .GetData()
                .Where(s => s.studentId == person.id)
                .Select(s => s.teacherName)
                .ToList();
        }
    }
}
