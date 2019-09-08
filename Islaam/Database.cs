using System;
using Microsoft.EntityFrameworkCore;

namespace Islaam
{
    public class Database : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Generation> Generations { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Praise> Praises { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TeacherStudent> TeacherStudents { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Topic> Topics { get; set; }
    }
}
