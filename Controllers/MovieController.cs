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
    [Authorize] // Requires users to be authenticated to access this controller
    public class MovieController : Controller
    {
        private readonly RentAmovieDbContext _context;

        public MovieController(RentAmovieDbContext context)
        {
            _context = context;
        }

        // GET: Movie
        // Displays a list of movies sorted by release date, genre, and title in descending order
        public async Task<IActionResult> Index()
        {
            var rentAmovieDbContext = _context.Movies
                .OrderByDescending(m => m.ReleaseDate)
                .ThenBy(m => m.Genre)
                .ThenBy(m => m.Title)
                .Include(m => m.GenreNavigation)
                .ToListAsync();
            return View(await rentAmovieDbContext);
        }

        // GET: Movie/Details/5
        // Displays details of a specific movie, including the actors starring in it
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            // Joining tables to get the actors associated with the movie
            var starring = from s in _context.Starrings
                           join a in _context.Actors on s.Actor equals a.Aid
                           join m in _context.Movies on s.Movie equals m.Mid
                           where s.Movie == id
                           select a.Name;
            var stars = await starring.ToListAsync();
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

        // GET: Movie/Search
        // Searches for movies by title or genre and displays the results
        public async Task<IActionResult> Search(string id)
        {
            if (id == null || _context.Movies == null)
            {
                return RedirectToAction("Index", "Movie");
            }
            else
            {
                string searchTermLower = id.ToLower();
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
        // Displays a form to create a new movie
        public IActionResult Create()
        {
            ViewData["Genre"] = new SelectList(_context.Genres, "Gid", "Name");
            return View();
        }

        // POST: Movie/Create
        // Creates a new movie in the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mid,Genre,Title,ReleaseDate,Rating,Price,Tax")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                // Convert release date to string before saving it to the database
                DateTime releaseDate = DateTime.Parse(movie.ReleaseDate);
                movie.ReleaseDate = releaseDate.ToString("yyyy-MM-dd");
                _context.Add(movie);
                await _context.SaveChangesAsync();

                // Redirect to the AddActor action in the Starring controller to associate actors with the new movie
                var movieId = movie.Mid;
                return RedirectToAction("AddActor", "Starring", new { id = movie.Mid });
            }

            ViewData["Genre"] = new SelectList(_context.Genres, "Gid", "Name", movie.Genre);
            return View(movie);
        }

        // GET: Movie/Edit/5
        // Displays a form to edit an existing movie
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
        // Updates an existing movie in the database
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
                    // Convert release date to string before updating it in the database
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
        // Displays a confirmation page to delete a movie
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
        // Deletes a movie from the database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Movies == null)
            {
                return Problem("Entity set 'RentAmovieDbContext.Movies' is null.");
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
