using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Auth
{
    public class CreateUpdateRoleDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public List<int> Permissions { get; set; } = [];
    }
}
