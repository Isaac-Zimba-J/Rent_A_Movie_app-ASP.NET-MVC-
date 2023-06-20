using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RAM_APP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;


namespace RAM_APP.Controllers
{
    [Authorize]
    public class StarringController : Controller
    {
        private readonly ILogger<StarringController> _logger;
        private readonly RentAmovieDbContext _context;

        public StarringController(ILogger<StarringController> logger,RentAmovieDbContext context)
        {
            _logger = logger;
            _context = context;
        }



        //Adding actors to the movie 
        //GET: Movie/AddActor
        public async Task<IActionResult> AddActor(long? id)
        {
            ViewData["Actor"] = new SelectList(_context.Actors, "Aid", "Name");

            //Get the starring table, actor and movie tables join them to be able to associate the
            //actors with the movie in which they're starring
            var actors = from s in _context.Starrings
                            join a in _context.Actors on s.Actor equals a.Aid
                            join m in _context.Movies on s.Movie equals m.Mid
                            where s.Movie == id
                            select new{a.Name, s.Sid, m.Mid, m.Title};
            var stars = await actors.ToListAsync();

            
            //create a viewBag for the joined tables to be able to display them in the view
            ViewBag.Stars = stars;

            return View();   
        }

        //POST: Starring/AddActor/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActor(long? id, [Bind("Sid,Movie,Actor")] Starring starring)
        {
            if (ModelState.IsValid)
                {

                    


                    try
    {
        // Your code that performs the database operation
        // For example, inserting a row into the Starring table

        starring.Movie = id;
        _context.Add(starring);
        await _context.SaveChangesAsync();
    }
    catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 19)
    {
        // Handle the UNIQUE constraint violation error
        // For example, log an error message and redirect to another page

        _logger.LogError("The actor and movie combination already exists.");
        TempData["ErrorMessage"] = "The actor and movie combination already exists.";
        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        // Handle other exceptions if necessary
        // For example, display a generic error message
        TempData["ErrorMessage"] = "The actor has already been added to the movie.";
        Console.WriteLine("An error occurred: " + ex.Message);
        return RedirectToAction("AddActor");
    }




                //dont



                return RedirectToAction(nameof(AddActor), new {id = id});
            }
            //Get the starring table, actor and movie tables join them to be able to associate the
            //actors with the movie in which they're starring
            var actors = from s in _context.Starrings
                            join a in _context.Actors on s.Actor equals a.Aid
                            join m in _context.Movies on s.Movie equals m.Mid
                            where s.Movie == id
                            select new{a.Name, s.Sid};
            var stars = await actors.ToListAsync();
            //create a viewBag for the joined tables to be able to display them in the view
            ViewBag.Stars = stars;

            ViewData["Actor"] = new SelectList(_context.Actors, "Aid", "Name", starring.Actor);
            return View(starring);
        }

        // GET: Starring
        public async Task<IActionResult> Index()
        {
            var rentAmovieDbContext = _context.Starrings.Include(s => s.ActorNavigation)
                                            .Include(s => s.MovieNavigation)
                                            .OrderBy(s => s.ActorNavigation.Name);
            return View(await rentAmovieDbContext.ToListAsync());
        }

        // GET: Starring/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Starrings == null)
            {
                return NotFound();
            }

            var starring = await _context.Starrings
                .Include(s => s.ActorNavigation)
                .Include(s => s.MovieNavigation)
                .FirstOrDefaultAsync(m => m.Sid == id);
            if (starring == null)
            {
                return NotFound();
            }

            return View(starring);
        }

        // GET: Starring/Create
        public IActionResult Create()
        {
            ViewData["Actor"] = new SelectList(_context.Actors, "Aid", "Name");
            ViewData["Movie"] = new SelectList(_context.Movies, "Mid", "Title");
            return View();
        }

        // POST: Starring/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Sid,Actor,Movie")] Starring starring)
        {
            if (ModelState.IsValid)
            {
                _context.Add(starring);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Actor"] = new SelectList(_context.Actors, "Aid", "Name", starring.Actor);
            ViewData["Movie"] = new SelectList(_context.Movies, "Mid", "Title", starring.Movie);
            return View(starring);
        }

        // GET: Starring/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Starrings == null)
            {
                return NotFound();
            }

            var starring = await _context.Starrings.FindAsync(id);
            if (starring == null)
            {
                return NotFound();
            }
            ViewData["Actor"] = new SelectList(_context.Actors, "Aid", "Name", starring.Actor);
            ViewData["Movie"] = new SelectList(_context.Movies, "Mid", "Title", starring.Movie);
            return View(starring);
        }

        // POST: Starring/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Sid,Actor,Movie")] Starring starring)
        {
            if (id != starring.Sid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(starring);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StarringExists(starring.Sid))
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
            ViewData["Actor"] = new SelectList(_context.Actors, "Aid", "Name", starring.Actor);
            ViewData["Movie"] = new SelectList(_context.Movies, "Mid", "Title", starring.Movie);
            return View(starring);
        }

        // GET: Starring/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Starrings == null)
            {
                return NotFound();
            }

            var starring = await _context.Starrings
                .Include(s => s.ActorNavigation)
                .Include(s => s.MovieNavigation)
                .FirstOrDefaultAsync(m => m.Sid == id);
            if (starring == null)
            {
                return NotFound();
            }

            return View(starring);
        }



        // POST: Starring/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            
            if (_context.Starrings == null)
            {
                return Problem("Entity set 'RentAmovieDbContext.Starrings'  is null.");
            }
            var starring = await _context.Starrings.FindAsync(id);
            if (starring != null)
            {
                _context.Starrings.Remove(starring);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteFromMovie(long? id, long? mid)
        {
            if (id == null || _context.Starrings == null)
            {
                return NotFound();
            }

            var starring = await _context.Starrings
                .Include(s => s.ActorNavigation)
                .Include(s => s.MovieNavigation)
                .FirstOrDefaultAsync(m => m.Sid == id);
            
          
            if (starring == null)
            {
                return NotFound();
            }

            return View(starring);
        }

        // POST: Starring/DeleteFromMovie/5?mid=14
        [HttpPost, ActionName("Yes")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFromMovieConfirmed(long? id, long mid)
        {
            
            if (_context.Starrings == null)
            {
                return Problem("Entity set 'RentAmovieDbContext.Starrings'  is null.");
            }
            var starring = await _context.Starrings.FindAsync(id);
            
            if (starring != null)
            {
                _context.Starrings.Remove(starring);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("AddActor", "Starring", new {id=mid});
        }

        private bool StarringExists(long id)
        {
          return (_context.Starrings?.Any(e => e.Sid == id)).GetValueOrDefault();
        }
    }
}
