using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Islaam
{
    public class Generation
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}