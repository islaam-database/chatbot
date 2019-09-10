using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Islaam;

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

        SearchResults = SearchResults.Skip(1).ToList();
    }



    public BioInfo GetBio(IslaamDBClient idb)
    {
        var people = idb.PersonAPI.GetDataFromSheet().ToList();
        var pronoun = useMasculinePronoun ? "He" : "She";
        var possesivePronoun = useMasculinePronoun ? "His" : "Her";

        // start
        var biography = $"{pronoun} is ";
        var praises = GetUniquePraises(idb);
        var teachersAndStudents = GetTeachersAndStudents(idb);
        var teachers = GetUniqueTeachers(teachersAndStudents);
        var students = GetUniqueStudents(teachersAndStudents);
        var titles = GetUniqueTitles(praises);
        var praiserNames = FriendlyJoin(praises.Select(x => x.recommenderName).ToList());

        // booleans
        var hasPraises = praises.Count > 0;
        var hasTitles = titles.Count > 0;
        var hasLocation = location != null;
        var hasKunya = kunya != null;
        var hasDeathYear = deathYear != null;
        var hasBirthYear = birthYear != null;
        var hasBirthPlace = birthPlace != null;
        var hasTeachers = teachers.Count > 0;
        var hasStudents = students.Count > 0;
        var hasGeneration = generation != null;

        /** The amount of information in this bio **/
        var amountOfInfo = GetAmountOfInfo();

        // titles
        if (hasTitles)
            biography += $"the {String.Join(", the ", titles)}, ";

        // name or kunya
        if (hasKunya)
            biography += kunya;
        else
            biography += name;
        biography += ". ";

        if (hasGeneration)
            biography += $"{pronoun} is from the {generation}. ";

        // birth
        if (hasBirthYear)
            if (hasBirthPlace)
                biography += $"{pronoun} was born in {birthPlace} in the year {birthYear} AH. ";
            else
                biography += $"{pronoun} was born in the year {birthYear} AH. ";
        else
            if (hasBirthPlace)
            biography += $"{pronoun} was born in {birthPlace}. (I don't have {possesivePronoun} birth year yet.) ";

        // location
        if (hasLocation)
            biography += $"{pronoun} lived in {location}. ";

        // praises
        if (hasPraises)
            biography += $"{pronoun} was praised by (including students of knowledge) {praiserNames}. ";

        // teachers
        if (hasTeachers)
            biography += $"{pronoun} took knowledge from {FriendlyJoin(teachers)}. ";

        // students
        if (hasStudents)
            biography += $"{pronoun} taught {FriendlyJoin(students)}. ";

        // books
        // not yet supported

        // death year
        if (hasDeathYear)
            biography += $"{pronoun} died in the year {deathYear} AH.";

        biography = biography.Trim();

        // join sentences together
        return new BioInfo
        {
            text = biography,
            amountOfInfo = amountOfInfo,
        };

        int GetAmountOfInfo()
        {
            return new bool[] {
                    hasPraises,
                    hasTitles,
                    hasLocation,
                    hasKunya,
                    hasDeathYear,
                    hasBirthYear,
                    hasBirthPlace,
                    hasTeachers,
                    hasStudents,
                    hasGeneration
                }.Where(x => x).Count();
        }
    }

    public class BioInfo
    {
        public int amountOfInfo;
        public string text;
    }
    public int GetLavDistanceForPerson(string query, Person p)
    {
        var queryVariations = QueryHelpers.GetNameVariations(query);
        var lowestScore = int.MaxValue;
        var nameVariations = QueryHelpers
            .GetNameVariations(p.name)
            .Concat(QueryHelpers.GetNameVariations(p.kunya));

        //if (p.id == 106) Debugger.Break();

        foreach (string variation in queryVariations)
        {
            foreach (string nameVariation in nameVariations)
            {
                // check if exact match
                if (nameVariation == variation) return 0;

                var score = LevenshteinDistance.Compute(variation, nameVariation);
                lowestScore = Math.Min(lowestScore, score);
            }
        }

        return lowestScore;
    }
    public class PersonSearchResult
    {
        public int lavDistance;
        public Person person;
    }
}