using CrudTest.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudTest.Controllers
{
    public class NewsController : Controller
    {
        private readonly TestContext _context;
        public NewsController(TestContext context) {
            _context = context;
        }
        public IActionResult Index()
        {
            var data = _context.News.Include(i => i.CategoryNavigation).ToList();
            ViewData["Category"] = _context.Categories.ToList();
            return View(data);
        }

        [HttpPost]
        public IActionResult Create(string name, string content, Guid category)
        {
            News news = new News()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Content = content,
                Category = category
            };
            _context.News.Add(news);
            _context.SaveChanges();
            return Ok(news);
        }

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
        [HttpPost]
        public IActionResult Edit(Guid id, string name, string content, Guid category)
        {
            var news = _context.News.FirstOrDefault(x => x.Id == id);
            news.Name = name;
            news.Content = content;
            news.Category = category;
            _context.News.Update(news);
            _context.SaveChanges();
            return Ok(news);
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            var news = _context.News.FirstOrDefault(x => x.Id == id);
            _context.News.Remove(news);
            _context.SaveChanges();
            return Ok();
        }

        public async Task<IActionResult> Category(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Where(n => n.Category == id)
                .Include(n => n.CategoryNavigation)
                .ToListAsync();

            ViewData["Category"] = category;
            return View(news);
        }
    }
}
