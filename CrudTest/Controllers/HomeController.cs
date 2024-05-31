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

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

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
    }
}
