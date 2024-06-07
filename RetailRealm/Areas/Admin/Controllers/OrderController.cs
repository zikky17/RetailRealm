using DataAccessLibrary.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace RetailRealm.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }


        #region API CALLS


        [HttpGet]
        public IActionResult GetAll()
        {
            var orders = _unitOfWork.OrderHeaderRepository.GetAll(null, "ApplicationUser").ToList();
            return Json(new { data = orders });
        }
        #endregion
    }
}
