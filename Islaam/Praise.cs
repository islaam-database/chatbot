using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Islaam
{
    public class Praise
    {
        [Required]
        public Person Praiser { get; set; }

        [Required]
        public int PraiserId { get; set; }

        [Required]
        public Person Praisee { get; set; }

        [Required]
        public int PraiseeId { get; set; }

        [Required]
        public string Source { get; set; }

        public int TitleId { get; set; }
        public Title Title { get; set; }

        public int TopicId { get; set; }
        public Topic Topic { get; set; }
    }
}