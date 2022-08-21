using System.ComponentModel.DataAnnotations;

namespace Blogio.Models
{
    public class Author
    {
        [Key]
        public string? NickName { get; set; }

        public List<Post>? Posts { get; set; }
    }
}