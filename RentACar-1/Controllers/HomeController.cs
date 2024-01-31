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
				CityList = new SelectList(_dbContext.CarDetails.Select(cd => cd.City).Distinct()),
				NewestCars = GetNewestCars(),
			};

			return View(viewModel);
		}

		private List<CarDetailViewModel> GetNewestCars()
		{
			var newestCars = _dbContext.Cars
				.Include(c => c.CarDetail)
				.OrderByDescending(c => c.CarDetail.Year)
				.Take(5)
				.Select(c => new CarDetailViewModel
				{
					Brand = c.CarDetail.Brand,
					Category = c.CarDetail.Category,
					City = c.CarDetail.City,
					Year = c.CarDetail.Year,
					IsAutomatic = c.CarDetail.IsAutomatic,
					FuelType = c.CarDetail.FuelType,
					Power = c.CarDetail.Power,
					PricePerDay = c.PricePerDay,
					CarId = c.CarID,
				})
				.ToList();

			return newestCars;
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
