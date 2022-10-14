using DeliveryApp.Data;
using DeliveryApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Order order)
        {
            //Ensure the order is not completed and is newly created with date now and save.
            order.IsCompleted = false;
            order.OrderDate = DateTime.Now;
            _context.Orders.Add(order);
            _context.SaveChanges();

            //Calculate cart items for the order
            var cartItems = _context.CartItems.Where(x => x.CustomerId == order.UserId);
            foreach(var item in cartItems)
            {
                //retrieve cart items and create new order with order details
                var orderDetails = new OrderDetail()
                {
                    Price = item.Price,
                    OrderTotal = item.TotalAmount,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId,
                    OrderId = order.Id
                };
                _context.OrderDetails.Add(orderDetails);
            }
            _context.SaveChanges();
            _context.CartItems.RemoveRange(cartItems); //Remove cart items from the cart after saving to order details
            _context.SaveChanges();
            return Ok(new { orderId = order.Id });

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public IActionResult PendingOrders()
        {
            //get pending orders
            var pendingOrders = _context.Orders.Where(x => x.IsCompleted == false);
            return Ok(pendingOrders);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public IActionResult CompletedOrders()
        {
            //get completed orders
            var pendingOrders = _context.Orders.Where(x => x.IsCompleted == true);
            return Ok(pendingOrders);
        }

        [HttpGet("[action]/{orderId}")] //Customer can view order details
        public IActionResult OrderDetails(int orderId)
        {
            //Get order details
            var orderDetails = _context.Orders.Where(x => x.Id == orderId).Include(x => x.OrderDetails)
                                .ThenInclude(x => x.Product);
            return Ok(orderDetails);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public IActionResult TotalOrders()
        {
            //Get total order for all customer which are not completed
            var orders = (from order in _context.Orders
                          where order.IsCompleted == false
                          select order.IsCompleted).Count();
            return Ok(new { PendingOrders = orders });
        }

        [HttpGet("[action]/{userId}")]
        public IActionResult OrdersByUser(int userId)
        {
            //Get user orders
            var orders = _context.Orders.Where(x => x.UserId == userId).OrderByDescending(x => x.OrderDate);
            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("[action]/{orderId}")]
        public IActionResult MarkOrdersCompleted(int orderId, [FromBody] Order order)
        {
            //Find completed order
            var orderCompleted = _context.Orders.Find(orderId);
            if(orderCompleted == null)
            {
                return NotFound("Order Not Found");
            }
            else
            {
                orderCompleted.IsCompleted = order.IsCompleted;
                _context.SaveChanges();
                return Ok("Order Completed!");
            }
        }


    }
}
