using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Models.Stock
{
    public class DamageType
    {
        [Key]
        public int TypeId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
