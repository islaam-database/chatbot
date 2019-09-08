using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace Islaam
{
    public class Database : DbContext
    {
        private readonly string Host = "ec2-107-22-235-119.compute-1.amazonaws.com";
        private readonly string DatabaseName = "dbb0622vq4nt5t";
        private readonly int Port = 5432;
        private string dbUsername = Environment.GetEnvironmentVariable("IDB_USERNAME");
        private string dbPassword = Environment.GetEnvironmentVariable("IDB_PASSWORD");

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
    }
}

// https://data.heroku.com/datastores/8b8c90a0-9ead-44c7-b3a0-bcf62df31085#administration
