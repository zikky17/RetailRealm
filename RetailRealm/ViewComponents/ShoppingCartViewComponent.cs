using DataAccessLibrary.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UtilitiesLibrary;

namespace RetailRealm.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {

        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userIdentity = (ClaimsIdentity)User.Identity;
            var claim = userIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(StaticDetails.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                       _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == claim.Value).Count());

                }
                return View(HttpContext.Session.GetInt32(StaticDetails.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }


    }
}
