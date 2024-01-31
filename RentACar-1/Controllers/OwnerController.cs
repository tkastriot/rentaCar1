using Microsoft.AspNetCore.Mvc;
using RentACar_1.ViewModels;
using RentACar_1.Models;
using RentACar_1.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

public class OwnerController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public OwnerController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    // GET: Owner/AddCar
    public IActionResult AddCar()
    {
        return View();
    }

    // POST: Owner/AddCar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCar(AddCarViewModel model)
    {

        var currentUser = await _userManager.GetUserAsync(User);

        // Get the current user's ID
        if (currentUser == null)
        {
            return Redirect("~/Identity/Account/Login");
            // Now you have the userId to use as needed
        }

        if (ModelState.IsValid)
        {
            // Create CarDetails entity

            var carDetail = new CarDetails
            {
                Brand = model.Brand,
                Category = model.Category,
                Year = model.Year,
                Description = model.Description,
                IsAutomatic = model.IsAutomatic,
                FuelType = model.FuelType,
                Power = model.Power,
                City = model.City,
                ImageUrl = model.ImageUrl,
            };

            // Add CarDetails to DbContext
            _dbContext.CarDetails.Add(carDetail);
            await _dbContext.SaveChangesAsync();

            // Create Car entity
            var car = new Car
            {
                CarDetailID = carDetail.CarDetailsID, // Assign the ID of the newly created CarDetails
                RegistrationNumber = model.RegistrationNumber,
                PricePerDay = model.PricePerDay,
                IsAvailable = true, // Assuming the car is available initially
                OwnerID = currentUser.Id // Assuming you'll pass the OwnerID
            };

            // Add Car to DbContext
            _dbContext.Cars.Add(car);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home"); // Redirect to home page or any other page
        }

        return View(model);
    }

    // GET: Owner/MyCars

    public IActionResult MyCars()
    {
        var currentUserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var ownerCars = _dbContext.Cars
            .Include(c => c.CarDetail)
            .Include(c => c.Bookings) // Explicitly include the Bookings navigation property
            .Where(c => c.OwnerID == currentUserID)
            .ToList();

        var viewModel = ownerCars.Select(car => new OwnerCarsViewModel
        {
            CarId = car.CarID,
            Brand = car.CarDetail?.Brand ?? "Unknown Brand",
            Category = car.CarDetail?.Category ?? "Unknown Category",
            Year = car.CarDetail?.Year ?? 0,
            Description = car.CarDetail?.Description ?? "No description available",
            IsAutomatic = car.CarDetail?.IsAutomatic ?? false,
            FuelType = car.CarDetail?.FuelType ?? "Unknown Fuel Type",
            Power = car.CarDetail?.Power ?? 0,
            City = car.CarDetail?.City ?? "Unknown City",
            RegistrationNumber = car.RegistrationNumber,
            PricePerDay = car.PricePerDay,
            ImageUrl = car.CarDetail.ImageUrl,
            BookingSchedule = car.Bookings?
                .Select(booking =>
                {
                    return new BookingSchedule
                    {
                        FromDate = booking.FromDate,
                        ToDate = booking.ToDate
                    };
                }).ToList() ?? new List<BookingSchedule>()
        }).ToList();

        return View(viewModel);
    }
}
