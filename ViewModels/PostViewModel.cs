using System.ComponentModel.DataAnnotations;
using Blogio.Models;

namespace Blogio.ViewModels
{
    public class PostViewModelBase
    {
        public long PostId { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Text { get; set; }
        public string? Picture { get; set; }
        public IFormFile? PreviewPicture { get; set; }
        public DateTime Date { get; set; }
        [Range(1, 5)]
        public int? Rating { get; set; }
        [Range(1, 5)]
        public int? NewRating { get; set; }
        public Author? Author { get; set; }
    }

    public class PostViewModel : PostViewModelBase
    {
        public List<Comment>? Comments { get; set; }
        public Comment? NewComment { get; set; }
    }
}