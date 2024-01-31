using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentACar_1.Models;
using System;
using System.Threading.Tasks;
using RentACar_1.Models;
using RentACar_1.Data;
using Microsoft.AspNetCore.Identity;
using RentACar_1.ViewModels;
using System.Security.Claims;

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

		var bookings = await _dbContext.Bookings
	  .Include(b => b.Car)
	  .ThenInclude(c => c.CarDetail)
	  .Include(b => b.Renter)
	  .FirstOrDefaultAsync(m => m.BookingID == bookingID);

		if (bookings == null)
		{
			return NotFound();
		}

		return View("BookingDetails", bookings);
    }

    public IActionResult MyBookings()
    {

        var currentUserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserID == null)
        {
            return NotFound();
        }

		var bookings = _dbContext.Bookings
              .Include(b => b.Car)
              .ThenInclude(c => c.CarDetail)
              .Include(b => b.Renter)
              .Where(b => b.RenterID == currentUserID).ToList();

        if (bookings == null)
        {
            return NotFound();
        }

        return View("MyBookings", bookings);
    }
}
