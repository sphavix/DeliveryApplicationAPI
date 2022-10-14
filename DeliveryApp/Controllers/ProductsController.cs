using DeliveryApp.Data;
using DeliveryApp.Models;
using ImageUploader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var products = (from p in _context.Products
                            select p);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(_context.Products.Find(id));
        }

        [HttpGet("[action]/{CategoryId}")]
        public IActionResult ProductsByCategory(int CategoryId)
        {
            var products = from x in _context.Products
                           where x.CategoryId == CategoryId
                           select new
                           {
                               Id = x.Id,
                               Title = x.Title,
                               Price = x.Price,
                               Description = x.Description,
                               CategoryId = x.CategoryId,
                               ImageUrl = x.ImageUri
                           };
            return Ok(products);
        }

        [HttpGet("[action]")]
        public IActionResult PopularProducts()
        {
            var products = from x in _context.Products
                          where x.IsPopular == true
                          select new
                          {
                              Id = x.Id,
                              Title = x.Title,
                              Price = x.Price,
                              ImageUri = x.ImageUri
                          };
            return Ok(products);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromBody] Product product)
        {
            var stream = new MemoryStream(product.ImageByte);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "wwwroot";
            var response = FilesHelper.UploadImage(stream, folder, file);
            if (!response)
            {
                return BadRequest();
            }
            else
            {
                product.ImageUri = file;
                _context.Products.Add(product);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Put(int id, [FromBody] Product product)
        {
            var entity = _context.Products.Find(id);
            if (entity == null)
            {
                return NotFound("Product Not Found");
            }
            var stream = new MemoryStream(product.ImageByte);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "wwwroot";
            var response = FilesHelper.UploadImage(stream, folder, file);
            if (!response)
            {
                return BadRequest();
            }
            else
            {
                entity.Title = product.Title;
                entity.Price = product.Price;
                entity.Description = product.Description;
                entity.CategoryId = product.CategoryId;
                entity.ImageUri = file;

                _context.SaveChanges();
                return Ok("Product Saved!");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound("Product Not Found");
            }
            else
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
                return Ok("Product Deleted!");
            }
        }
    }
}
