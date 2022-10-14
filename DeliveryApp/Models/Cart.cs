namespace DeliveryApp.Models
{
    public class Cart
    {

        public int Id { get; set; }
        public int ProductId { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double TotalAmount { get; set; }
        public int CustomerId { get; set; }
    }
}
