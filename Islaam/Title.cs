using System.ComponentModel.DataAnnotations;

namespace Islaam
{
    public class Title
    {
        public int Id;

        [Required]
        public string Name { get; set; }

        public Status Status { get; set; }
    }
}