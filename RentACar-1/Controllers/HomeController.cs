using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
