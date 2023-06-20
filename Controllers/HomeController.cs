using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RAM_APP.Models;
using Microsoft.AspNetCore.Authorization;

namespace RAM_APP.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly RentAmovieDbContext _context;

    public HomeController(ILogger<HomeController> logger, RentAmovieDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Main()
    {
        var transactions =  _context.Transactions
                                .Include(t => t.CustomerNavigation)
                                .Include(t => t.MovieNavigation)
                                .OrderByDescending(t => t.DateRented)
                                .ThenBy(t => t.CustomerNavigation.Surname)
                                .ThenBy(t => t.CustomerNavigation.FirstName)
                                .AsEnumerable();

        var thisMonth = DateTime.Now.Month;
        var monthTransactions = transactions
                                .Where(t => t.MonthRented == thisMonth)
                                .ToList();
            
        ViewBag.MonthTransactions = monthTransactions;

        //total amount of monthly transactions
        var monthlySales = monthTransactions.Sum(t => t.MovieNavigation.Amount);
        ViewBag.MonthSales = monthlySales.ToString("N2");

        //to get the number of times a movie appears in the transactions 
        var movieCounts = _context.Transactions
        .Include(t => t.MovieNavigation)
        .GroupBy(t => t.MovieNavigation.Mid)
        .Select(g => new { Movie = g.First().MovieNavigation, 
                            TransactionCount = g.Count(),
                            Revenue = (g.First().MovieNavigation.Amount*g.Count()).ToString("N2") })
        .OrderByDescending(mc => mc.TransactionCount)
        .ToList();

        ViewBag.MovieCounts = movieCounts;

        //get the total number of movies in the database
        var totalMovies = _context.Movies.Count();
        ViewBag.AllMovies = totalMovies.ToString("N0");
        //all customers
        var totalCustomers = _context.Customers.Count();
        ViewBag.AllCustomers =totalCustomers.ToString("N0");

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
