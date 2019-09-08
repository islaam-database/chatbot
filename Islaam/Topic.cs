using System.ComponentModel.DataAnnotations;

namespace Islaam
{
    public class Topic
    {
        public int Id { get; set; }

        [Required]
        public string Name;

        public Topic ParentTopic;
    }
}