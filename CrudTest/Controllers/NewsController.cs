using CrudTest.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudTest.Controllers
{
    [Route("tin-tuc")]
    public class NewsController : Controller
    {
        private readonly TestContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public NewsController(TestContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [Route("")]
        public IActionResult Index()
        {
            var data = _context.News.Include(i => i.CategoryNavigation).ToList();
            ViewData["Category"] = _context.Categories.ToList();
            var tokenUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/token";
            ViewData["Token"] = tokenUrl;
            return View(data);
        }

        [Route("tao-moi")]
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile image, string name, string content, Guid category)
        {
            if (image != null && image.Length > 0)
            {
                var supportedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/tiff" };
                if (!supportedTypes.Contains(image.ContentType))
                {
                    ModelState.AddModelError("image", "Invalid image type. Only JPEG, PNG, GIF, BMP, and TIFF are allowed.");
                    return BadRequest(ModelState);
                }

                var newsEntry = new News
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Content = content,
                    Category = category
                };

                _context.News.Add(newsEntry);
                await _context.SaveChangesAsync();

                var fileExtension = Path.GetExtension(image.FileName);
                var newFileName = $"{newsEntry.Id}{fileExtension}";
                var relativePath = Path.Combine("Img", newFileName);
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Img");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, newFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                newsEntry.Image = relativePath;
                _context.Update(newsEntry);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "News created successfully" });
            }

            ModelState.AddModelError("image", "Image is required.");
            return BadRequest(ModelState);
        }


        [Route("GetById/{id:guid}")]
        [HttpGet]
        public IActionResult GetById(Guid id)
        {
            var data = _context.News.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                return BadRequest();
            }
            return Ok(data);
        }

        [Route("sua")]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, IFormFile image, string name, string content, Guid category)
        {
            var news = await _context.News.FirstOrDefaultAsync(x => x.Id == id);
            if (news == null)
            {
                return BadRequest(new { success = false, message = "News item not found" });
            }

            news.Name = name;
            news.Content = content;
            news.Category = category;

            if (image != null && image.Length > 0)
            {
                var supportedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/tiff" };
                if (!supportedTypes.Contains(image.ContentType))
                {
                    ModelState.AddModelError("image", "Invalid image type. Only JPEG, PNG, GIF, BMP, and TIFF are allowed.");
                    return BadRequest(ModelState);
                }

                var fileExtension = Path.GetExtension(image.FileName);
                var newFileName = $"{news.Id}{fileExtension}";
                var relativePath = Path.Combine("Img", newFileName);
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Img");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, newFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                news.Image = relativePath;
            }

            _context.News.Update(news);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "News updated successfully" });
        }



        [Route("xoa/{id:guid}")]
        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            var news = _context.News.FirstOrDefault(x => x.Id == id);
            if (news == null)
            {
                return NotFound(); // Return 404 if the news item is not found
            }

            // Construct the full path to the associated image file
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, news.Image);

            try
            {
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                else
                {
                    return BadRequest("Image file not found");
                }
                _context.News.Remove(news);
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the image file: {ex.Message}");
            }
        }


        [Route("tat-ca-tin-tuc")]
        public IActionResult GetAllNews()
        {
            var allNews = _context.News.ToList();
            return View(allNews);
        }

    }
}