using System.ComponentModel.DataAnnotations;

namespace Stationery_Management_System.Models
{
    public class UserRoles
    {
        [Key]
        public int UserRoleId { get; set; }
        public string UserRoleName { get; set; }
    }
}
