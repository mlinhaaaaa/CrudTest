using CrudTest.Entities;
using CrudTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CrudTest.Controllers
{
    public class HomeController : Controller
    {

        private readonly TestContext _context;

        public HomeController(TestContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var newsList = _context.News.ToList();
            return View(newsList);
        }


        [Route("bang-dieu-khien")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("tim-kiem")]
        public async Task<IActionResult> Search(string searchString)
        {
            var news = from n in _context.News
                       select n;
            if (!string.IsNullOrEmpty(searchString))
            {
                news = news.Where(s => s.Name.Contains(searchString));
            }
            return View("Index", await news.ToListAsync());
        }

        [Route("danh-muc/{name}")]
        public async Task<IActionResult> Category(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
            if (category == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Where(n => n.Category == category.Id)
                .Include(n => n.CategoryNavigation)
                .ToListAsync();

            ViewData["NewsofCategory"] = category;
            return View(news);
        }
    }
}
