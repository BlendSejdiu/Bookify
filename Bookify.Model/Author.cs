using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bookify.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Full Name")]
        public string? FullName { get; set; }

    }
}
