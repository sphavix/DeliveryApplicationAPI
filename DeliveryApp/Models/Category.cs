using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }

        [NotMapped]
        public byte[] ImageByte { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
