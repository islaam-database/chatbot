using System.ComponentModel.DataAnnotations;

namespace Islaam
{
    public class Title
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Status Status { get; set; }
        public int? StatusId { get; set; }

        public string TestFunction()
        {
            return "test return value";
        }
    }
}