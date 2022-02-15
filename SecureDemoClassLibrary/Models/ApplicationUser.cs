using Microsoft.AspNetCore.Identity;
using SecureDemoClassLibrary.Helpers;
using System.ComponentModel.DataAnnotations;

namespace SecureDemoClassLibrary.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(50)]
        public string UserEntityType { get; set; } = string.Empty;

        public int UserEntityId { get; set; }

        public bool IsActive { get; set; }

    }
}
