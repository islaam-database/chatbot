using System;
using Microsoft.EntityFrameworkCore;

namespace Islaam
{
    public class Database : DbContext
    {
        public DbSet<Person> People { get; set; }
        public Database()
        {
        }
    }
}
