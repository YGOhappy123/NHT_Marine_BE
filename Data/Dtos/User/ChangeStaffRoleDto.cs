using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.User
{
    public class ChangeStaffRoleDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int RoleId { get; set; }
    }
}
