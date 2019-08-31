using System.Collections.Generic;
using System.Linq;
using idb_dialog_flow;
using islaam_db_client;

namespace IslaamDatabase
{
    public class GetStudentsHandler : SinglePersonHandler
    {
        private List<string> studentNames;
        public GetStudentsHandler(IslaamDBClient idb, IDictionary<string, object> entities)
            : base(idb, entities)
        {
            this.idb = idb;
            var allStudentTeachers = idb.StudentsAPI.GetData();
            studentNames = personHelper.GetStudents(allStudentTeachers);
        }

        public override string TextResponse
        {
            get
            {
                if (studentNames.Count == 0)
                    return $"Sorry. I currently don't have any information on {friendlyName}'s students.";

                if (studentNames.Count == 1)
                    return $"{friendlyName} taught {studentNames[0]}. {TAIKATM}";

                return $"{friendlyName}'s students include {FriendlyStringJoin(studentNames)}.";
            }
        }

        public override List<string> QuickReplies
        {
            get
            {
                return studentNames
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
            return $"{person}'s students";
        }
    }
}