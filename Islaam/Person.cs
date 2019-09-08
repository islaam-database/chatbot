using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Islaam
{
    public class Person
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Source { get; set; }

        public Praise MainTitle { get; set; }
        public int? MainTitleId { get; set; }

        public string FullName { get; set; }
        public string FillNameSource { get; set; }

        public string DeathYear { get; set; }
        public string DeathYearSource { get; set; }

        public string BirthYear { get; set; }
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
        public List<TeacherStudent> Teachers { get; set; }

        [InverseProperty("Teacher")]
        public List<TeacherStudent> Students { get; set; }

        public List<Book> BooksAuthored { get; set; }
    }
}