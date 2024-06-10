using DataAccessLibrary.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.Models;
using ModelsLibrary.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using UtilitiesLibrary;

namespace RetailRealm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] 
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }

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
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeaderRepository.GetOne(u => u.OrderId == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetailRepository.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };
            return View(OrderVM);
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeaderRepository.GetOne(u => u.OrderId == OrderVM.OrderHeader.OrderId);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeaderRepository.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Order Details Updated Successfully.";

            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.OrderId });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeaderRepository.UpdateStatus(OrderVM.OrderHeader.OrderId, StaticDetails.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.OrderId });

        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult ShipOrder()
        {

            var orderHeader = _unitOfWork.OrderHeaderRepository.GetOne(u => u.OrderId == OrderVM.OrderHeader.OrderId);
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = StaticDetails.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if(orderHeader.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _unitOfWork.OrderHeaderRepository.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Order Shipped Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.OrderId });

        }


        #region API CALLS


        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orders;

            if(User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
            {
                orders = _unitOfWork.OrderHeaderRepository.GetAll(null, "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                orders = _unitOfWork.OrderHeaderRepository
                    .GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }

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
