using CrudTest.Entities;
using CrudTest.Models;
using Microsoft.AspNetCore.Mvc;
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
    }
}
