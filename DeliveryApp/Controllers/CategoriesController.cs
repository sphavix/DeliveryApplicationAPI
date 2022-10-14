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
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var categories = from c in _context.Categories
                             select new { c.Id, c.CategoryName, c.ImageUrl };
            return Ok(categories);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var category = (from c in _context.Categories
                            where c.Id == id
                            select new
                            {
                                Id = c.Id,
                                CategoryName = c.CategoryName,
                                ImageUrl = c.ImageUrl
                            }).FirstOrDefault();
            return Ok(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromBody] Category category)
        {
            var stream = new MemoryStream(category.ImageByte);
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
                category.ImageUrl = file;
                _context.Categories.Add(category);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }
            
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public IActionResult Put(int id, [FromBody] Category category)
        {
            var entity = _context.Categories.Find(id);
            if(entity == null)
            {
                return NotFound("Category Not Found");
            }
            var stream = new MemoryStream(category.ImageByte);
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
                entity.CategoryName = category.CategoryName;
                entity.ImageUrl = file;
                
                _context.SaveChanges();
                return Ok("Changes Saved!");
            }
            
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if(category == null)
            {
                return NotFound("Category Not Found");
            }
            else
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return Ok("Category Deleted!");
            }
        }


    }
}
