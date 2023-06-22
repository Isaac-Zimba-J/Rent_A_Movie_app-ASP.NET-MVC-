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
    public class MovieController : Controller
    {
        private readonly RentAmovieDbContext _context;

        public MovieController(RentAmovieDbContext context)
        {
            _context = context;
        }

        // GET: Movie
        public async Task<IActionResult> Index()
        {
            //Sort the movies in decending order of their release date, then the genre and lastly the title
            var rentAmovieDbContext = _context.Movies
            .OrderByDescending(m => m.ReleaseDate)
            .ThenBy(m => m.Genre)
            .ThenBy(m => m.Title)
            .Include(m => m.GenreNavigation)
            .ToListAsync();
            return View(await rentAmovieDbContext);
        }

        // GET: Movie/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }
            //Get the starring table, actor and movie tables join them to be able to associate the
            //actors with the movie in which they're starring
            var starring = from s in _context.Starrings
                            join a in _context.Actors on s.Actor equals a.Aid
                            join m in _context.Movies on s.Movie equals m.Mid
                            where s.Movie == id
                            select a.Name;
            var stars = await starring.ToListAsync();
            //create a view for the joined tables to be able to display them in the view
            ViewBag.Stars = stars;

            var movie = await _context.Movies
                .Include(m => m.GenreNavigation)
                .FirstOrDefaultAsync(m => m.Mid == id);
                
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }


        public async Task<IActionResult> Search(string id)
        {
            if(id == null|| _context.Movies == null){
                return RedirectToAction("Index", "Movie");
            }
            else{
            string searchTermLower = id.ToLower();
            //Sort the movies in decending order of their release date, then the genre and lastly the title
            var rentAmovieDbContext = _context.Movies
            .OrderByDescending(m => m.ReleaseDate)
            .ThenBy(m => m.Genre)
            .ThenBy(m => m.Title)
            .Include(m => m.GenreNavigation)
            .Where(m => m.Title.ToLower().Contains(searchTermLower) || 
                        m.GenreNavigation.Name.ToLower().Contains(searchTermLower))
            .ToListAsync();
            return View(await rentAmovieDbContext);
            }

        }

        

        // GET: Movie/Create
        public IActionResult Create()
        {
            ViewData["Genre"] = new SelectList(_context.Genres, "Gid", "Name");
            return View();
        }

        // POST: Movie/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mid,Genre,Title,ReleaseDate,Rating,Price,Tax")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                //since the release date is being received as a Datetime, convert it to a string before saving
                //do the same when updating details too
                DateTime releaseDate = DateTime.Parse(movie.ReleaseDate);
                movie.ReleaseDate = releaseDate.ToString("yyyy-MM-dd");
                _context.Add(movie);
                await _context.SaveChangesAsync();

                //getting the movie Id
                var movieId = movie.Mid;
                return RedirectToAction("AddActor", "Starring", new { id = movie.Mid });

            }
            ViewData["Genre"] = new SelectList(_context.Genres, "Gid", "Name", movie.Genre);
            return View(movie);
        }

        // GET: Movie/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);           
            if (movie == null)
            {
                return NotFound();
            }
            
            ViewData["Genre"] = new SelectList(_context.Genres, "Gid", "Name", movie.Genre);
            return View(movie);
        }

        // POST: Movie/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Mid,Genre,Title,ReleaseDate,Rating,Price,Tax")] Movie movie)
        {
            if (id != movie.Mid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    DateTime releaseDate = DateTime.Parse(movie.ReleaseDate);
                    movie.ReleaseDate = releaseDate.ToString("yyyy-MM-dd");
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Mid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                var movieId = movie.Mid;
                return RedirectToAction("AddActor", "Starring", new { id = movie.Mid });
            }
            ViewData["Genre"] = new SelectList(_context.Genres, "Gid", "Name", movie.Genre);
            return View(movie);
        }

        // GET: Movie/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.GenreNavigation)
                .FirstOrDefaultAsync(m => m.Mid == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Movies == null)
            {
                return Problem("Entity set 'RentAmovieDbContext.Movies'  is null.");
            }
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(long id)
        {
          return (_context.Movies?.Any(e => e.Mid == id)).GetValueOrDefault();
        }
    }
}
