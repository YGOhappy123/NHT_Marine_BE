using NHT_Marine_BE.Models.Product;

namespace NHT_Marine_BE.Models.User
{
    public class CartItem
    {
        public int? CartId { get; set; }
        public int? ProductItemId { get; set; }
        public int Quantity { get; set; } = 1;
        public CustomerCart? Cart { get; set; }
        public ProductItem? ProductItem { get; set; }
    }
}
