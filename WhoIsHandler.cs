using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using islaam_db_client;

namespace idb_dialog_flow
{
    public class WhoIsHandler : Handler
    {
        private readonly PersonHelper personHelper;
        private readonly IslaamDBClient idb;
        private readonly BioInfo bioInfo;

        private string query;
        private HandlerLite pnfHandler;
        public override string Id
        {
            get
            {
                return "who-is";
            }
        }

        public override string TextResponse
        {
            get
            {
                if (personHelper.person == null)
                {
                    return $"Sorry. I couldn't find anyone named \"{personHelper.FirstNameCapitalized}\"";
                }
                return personHelper.person.GetBio(idb).text;
            }
        }

        public override List<string> QuickReplies
        {
            get
            {
                if (personHelper.person == null) return pnfHandler.QuickReplies;

                var teacherStudents = idb.StudentsAPI.GetData();
                var teachers = personHelper.GetTeachers(teacherStudents);
                var students = personHelper.GetStudents(teacherStudents);
                var searchResults = personHelper.SearchResults;

                return GetFivePeopleForSuggestions(teachers, students, searchResults);
            }
        }
        public WhoIsHandler(IslaamDBClient idb, IDictionary entities)
        {
            this.idb = idb;

            query = entities["person"].ToString();
            personHelper = new PersonHelper(query, idb);
            bioInfo = personHelper.person?.GetBio(idb);
            pnfHandler = PersonNotFoundHandler(query, x => $"Who is {x}", personHelper.SearchResults);
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
    }
}
