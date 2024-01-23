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

        public CarController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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
                        !c.Bookings.Any(b => (filters.FromDate >= b.ToDate || filters.ToDate <= b.FromDate))));

            return cars;
        }
        public IActionResult CarDetail(int carId)
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
            };

            return View(viewModel);
        }

    }
}
