using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Islaam
{
    public class Person
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Source { get; set; }

        public Title MainTitle { get; set; }
        public int? MainTitleId { get; set; }
        public string MainTitleSource { get; set; }

        public string FullName { get; set; }
        public string FillNameSource { get; set; }

        public int? DeathYear { get; set; }
        public string DeathYearSource { get; set; }

        public int? BirthYear { get; set; }
        public string BirthYearSource { get; set; }

        public Generation Generation { get; set; }
        public int? GenerationId { get; set; }
        public string GenerationSource { get; set; }

        public int? TaqreedId { get; set; }

        [InverseProperty("Praisee")]
        public ICollection<Praise> PraisesReceived { get; set; }

        [InverseProperty("Praiser")]
        public ICollection<Praise> PraisesGiven { get; set; }

        public bool UseMascPron { get; set; }

        public string Location { get; set; }
        public string LocationSource { get; set; }

        [InverseProperty("Student")]
        public ICollection<TeacherStudent> Teachers { get; set; }

        [InverseProperty("Teacher")]
        public ICollection<TeacherStudent> Students { get; set; }

        public ICollection<Book> BooksAuthored { get; set; }

        public string FriendlyName
        {
            get
            {
                if (MainTitle != null) return MainTitle.Name + " " + Name;
                return Name;
            }
        }

        public BioInfo Bio
        {
            get
            {
                var pronoun = UseMascPron ? "He" : "She";
                var possesivePronoun = UseMascPron ? "His" : "Her";

                // start
                var biography = $"{pronoun} is ";
                var praisesReceived = PraisesReceived.ToList();
                var titles = praisesReceived
                    .Select(p => p.Title?.Name)
                    .Where(t => t != null)
                    .Distinct()
                    .ToList();
                var praiserNames = FriendlyJoin(
                    praisesReceived
                    .Select(x => x.Praiser.FriendlyName)
                    .Distinct()
                    .ToList()
                );
                var teacherNames = Teachers
                    .Select(t => t.Teacher.FriendlyName)
                    .Distinct()
                    .ToList();
                var studentNames = Students
                    .Select(t => t.Student.FriendlyName)
                    .Distinct()
                    .ToList();

                // booleans
                var hasPraises = praisesReceived.Count > 0;
                var hasTitles = titles.Count > 0;
                var hasLocation = Location != null;
                var hasKunya = FullName != null;
                var hasDeathYear = DeathYear != null;
                var hasBirthYear = BirthYear != null;
                var hasTeachers = Teachers.Count > 0; // slow?
                var hasStudents = Students.Count > 0; // slow?
                var hasGeneration = Generation != null;

                /** The amount of information in this bio **/
                var amountOfInfo = GetAmountOfInfo();

                // titles
                if (hasTitles)
                    biography += $"the {string.Join(", the ", titles)}, ";

                // name or kunya
                if (hasKunya)
                    biography += FullName;
                else
                    biography += Name;
                biography += ". ";

                if (hasGeneration)
                    biography += $"{pronoun} is from the {Generation.Name}. ";

                // birth
                if (hasBirthYear)
                    biography += $"{pronoun} was born in the year {BirthYear} AH. ";

                // location
                if (hasLocation)
                    biography += $"{pronoun} lived in {Location}. ";

                // praises
                if (hasPraises)
                    biography += $"{pronoun} was praised by {praiserNames}. ";

                // teachers
                if (hasTeachers)
                    biography += $"{pronoun} took knowledge from {FriendlyJoin(teacherNames)}. ";

                // students
                if (hasStudents)
                    biography += $"{pronoun} taught {FriendlyJoin(studentNames)}. ";

                // books
                // not yet supported

                // death year
                if (hasDeathYear)
                    biography += $"{pronoun} died in the year {DeathYear} AH.";

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
                        hasTeachers,
                        hasStudents,
                        hasGeneration
                    }.Count();
                }

                string FriendlyJoin(List<string> list)
                {
                    if (list.Count == 0) return null;
                    if (list.Count == 1) return list[0];
                    if (list.Count == 2) return $"{list[0]} and {list[1]}";

                    return $"{string.Join(", ", list.SkipLast(1))}, and {list.Last()}";
                }
            }
        }

    }

    public class BioInfo
    {
        public int amountOfInfo;
        public string text;
    }
}
