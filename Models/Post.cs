using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blogio.Models
{
    public class Post
    {
        public long PostId { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Text { get; set; } = string.Empty;
        public string? Picture { get; set; }
        // [NotMapped]
        // public IFormFile? PreviewPicture { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        [Range(1, 5)]
        public int? Rating { get; set; }
        // [NotMapped][Range(1, 5)]
        // public int? NewRating { get; set; }

        public Author? Author { get; set; }
    }
}