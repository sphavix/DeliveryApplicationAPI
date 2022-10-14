using System.ComponentModel.DataAnnotations;

namespace DeliveryApp.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public double Total { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsCompleted { get; set; }
        public int UserId { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
