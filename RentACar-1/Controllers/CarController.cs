using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentACar_1.Data;
using RentACar_1.Models;
using RentACar_1.ViewModels;

namespace RentACar_1.Controllers
{
    public class CarController : Controller
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

		public IActionResult Index()
		{
			var filters = new CarFilterViewModel(); // Create an empty filter model initially
            var cars = GetFilteredCars(filters);

            var viewModel = new CarIndexViewModel
            {
                Filters = filters,
                Cars = cars.ToList(),
                BrandList = new SelectList(_dbContext.CarDetails.Select(cd => cd.Brand).Distinct()),
                CategoryList = new SelectList(_dbContext.CarDetails.Select(cd => cd.Category).Distinct()),
                CityList = new SelectList(_dbContext.CarDetails.Select(cd => cd.City).Distinct())
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult FilterCars(CarFilterViewModel filters)
        {
            var cars = GetFilteredCars(filters);

            var viewModel = new CarIndexViewModel
            {
                Filters = filters,
                Cars = cars.ToList(),
                BrandList = new SelectList(_dbContext.CarDetails.Select(cd => cd.Brand).Distinct()),
                CategoryList = new SelectList(_dbContext.CarDetails.Select(cd => cd.Category).Distinct()),
                CityList = new SelectList(_dbContext.CarDetails.Select(cd => cd.City).Distinct())
            };

            return View("Index", viewModel);
        }
        private IQueryable<Car> GetFilteredCars(CarFilterViewModel filters)
        {
            var cars = _dbContext.Cars
                .Include(c => c.Bookings)
                .Include(c => c.CarDetail)
                .Where(c =>
                    (filters.IsAutomatic == null || c.CarDetail.IsAutomatic == filters.IsAutomatic) &&
                    (string.IsNullOrEmpty(filters.Brand) || c.CarDetail.Brand == filters.Brand) &&
                    (string.IsNullOrEmpty(filters.Category) || c.CarDetail.Category == filters.Category) &&
                    (string.IsNullOrEmpty(filters.City) || c.CarDetail.City == filters.City) &&
                    (!filters.FromDate.HasValue || !filters.ToDate.HasValue ||
		            !c.Bookings.Any(b =>
			        filters.FromDate <= b.ToDate && filters.ToDate >= b.FromDate)));

            return cars;
        }
        public async Task<IActionResult> CarDetail(int carId, string? fromDate, string? toDate)
        {
            // Retrieve the car details from the database based on the carId
            var carData = _dbContext.Cars
                .Include(c => c.CarDetail)
                .FirstOrDefault(c => c.CarID == carId);

            if (carData == null)
            {
                // Handle the case where the car with the specified ID is not found
                return NotFound();
            }

            // Retrieve reviews for the specified car
            var reviews = await _dbContext.Reviews
	            .Where(r => r.CarID == carId)
	            .Include(r => r.Car) // Include the associated car
	            .ToListAsync();

			// Map the retrieved data to CarDetailViewModel and pass it to the view
			var viewModel = new CarDetailViewModel
            {
                Brand = carData.CarDetail.Brand,
                Category = carData.CarDetail.Category,
                City = carData.CarDetail.City,
                Year = carData.CarDetail.Year,
                IsAutomatic = carData.CarDetail.IsAutomatic,
                FuelType = carData.CarDetail.FuelType,
                Power = carData.CarDetail.Power,
                PricePerDay = carData.PricePerDay,
                CarId = carData.CarID,
                FromDate = fromDate,
                ToDate = toDate,
                ImageUrl= carData.CarDetail.ImageUrl,
                Reviews = reviews
            };

            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BookCar(int carId, DateTime fromDate, DateTime toDate)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Model State invalid";
                return RedirectToAction("CarDetail", "Car", new
                {
                    carId = carId,
                    fromDate = fromDate,
                    toDate = toDate
                }); // Redirect to the CarDetail action
            }


            // Find the selected car
            var car = await _dbContext.Cars.FindAsync(carId);

            if (car == null)
            {
                // Handle car not found
                return NotFound();
            }

            // Check if the car is available for the selected dates
            if (!IsCarAvailable(car, fromDate, toDate))
            {
                TempData["ErrorMessage"] = "Car is not available for the selected dates.";
                return RedirectToAction("CarDetail", "Car", new
                {
                    carId = carId,
                    fromDate = fromDate,
                    toDate = toDate
                }); // Redirect to the CarDetail action
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Get the current user's ID
            if (currentUser == null)
            {
                return Redirect("~/Identity/Account/Login");
            }

            // Create a new booking
            var booking = new Booking
            {
                CarID = carId,
                RenterID = currentUser.Id,
                FromDate = fromDate,
                ToDate = toDate
            };

            _dbContext.Bookings.Add(booking);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("BookingDetails", "Booking", new
            {
                bookingId = booking.BookingID,
            });
        }

        private bool IsCarAvailable(Car car, DateTime fromDate, DateTime toDate)
        {
            // Check if there are any overlapping bookings for the selected dates
            return !_dbContext.Bookings.Any(b => b.CarID == car.CarID &&
                                                ((fromDate >= b.FromDate && fromDate <= b.ToDate) ||
                                                (toDate >= b.FromDate && toDate <= b.ToDate)));
        }

        public IActionResult EditCar(int id)
        {
            var car = _dbContext.Cars
                .Include(c => c.CarDetail)
                .FirstOrDefault(c => c.CarID == id);

            if (car == null)
            {
                return NotFound();
            }

            var viewModel = new EditCarViewModel
            {
                CarId = car.CarID,
                Brand = car.CarDetail?.Brand ?? "",
                Category = car.CarDetail?.Category ?? "",
                Year = car.CarDetail?.Year ?? 0,
                Description = car.CarDetail?.Description ?? "",
                IsAutomatic = car.CarDetail?.IsAutomatic ?? false,
                FuelType = car.CarDetail?.FuelType ?? "",
                Power = car.CarDetail?.Power ?? 0,
                City = car.CarDetail?.City ?? "",
                RegistrationNumber = car.RegistrationNumber,
                PricePerDay = car.PricePerDay,
                ImageUrl = car.CarDetail?.ImageUrl ?? "",
            };

            return View(viewModel);
        }

        // POST: Car/EditCar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCar(int id, EditCarViewModel viewModel)
        {
            if (id != viewModel.CarId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var car = await _dbContext.Cars
                    .Include(c => c.CarDetail)
                    .FirstOrDefaultAsync(c => c.CarID == id);

                if (car == null)
                {
                    return NotFound();
                }

                car.CarDetail.Brand = viewModel.Brand;
                car.CarDetail.Category = viewModel.Category;
                car.CarDetail.Year = viewModel.Year;
                car.CarDetail.Description = viewModel.Description ?? "";
                car.CarDetail.IsAutomatic = viewModel.IsAutomatic;
                car.CarDetail.FuelType = viewModel.FuelType;
                car.CarDetail.Power = viewModel.Power;
                car.CarDetail.City = viewModel.City;
                car.RegistrationNumber = viewModel.RegistrationNumber;
                car.PricePerDay = viewModel.PricePerDay;
                car.CarDetail.ImageUrl = viewModel.ImageUrl;

                _dbContext.Update(car);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("MyCars", "Owner");
            }

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCar(int carId)
        {
            var car = await _dbContext.Cars
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(c => c.CarID == carId);

            if (car == null)
            {
                return NotFound();
            }

            // Check if the car has bookings for current or future dates
            var currentDate = DateTime.Now.Date;
            var isBooked = car.Bookings.Any(b => b.FromDate.Date <= currentDate && b.ToDate.Date >= currentDate || b.FromDate.Date > currentDate);

            if (isBooked)
            {
                TempData["ErrorMessage"] = "This car cannot be deleted as there are bookings for future dates.";
                return RedirectToAction("MyCars", "Owner"); // Redirect to the MyCars action
            }

            // Delete the car
            _dbContext.Cars.Remove(car);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("MyCars", "Owner");
        }

		[HttpPost]
		public async Task<IActionResult> AddReviewAsync(int carId, int rating, string comment)
		{
			if (carId == null)
			{
				return NotFound();
			}

			var currentUser = await _userManager.GetUserAsync(User);
            // Get the current user's ID
            if (currentUser == null)
            {
                return Redirect("~/Identity/Account/Login");
            }

            // Create a new review object
            var review = new Review
			{
				CarID = carId,
                RentalID = currentUser.Id,
				Rating = rating,
				Comment = comment
			};

			// Add the new review to the database
			await _dbContext.Reviews.AddAsync(review);
			await _dbContext.SaveChangesAsync();

			// Redirect back to the car details page
			return RedirectToAction("CarDetail", new { carId });
		}
	}
}
