using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Blogio.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string? NickName { get; set; }
    }
}