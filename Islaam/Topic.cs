using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Islaam
{
    public class Topic
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Topic ParentTopic { get; set; }
        public List<Topic> ChildTopics { get; set; }
    }
}