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
        public static int MAX_DIST_FOR_SEARCH_RESULTS = 5;

        [FunctionName("fulfillment")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            var googleApiKey = (string)req.Query["google-api-key"];

            // if no API KEY
            if (googleApiKey == null)
                return new BadRequestObjectResult("Missing 'google-api-key' as a parameter.");

            var idb = new IslaamDBClient(googleApiKey);
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var fulfillmentRequest = JsonConvert.DeserializeObject<GoogleCloudDialogflowV2WebhookRequest>(requestBody);

            // get people
            var query = fulfillmentRequest.QueryResult.Parameters["person"].ToString();
            var searchResults = GetSearchResults(idb, query);
            var possibleResults = searchResults.Take(4);
            var acceptableResults = searchResults
                    .FindAll(x => x.lavDistance <= MAX_DIST_FOR_SEARCH_RESULTS);
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
                        var teachers = GetTeachers(idb, person);
                        var students = GetStudents(idb, person);
                        var QuickReplies = GetFivePeopleForSuggestions(teachers, students, searchResults);
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
                                        QuickReplies = QuickReplies,
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
            }
            return new OkObjectResult(new GoogleCloudDialogflowV2WebhookResponse { FulfillmentText = "Huh?" });
        }

        private static List<PersonSearchResult> GetSearchResults(IslaamDBClient idb, string query)
        {
            return idb.PersonAPI
                .Search(query)
                .OrderBy(x => x.lavDistance)
                .ToList();
        }

        private static List<string> GetFivePeopleForSuggestions(
            List<string> teachers,
            List<string> students,
            List<PersonSearchResult> searchResults
        )
        {
            var fivePeople = new List<string>();
            var minGroupLength = Math.Min(teachers.Count, students.Count);
            if (minGroupLength >= 2)
            {
                fivePeople = fivePeople.Concat(teachers.Take(2)).ToList(); // add 2 teachers
                fivePeople = fivePeople.Concat(students.Take(2)).ToList(); // add 2 students
            }
            else
            {
                var sum = teachers.Count + students.Count;
                if (sum >= 4)
                {
                    var minGroup = teachers.Count < students.Count ? teachers : students;
                    var maxGroup = teachers.Count < students.Count ? students : teachers;
                    fivePeople = fivePeople.Concat(minGroup).ToList(); // add small group
                    var amountLeft = 4 - fivePeople.Count;
                    fivePeople = fivePeople.Concat(maxGroup.Take(amountLeft)).ToList();
                }
                else
                {
                    fivePeople = fivePeople
                        .Concat(teachers)
                        .Concat(students)
                        .ToList();
                }
            }
            {
                var amountLeft = 5 - fivePeople.Count;
                var remainingPeople = searchResults
                    .Take(amountLeft + 1)
                    .Skip(1)
                    .Select(sr => sr.person.friendlyName);
                fivePeople = fivePeople.Concat(remainingPeople).ToList();
            }
            return fivePeople;
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
        private static List<string> GetStudents(IslaamDBClient idb, Person person)
        {
            return idb.StudentsAPI
                .GetData()
                .Where(s => s.teacherId == person.id)
                .Select(s => s.studentName)
                .ToList();
        }

    }
}
