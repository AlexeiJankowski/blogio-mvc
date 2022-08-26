using System.ComponentModel.DataAnnotations;

namespace Blogio.Models
{
    public class CommentViewModel
    {
        [Required]
        public string Text { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public string UserName { get; set; } = string.Empty;
    }
}