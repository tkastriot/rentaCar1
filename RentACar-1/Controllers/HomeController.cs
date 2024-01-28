using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentACar_1.Data;
using RentACar_1.Models;
using RentACar_1.ViewModels;

namespace RentACar_1.Controllers
{
    public class HomeController : Controller
	{
		private readonly ApplicationDbContext _dbContext;

		public HomeController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public IActionResult Index()
		{
			var filters = new CarFilterViewModel();

			var viewModel = new HomeIndexViewModel
			{
				Filters = filters,
				BrandList = new SelectList(_dbContext.CarDetails.Select(cd => cd.Brand).Distinct()),
				CategoryList = new SelectList(_dbContext.CarDetails.Select(cd => cd.Category).Distinct()),
				CityList = new SelectList(_dbContext.CarDetails.Select(cd => cd.City).Distinct())
            };

			return View(viewModel);
		}

        [HttpPost]
        public IActionResult NewestCars()
        {
            // Retrieve the 5 newest cars based on the Year property
            var newestCars = _dbContext.Cars
                .Include(c => c.CarDetail)
                .OrderByDescending(c => c.CarDetail.Year)
                .Take(5)
                .ToList();

            // Create a list to store CarDetailViewModel instances
            var carViewModels = new List<CarDetailViewModel>();

            // Iterate over each car and create a CarDetailViewModel instance
            foreach (var car in newestCars)
            {
                var viewModel = new CarDetailViewModel
                {
                    Brand = car.CarDetail.Brand,
                    Category = car.CarDetail.Category,
                    City = car.CarDetail.City,
                    Year = car.CarDetail.Year,
                    IsAutomatic = car.CarDetail.IsAutomatic,
                    FuelType = car.CarDetail.FuelType,
                    Power = car.CarDetail.Power,
                    PricePerDay = car.PricePerDay,
                    CarId = car.CarID,
                };

                // Add the view model to the list
                carViewModels.Add(viewModel);
            }

            // Pass the list of view models to the view
            return View(carViewModels);
        }
 

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
