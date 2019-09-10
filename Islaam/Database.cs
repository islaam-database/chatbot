using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace Islaam
{
    public class Database : DbContext
    {
        private readonly string Host = "ec2-107-22-235-119.compute-1.amazonaws.com";
        private readonly string DatabaseName = "dbb0622vq4nt5t";
        private readonly int Port = 5432;

        private readonly string dbUsername = Environment.GetEnvironmentVariable("IDB_USERNAME");
        private readonly string dbPassword = Environment.GetEnvironmentVariable("IDB_PASSWORD");

        public DbSet<Book> Books { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Praise> Praises { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Generation> Generations { get; set; }
        public DbSet<TeacherStudent> TeacherStudents { get; set; }

        public Database() { }

        public Database(string dbUsername, string dbPassword)
        {
            this.dbUsername = dbUsername;
            this.dbPassword = dbPassword;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var ConnectionString = String.Join(";", new string[] {
                 $"User ID={dbUsername}",
                 $"Password={dbPassword}",
                 $"Host={Host}",
                 $"Port={Port}",
                 $"Database={DatabaseName}",
                 "SslMode=Require",
                 "TrustServerCertificate=true",
            });
            optionsBuilder.UseNpgsql(ConnectionString);
        }

        public List<PersonSearchResult> SearchForPerson(string query)
        {
            var data = People;
            var resultsQuery = data
                .Select(p => new PersonSearchResult
                {
                    Person = p,
                    lavDistance = GetLavDistanceForPerson(query, p),
                })
                .OrderBy(x => x.lavDistance)
                .ToList();
            return resultsQuery;
        }

        private int GetLavDistanceForPerson(string query, Person p)
        {
            var queryVariations = QueryHelpers.GetNameVariations(query);
            var lowestScore = int.MaxValue;
            var nameVariations = QueryHelpers
                .GetNameVariations(p.Name)
                .Concat(QueryHelpers.GetNameVariations(p.FullName));

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
    }

    public class PersonSearchResult
    {
        public int lavDistance;
        public Person Person;
    }

    public static class QueryHelpers
    {
        public static HashSet<string> GetNameVariations(string query)
        {
            return GetQueryVariations(query, null);
        }
        private static HashSet<string> GetQueryVariations(string query, HashSet<string> variations)
        {
            if (query == null) return new HashSet<string>();
            if (variations == null) variations = new HashSet<string>();
            variations.Add(query);

            var removableCharacters = new string[] { "'", "`", "-", " " };

            if (query != query.ToLower())
            {
                var lowerCaseVariations = GetQueryVariations(query.ToLower(), variations);
                variations.Concat(lowerCaseVariations);
            }

            foreach (string character in removableCharacters)
            {
                if (query.Contains(character))
                {
                    var withoutThatCharacter = query.Replace(character, "");
                    var withoutThatCharacterVariations = GetQueryVariations(withoutThatCharacter, variations);
                    variations.Concat(withoutThatCharacterVariations);
                }
            }

            return variations;
        }
    }
}

// https://data.heroku.com/datastores/8b8c90a0-9ead-44c7-b3a0-bcf62df31085#administration
