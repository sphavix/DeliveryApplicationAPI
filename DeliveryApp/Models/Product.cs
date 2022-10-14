using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DeliveryApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUri { get; set; }
        public double Price { get; set; }
        public bool IsPopular { get; set; }
        public int CategoryId { get; set; }

        [NotMapped]
        public byte[] ImageByte { get; set; }

        [JsonIgnore]
        public ICollection<OrderDetail> OrderDetails { get; set; }

        [JsonIgnore]
        public ICollection<Cart> CartItems { get; set; }
    }
}
