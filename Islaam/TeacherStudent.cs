using System.ComponentModel.DataAnnotations;

namespace Islaam
{
    public class TeacherStudent
    {
        public int Id { get; set; }

        public Person Teacher { get; set; }
        public int? TeacherId { get; set; }

        public Person Student { get; set; }
        public int? StudentId { get; set; }

        public Subject Subject { get; set; }
        public int? SubjectId { get; set; }

        [Required]
        public string Source { get; set; }
    }
}