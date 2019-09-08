namespace Islaam
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Subject ParentSubject { get; set; }
        public int ParentSubjectId { get; set; }
    }
}