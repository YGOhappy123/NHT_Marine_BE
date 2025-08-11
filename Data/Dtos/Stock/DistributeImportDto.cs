using System.ComponentModel.DataAnnotations;
using NHT_Marine_BE.Data.Dtos.Order;

namespace NHT_Marine_BE.Data.Dtos.Stock
{
    public class DistributeImportDto
    {
        [MinLength(1)]
        public List<ChooseInventoryDto> Items { get; set; } = [];
    }
}
