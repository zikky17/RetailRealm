using DataAccessLibrary.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.Models;
using ModelsLibrary.ViewModels;
using System.Diagnostics;
using UtilitiesLibrary;

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

        public IActionResult Details(int orderId)
        {
            OrderVM orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeaderRepository.GetOne(u => u.OrderId == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetailRepository.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };
            return View(orderVM);
        }


        #region API CALLS


        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orders = _unitOfWork.OrderHeaderRepository.GetAll(null, "ApplicationUser").ToList();

            switch (status)
            {
                case "pending":
                    orders = orders.Where(u => u.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orders = orders.Where(u => u.PaymentStatus == StaticDetails.StatusInProcess);
                    break;
                case "completed":
                    orders = orders.Where(u => u.PaymentStatus == StaticDetails.StatusShipped);
                    break;
                case "approved":
                    orders = orders.Where(u => u.PaymentStatus == StaticDetails.StatusApproved);
                    break;
                default:
                    break;
            }


            return Json(new { data = orders });
        }
        #endregion
    }
}
