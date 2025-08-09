using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Data.Dtos.Order
{
    public class AcceptOrderDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int StatusId { get; set; }

        [MinLength(1)]
        public List<ChooseInventoryDto> Inventories { get; set; } = [];
    }

    public class ChooseInventoryDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductItemId { get; set; }

        [MinLength(1)]
        public List<ChooseStorageDto> Storages { get; set; } = [];
    }

    public class ChooseStorageDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int StorageId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
