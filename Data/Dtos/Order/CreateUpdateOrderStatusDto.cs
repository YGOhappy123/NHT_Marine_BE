using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Order
{
    public class CreateUpdateOrderStatusDto
    {
        [Required(ErrorMessage = "Tên trạng thái không được để trống.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsDefaultState { get; set; } = false;

        public bool IsAccounted { get; set; } = false;

        public bool IsUnfulfilled { get; set; } = false;
    }
}
