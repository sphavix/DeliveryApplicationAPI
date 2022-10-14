using DeliveryApp.Data;
using DeliveryApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public IActionResult Get(int userId)
        {
            var user = _context.CartItems.Where(x => x.CustomerId == userId);
            if(user == null)
            {
                return NotFound();
            }

            //Get the cart by user ID using a join by LINQ and Lambda
            var cartItems = from a in _context.CartItems.Where(x => x.CustomerId == userId)
                            join p in _context.Products on a.ProductId equals p.Id
                            select new
                            {
                                Id = a.Id,
                                Price = a.Price,
                                TotalAmount = a.TotalAmount,
                                Quantity = a.Quantity,
                                ProductName = p.Title
                            };
            return Ok(cartItems);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Cart cart)
        {
            //
            var cartItem = _context.CartItems.FirstOrDefault(x => x.ProductId == cart.ProductId 
                            && x.CustomerId == cart.CustomerId);

            //Check If the cart is not empty
            if(cartItem != null)
            {
                //Increment the number of items in the cart and add up the subtotal
                cartItem.Quantity += cart.Quantity;
                cartItem.TotalAmount = cart.Price * cart.Quantity;
            }
            else
            {
                //if the cart is empty then create and add items to the shopping cart
                var shoppingCart = new Cart()
                {
                    CustomerId = cart.CustomerId,
                    ProductId = cart.ProductId,
                    Price = cart.Price,
                    Quantity = cart.Quantity,
                    TotalAmount = cart.TotalAmount,
                };
                _context.CartItems.Add(shoppingCart);
            }
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet("[action]/{userId}")]
        public IActionResult TotalCartItems(int userId)
        {
            //Get customer cart items
            var cartItems = (from cart in _context.CartItems
                             where cart.CustomerId == userId
                             select cart.Quantity).Sum();
            return Ok(new { TotalCartItems = cartItems });
        }

        [HttpGet("[action]/{userId}")]
        public IActionResult TotalAmount(int userId)
        {
            //Get the total amount in the cart
            var totalAmount = (from cart in _context.CartItems
                               where cart.CustomerId == userId
                               select cart.TotalAmount).Sum();
            return Ok(new { TotalAmount = totalAmount }) ;
        }

        [HttpDelete("userId")]
        public IActionResult Delete(int userId)
        {
            //Remove items from the cart.
            var cart = _context.CartItems.Where(x => x.CustomerId == userId);
            _context.CartItems.RemoveRange(cart);
            _context.SaveChanges();
            return Ok();
        }

    }
}
