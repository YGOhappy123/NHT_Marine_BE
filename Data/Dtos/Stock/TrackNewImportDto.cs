using System.ComponentModel.DataAnnotations;
using NHT_Marine_BE.Data.Validators;

namespace NHT_Marine_BE.Data.Dtos.Stock
{
    public class TrackNewImportDto
    {
        [Required]
        public int ImportId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        public DateTime ImportDate { get; set; }

        [Required]
        [MinLength(1)]
        public List<TrackNewImportItemDto> Items { get; set; } = [];
    }

    public class TrackNewImportItemDto
    {
        [Required]
        public int ProductItemId { get; set; }

        [DivisibleBy(100)]
        public int Cost { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
