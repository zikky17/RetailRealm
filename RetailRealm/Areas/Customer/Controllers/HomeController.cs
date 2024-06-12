using DataAccessLibrary.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.Models;
using RetailRealm.Models;
using System.Diagnostics;
using System.Security.Claims;
using UtilitiesLibrary;

namespace RetailRealm.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {        
            IEnumerable<Product> productList = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category,ProductImages");
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.ProductRepository.GetOne(u => u.Id == id, includeProperties: "Category,ProductImages"),
                Count = 1,
                ProductId = id
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var userIdentity = (ClaimsIdentity)User.Identity;
            var userId = userIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;

            var cartFromDb = _unitOfWork.ShoppingCartRepository.GetOne(u => u.ApplicationUserId == userId
            && u.ProductId == cart.ProductId);

            if (cartFromDb != null)
            {
                cartFromDb.Count += cart.Count;
                _unitOfWork.ShoppingCartRepository.Update(cartFromDb);
                _unitOfWork.Save();
               
            }
            else
            {
                _unitOfWork.ShoppingCartRepository.Add(cart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                    _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            TempData["success"] = "Cart updated successfully";
          
            return RedirectToAction("Index");
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
