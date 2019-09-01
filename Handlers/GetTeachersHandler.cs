using System;
using System.Collections.Generic;
using System.Linq;
using idb_dialog_flow;
using islaam_db_client;

namespace IslaamDatabase
{
    public class GetTeachersHandler : SinglePersonHandler
    {
        private List<string> teacherNames;

        public GetTeachersHandler(IslaamDBClient idb, IDictionary<string, object> entities)
            : base(idb, entities)
        {
            this.idb = idb;
            var allStudentTeachers = idb.StudentsAPI.GetData();
            teacherNames = personHelper.GetTeacherNames(allStudentTeachers);
        }

        public override string TextResponse
        {
            get
            {
                if (personHelper.person == null)
                    return PnfHandler.TextResponse;

                if (teacherNames.Count == 0)
                    return $"Sorry. I currently don't have any information on {friendlyName}'s teachers.";

                if (teacherNames.Count == 1)
                    return $"{friendlyName} was taught by {teacherNames[0]}. {TAIKATM}";

                return $"{friendlyName} was taught by {FriendlyStringJoin(teacherNames)}.";
            }
        }

        public override List<string> QuickReplies
        {
            get
            {
                if (personHelper.person == null)
                    return PnfHandler.QuickReplies;

                var defaultQRs = new List<string>()
                {
                    $"{friendlyName}'s students",
                    $"Who praised {friendlyName}?",
                    $"Who did {friendlyName} praised?",
                };
                var qrs = teacherNames
                    .Concat(
                        personHelper
                            .SearchResults
                            .Select(x => x.person.friendlyName)
                    )
                    .Take(5)
                    .Select(DefaultUtterance);
                return defaultQRs.Concat(qrs).ToList();
            }
        }

        protected override Func<string, string> Formula => x => $"Who taught {x}";

        public static string DefaultUtterance(string person)
        {
            return $"Who taught {person}?";
        }
    }
}