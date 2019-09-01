using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using islaam_db_client;
using static idb_dialog_flow.Handler;

namespace idb_dialog_flow
{
    public class WhoIsHandler : SinglePersonHandler
    {
        public WhoIsHandler(IslaamDBClient idb, IDictionary<string, object> entities)
            : base(idb, entities)
        { }

        public override string TextResponse
        {
            get
            {
                if (personHelper.person == null)
                    return PnfHandler.TextResponse;

                var bio = personHelper.person.GetBio(idb);

                if (bio.amountOfInfo <= 2)
                {
                    bio.text += " That's all the information I have at the moment.";
                }
                return bio.text;
            }
        }

        public override List<string> QuickReplies
        {
            get
            {
                if (personHelper.person == null)
                    return PnfHandler.QuickReplies;

                var teacherStudents = idb.StudentsAPI.GetData();
                var teachers = personHelper.GetTeacherNames(teacherStudents);
                var students = personHelper.GetStudentNames(teacherStudents);
                var searchResults = personHelper.SearchResults;

                return GetFivePeopleForSuggestions(teachers, students, searchResults)
                    .Select(Formula)
                    .ToList();
            }
        }

        protected override Func<string, string> Formula => x => $"Who is {x}?";

        public static List<string> GetFivePeopleForSuggestions(
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
                    .Select(sr => sr.person.friendlyName);
                fivePeople = fivePeople.Concat(remainingPeople).ToList();
            }
            return fivePeople;
        }
    }
}
