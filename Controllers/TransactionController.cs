using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RAM_APP.Models;
using Microsoft.AspNetCore.Authorization;

namespace RAM_APP.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly RentAmovieDbContext _context;

        public TransactionController(RentAmovieDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
             var rentAmovieDbContext = _context.Transactions
                                        .Include(t => t.CustomerNavigation)
                                        .Include(t => t.MovieNavigation)
                                        .OrderByDescending(t => t.DateRented)
                                        .ThenBy(t => t.CustomerNavigation.Surname)
                                        .ThenBy(t => t.CustomerNavigation.FirstName)
                                        .ToListAsync();
            return View(await rentAmovieDbContext);
        }

        //GET: Transaction/ByCustomer/8
        public async Task<IActionResult> ByCustomer(long? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.CustomerNavigation)
                .Include(t => t.MovieNavigation)
                .Where(t => t.CustomerNavigation.Cid == id)
                .OrderByDescending(t => t.DateRented)
                .ToListAsync();

            //get the total amount for the customer's transactions
            ViewBag.CtotalTransactions = transaction.Sum(t => t.MovieNavigation.Amount);

            //get the first name of the customer
            var CName = _context.Transactions
                .Include(t => t.CustomerNavigation)
                .Include(t => t.MovieNavigation)
                .Where(t => t.CustomerNavigation.Cid == id)
                .GroupBy(t => t.CustomerNavigation.Cid)
                .Select(n => new {FName = n.First().CustomerNavigation.FirstName})
                .FirstOrDefault();

            ViewBag.CustomerName = CName;

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);         

        }

        // GET: Transaction/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.CustomerNavigation)
                .Include(t => t.MovieNavigation)
                .FirstOrDefaultAsync(m => m.Tid == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transaction/Create
        public IActionResult Create()
        {
            ViewData["Customer"] = new SelectList(_context.Customers, "Cid", "FirstName");
            ViewData["Movie"] = new SelectList(_context.Movies, "Mid", "Title");
            return View();
        }

        // POST: Transaction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Tid,Customer,Movie,DateRented")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                //Get the date and time in a datetime format then convert to string before storing in the database
                //Do the same even when editing the records... in the Edit's post action.
                DateTime dateRented = DateTime.Parse(transaction.DateRented);
                transaction.DateRented = dateRented.ToString("yyyy-MM-dd, HH:MM");
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Customer"] = new SelectList(_context.Customers, "Cid", "FirstName", transaction.Customer);
            ViewData["Movie"] = new SelectList(_context.Movies, "Mid", "Title", transaction.Movie);
            return View(transaction);
        }

        // GET: Transaction/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            DateTime dateRented = DateTime.Parse(transaction.DateRented);
                    transaction.DateRented = dateRented.ToString("dd MMM yyyy HH:MM");
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["Customer"] = new SelectList(_context.Customers, "Cid", "FirstName", transaction.Customer);
            ViewData["Movie"] = new SelectList(_context.Movies, "Mid", "Title", transaction.Movie);
            return View(transaction);
        }

        // POST: Transaction/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Tid,Customer,Movie,DateRented")] Transaction transaction)
        {
            if (id != transaction.Tid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                     DateTime dateRented = DateTime.Parse(transaction.DateRented);
                    transaction.DateRented = dateRented.ToString("yyyy-MM-dd, HH:MM");
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Tid))
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
            ViewData["Customer"] = new SelectList(_context.Customers, "Cid", "FirstName", transaction.Customer);
            ViewData["Movie"] = new SelectList(_context.Movies, "Mid", "Title", transaction.Movie);
            return View(transaction);
        }

        // GET: Transaction/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.CustomerNavigation)
                .Include(t => t.MovieNavigation)
                .FirstOrDefaultAsync(m => m.Tid == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'RentAmovieDbContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(long id)
        {
          return (_context.Transactions?.Any(e => e.Tid == id)).GetValueOrDefault();
        }
    }
}
