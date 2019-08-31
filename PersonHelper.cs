using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using islaam_db_client;

public class PersonHelper
{
    public const int MAX_LAV_DIST_FOR_SEARCH = 5;
    public string FirstNameCapitalized;
    public List<PersonSearchResult> SearchResults;
    public Person person;
    public PersonHelper(string query, IslaamDBClient idb)
    {
        FirstNameCapitalized = new CultureInfo("en-US", false).TextInfo.ToTitleCase(query);

        SearchResults = idb.PersonAPI
            .Search(query)
            .OrderBy(x => x.lavDistance)
            .ToList();

        person = SearchResults
            .FindAll(x => x.lavDistance <= MAX_LAV_DIST_FOR_SEARCH)
            .FirstOrDefault()
            ?.person;
    }

    public List<string> GetTeacherNames(List<Student> allStudents)
    {
        var teacherNames = allStudents
            .FindAll(s => s.studentId == person?.id)
            .Select(x => x.teacherName)
            .Distinct()
            .ToList();
        return teacherNames;
    }
    public List<string> GetStudentNames(List<Student> allStudents)
    {
        var studentNames = allStudents
            .FindAll(s => s.teacherId == person?.id)
            .Select(x => x.studentName)
            .Distinct()
            .ToList();
        return studentNames;
    }
    public List<string> GetPraiserNames(List<Praise> praises)
    {
        var praiserNames = praises
            .FindAll(s => s.recommendeeId == person?.id)
            .Select(x => x.recommenderName)
            .Distinct()
            .ToList();
        return praiserNames;
    }
    public List<string> GetPraiseeNames(List<Praise> praises)
    {
        var praiseeNames = praises
            .FindAll(s => s.recommenderId == person?.id)
            .Select(x => x.recommendeeName)
            .Distinct()
            .ToList();
        return praiseeNames;
    }

}