using System;
using System.Collections.Generic;
using System.Linq;
using idb_dialog_flow;
using Microsoft.EntityFrameworkCore;

namespace IslaamDatabase
{
    public class GetStudentsHandler : SinglePersonHandler
    {
        private readonly List<string> studentNames;
        public GetStudentsHandler(Islaam.Database idb, IDictionary<string, object> entities)
            : base(idb, entities)
        {
            this.idb = idb;
            if (Person != null)
            {
                Person = idb
                    .People
                    .Include(p => p.Students)
                    .Where(p => p.Id == Person.Id)
                    .First();

                studentNames = Person
                    .Students
                    .Select(x => x.Student.Name)
                    .Distinct()
                    .ToList();
            }
        }

        public override string TextResponse
        {
            get
            {
                if (Person == null)
                    return PnfHandler.TextResponse;

                if (studentNames.Count == 0)
                    return $"Sorry. I currently don't have any information on {Person.FriendlyName}'s students.";

                if (studentNames.Count == 1)
                    return $"{Person.FriendlyName} taught {studentNames[0]}. {TAIKATM}";

                return $"{Person.FriendlyName}'s students include {FriendlyStringJoin(studentNames)}.";
            }
        }

        public override List<string> QuickReplies
        {
            get
            {
                if (Person == null)
                    return PnfHandler.QuickReplies;

                var defaultQRs = new List<string>()
                {
                    $"{Person.FriendlyName}'s teachers",
                    $"Who praised {Person.FriendlyName}?",
                    $"Who did {Person.FriendlyName} praised?",
                };
                var qrs = studentNames
                    .Concat(SearchResults.Select(x => x.Person.FriendlyName))
                    .Take(5)
                    .Select(DefaultUtterance);
                return defaultQRs.Concat(qrs).ToList();
            }
        }


        public static string DefaultUtterance(string person)
        {
            return $"{person}'s students";
        }

        protected override string Formula(string personName)
        {
            return $"Students of {personName}";
        }
    }
}