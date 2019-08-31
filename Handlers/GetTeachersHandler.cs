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
            teacherNames = personHelper.GetTeachers(allStudentTeachers);
        }

        public override string TextResponse
        {
            get
            {

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
                return teacherNames
                    .Concat(
                        personHelper
                            .SearchResults
                            .Skip(1)
                            .Select(x => x.person.friendlyName)
                    )
                    .Take(5)
                    .Select(DefaultUtterance)
                    .ToList();
            }
        }
        public static string DefaultUtterance(string person)
        {
            return $"Who taught {person}?";
        }
    }
}