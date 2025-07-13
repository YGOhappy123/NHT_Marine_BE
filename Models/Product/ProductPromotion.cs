using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Models.Product
{
    public class ProductPromotion
    {
        public int? PromotionId { get; set; }
        public int? ProductId { get; set; }
        public Promotion? Promotion { get; set; }
        public RootProduct? Product { get; set; }
    }
}
