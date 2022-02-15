using System.ComponentModel.DataAnnotations;

namespace SecureDemoClassLibrary.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        [StringLength(50)]
        public string? FirstName { get; set; }
        [StringLength(50)]
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public byte[]? Image { get; set; }
        [StringLength(15)]
        public string? PictureFileExtension { get; set; }
        public byte[]? Attachment { get; set; }
        [StringLength(15)]
        public string? AttachmentFileExtension { get; set; }
    }
}
