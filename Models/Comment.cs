using System.ComponentModel.DataAnnotations;

namespace Blogio.Models
{
    public class Comment
    {
        [Key]
        public long CommentId { get; set; }
        [Required]
        public string Text { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Post Post { get; set; }
        public long PostId { get; set; }
    }
}