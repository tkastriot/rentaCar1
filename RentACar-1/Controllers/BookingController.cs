using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentACar_1.Models;
using System;
using System.Threading.Tasks;
using RentACar_1.Models;
using RentACar_1.Data;
using Microsoft.AspNetCore.Identity;
using RentACar_1.ViewModels;

public class BookingController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;


    public BookingController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }


    public async Task<IActionResult> BookingDetails(int bookingID)
    {
        if (bookingID == null)
        {
            return NotFound();
        }

        var booking = await _dbContext.Bookings
        .Include(b => b.Car)
        .ThenInclude(c => c.CarDetail)
        .Include(b => b.Renter)
        .FirstOrDefaultAsync(m => m.BookingID == bookingID);

        if (booking == null)
        {
            return NotFound();
        }

        return View("BookingDetails", booking);
    }
    public IActionResult BookingsByCar(int carID)
    {
        if (carID == null)
        {
            return NotFound();
        }

        var bookings = _dbContext.Bookings
            .Include(b => b.Car)
            .ThenInclude(c => c.CarDetail)
            .Include(b => b.Renter)
            .Where(b => b.CarID == carID);

        if (bookings == null)
        {
            return NotFound();
        }

        return View("Index", bookings);
    }
    public IActionResult BookingsByRenter(string renterID)
    {
        if (renterID == null)
        {
            return NotFound();
        }

        var bookings = _dbContext.Bookings
              .Include(b => b.Car)
              .ThenInclude(c => c.CarDetail)
              .Include(b => b.Renter)
              .Where(b => b.RenterID == renterID);

        if (bookings == null)
        {
            return NotFound();
        }

        return View("Index", bookings);
    }
}
