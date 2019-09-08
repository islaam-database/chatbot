namespace Islaam
{
    public class TeacherStudent
    {
        public Person Teacher { get; set; }
        public Person Student { get; set; }
        public Subject Subject { get; set; }
        public int SubjectId { get; set; }
    }
}