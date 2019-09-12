using System;
using System.Collections.Generic;
using System.Linq;
using idb_dialog_flow;
using Microsoft.EntityFrameworkCore;

namespace IslaamDatabase
{
    public class GetTeachersHandler : SinglePersonHandler
    {
        private readonly List<string> teacherNames;

        public GetTeachersHandler(Islaam.Database idb, IDictionary<string, object> entities)
            : base(idb, entities)
        {
            this.idb = idb;
            if (Person != null)
            {
                Person = idb
                    .People
                    .Include(p => p.Teachers)
                    .Where(p => p.Id == Person.Id)
                    .First();

                teacherNames = Person
                    .Teachers
                    .Select(x => x.Teacher.Name)
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

                if (teacherNames.Count == 0)
                    return $"Sorry. I currently don't have any information on {Person.FriendlyName}'s teachers.";

                if (teacherNames.Count == 1)
                    return $"{Person.FriendlyName} was taught by {teacherNames[0]}. {TAIKATM}";

                return $"{Person.FriendlyName} was taught by {FriendlyStringJoin(teacherNames)}.";
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
                    $"{Person.FriendlyName}'s students",
                    $"Who praised {Person.FriendlyName}?",
                    $"Who did {Person.FriendlyName} praised?",
                };
                var qrs = teacherNames
                    .Concat(SearchResults.Select(x => x.Person.FriendlyName))
                    .Take(5)
                    .Select(Formula);
                return defaultQRs.Concat(qrs).ToList();
            }
        }

        protected override string Formula(string personName)
        {
            return $"Who taught {personName}?";
        }
    }
}
