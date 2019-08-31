using System.Collections.Generic;
using System.Linq;
using islaam_db_client;

public class PersonHelper
{
    public const int MAX_LAV_DIST_FOR_SEARCH = 5;
    public List<PersonSearchResult> searchResults;
    public Person person;
    public PersonHelper(string query, IslaamDBClient idb)
    {
        searchResults = idb.PersonAPI
            .Search(query)
            .OrderBy(x => x.lavDistance)
            .ToList();

        person = searchResults
            .FindAll(x => x.lavDistance <= MAX_LAV_DIST_FOR_SEARCH)
            .FirstOrDefault()
            ?.person;
    }

    public List<string> GetTeachers(List<Student> students)
    {
        var teacherNames = students
            .FindAll(s => s.studentId == person.id)
            .Select(x => x.teacherName)
            .ToList();
        return teacherNames;
    }
    public List<string> GetStudents(List<Student> students)
    {
        var studentNames = students
            .FindAll(s => s.teacherId == person.id)
            .Select(x => x.studentName)
            .ToList();
        return studentNames;
    }
    public List<string> GetPraiserNames(List<Praise> praises)
    {
        var praiserNames = praises
            .FindAll(s => s.recommendeeId == person.id)
            .Select(x => x.recommenderName)
            .ToList();
        return praiserNames;
    }
    public List<string> GetPraiseeNames(List<Praise> praises)
    {
        var praiseeNames = praises
            .FindAll(s => s.recommenderId == person.id)
            .Select(x => x.recommendeeName)
            .ToList();
        return praiseeNames;
    }


}