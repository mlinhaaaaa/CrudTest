using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrudTest.Entities;

namespace CrudTest.Controllers
{
    [Route("danh-muc")]
    public class CategoriesController : Controller
    {
        private readonly TestContext _context;

        public CategoriesController(TestContext context)
        {
            _context = context;
        }

        // GET: Categories
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewData["Category"] = categories;
            return View(categories);
        }


        // GET: Categories/Create
        [Route("tao-moi")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("tao-moi")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.Id = Guid.NewGuid();
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        [Route("sua/{name}")]
        public async Task<IActionResult> Edit(string name)
        {
            if (name == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("sua/{name}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string name, [Bind("Id,Name")] Category category)
        {
            if (name != category.Name)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        [Route("xoa/{id:guid}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("xoa/{id:guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                var newsList = await _context.News.Where(n => n.Category == id).ToListAsync();
                foreach (var news in newsList)
                {
                    news.IsDeleted = true;
                    _context.Update(news);
                }
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(Guid id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        [Route("slidebar")]
        public IActionResult GetDataHeader()
        {
            var categories = _context.Categories.ToList();
            return PartialView("_CategoryHeader", categories);
        }

        [Route("tin-tuc/{id:int}")]
        public IActionResult Category(int id)
        {
            var categoryId = id.ToString();
            var category = _context.Categories
                .Include(c => c.News)
                .FirstOrDefault(c => c.Id.ToString() == categoryId);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
    }
}