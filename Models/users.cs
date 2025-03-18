
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stationery_Management_System.Models
{
    public class users
    {

        [Key]
        public int userId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "name cannot be null")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        [Phone]
        public string UserPhone { get; set; }

        [Required]
        [Range(8,32, ErrorMessage = "User Password cannot be null")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

       
       [Required]
        public int UserRole { get; set; }
        [ForeignKey("UserRole")]
        public virtual UserRoles Roles { get; set; }

        [Required]
        public int UserLimits { get; set; }

        public int? Add_By { get; set; }

        [ForeignKey("Add_By")]
        public virtual users? AddedBy { get; set; }
    }
}
