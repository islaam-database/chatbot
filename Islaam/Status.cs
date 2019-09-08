using System.ComponentModel.DataAnnotations;

namespace Islaam
{
    public class Status
    {
        [Required]
        public int Rank { get; set; }

        public string Name { get; set; }
    }
}