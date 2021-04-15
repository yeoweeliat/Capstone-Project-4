using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Grocery.WebApp.Data;
using Grocery.WebApp.Models;

using Microsoft.AspNetCore.Authorization;

namespace Grocery.WebApp.Areas.Manage.Controllers
{

    // you dont want ppl to access this without being logged in

    [Area("Manage")]
    [Authorize] // only a user who is authenticated and authorized is allowed to access this
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Manage/Products
        public async Task<IActionResult> Index()
        {
            //pull data from db and put in some child data
            var applicationDbContext = _context.Products.Include(p => p.CreatedByUser).Include(p => p.UpdatedByUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Manage/Products/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.CreatedByUser)
                .Include(p => p.UpdatedByUser)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Manage/Products/Create
        public IActionResult Create()
        {
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName");
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName");
            return View();
        }

        // POST: Manage/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,ProductName,Quantity,SellingPricePerUnit,Image,CreatedByUserId,UpdatedByUserId,LastUpdatedOn")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.ProductID = Guid.NewGuid();
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", product.CreatedByUserId);
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", product.UpdatedByUserId);
            return View(product);
        }

        // GET: Manage/Products/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", product.CreatedByUserId);
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", product.UpdatedByUserId);
            return View(product);
        }

        // POST: Manage/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ProductID,ProductName,Quantity,SellingPricePerUnit,Image,CreatedByUserId,UpdatedByUserId,LastUpdatedOn")] Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
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
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", product.CreatedByUserId);
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", product.UpdatedByUserId);
            return View(product);
        }

        // GET: Manage/Products/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.CreatedByUser)
                .Include(p => p.UpdatedByUser)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Manage/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
    }
}
