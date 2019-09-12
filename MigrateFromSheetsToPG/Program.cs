using System;
using System.Collections.Generic;
using System.Linq;

namespace MigrateFromSheetsToPG
{
    class Program
    {
        static void Main(string[] args)
        {
            //var sheetsDb = new IslaamDBClient("AIzaSyAyKho7TCDQkvI6uVWj634FVsEL_iiP-Ps");
            //var pgDB = new Islaam.Database();
            if (false)
            {
                var data = sheetsDb.StudentsAPI.GetData();
                foreach (var ts in data)
                {
                    pgDB.TeacherStudents.Add(new Islaam.TeacherStudent
                    {
                        StudentId = ts.studentId,
                        TeacherId = ts.teacherId,
                        Subject = null, //TODO
                        Source = ts.source
                    });
                }
            }
            if (false)
            {
                var praises = sheetsDb.PraisesAPI.GetData();
                foreach (var p in praises)
                {
                    pgDB.Praises.Add(new Islaam.Praise
                    {
                        PraiseeId = p.recommendeeId,
                        PraiserId = p.recommenderId,
                        Topic = null, // TODO
                        Title = pgDB.Titles.Where(x => x.Name == p.title).FirstOrDefault(),
                        Source = p.source,
                    });
                }
            }
            if (false)
            {
                var titles = sheetsDb
                    .PraisesAPI
                    .GetData()
                    .FindAll(x => x.title != null)
                    .GroupBy(x => x.title)
                    .Select(x => x.First())
                    .Select(x => x.title);

                foreach (var t in titles)
                {
                    pgDB.Titles.Add(new Islaam.Title
                    {
                        Name = t
                    });
                }
            }
            if (false)
            {
                // import people
                var people = sheetsDb.PersonAPI.GetDataFromSheet();
                foreach (Person p in people)
                {
                    pgDB.People.Add(new Islaam.Person
                    {
                        BirthYear = p.birthYear,
                        BirthYearSource = null, // done
                        BooksAuthored = null, // done
                        DeathYear = p.deathYear,
                        DeathYearSource = null, // done
                        FillNameSource = null, // done
                        FullName = p.kunya,
                        GenerationId = p.generation == null ? (int?)null : int.Parse(p.generation.Split('.').First()),
                        GenerationSource = null, // done
                        Id = p.id,
                        Location = p.location,
                        LocationSource = null, // do this?
                        Name = p.name,
                        Source = p.source,
                        TaqreedId = p.taqreebNumber,
                        UseMascPron = p.useMasculinePronoun,
                    });
                }
            }
            pgDB.SaveChanges();
        }
    }
}
