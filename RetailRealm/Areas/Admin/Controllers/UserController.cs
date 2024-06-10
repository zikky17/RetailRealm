using DataAccessLibrary.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Models;
using ModelsLibrary.ViewModels;
using RetailRealm.DataAccessLibrary.Data;
using System.IO;
using UtilitiesLibrary;

namespace RetailRealm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        public UserController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS


        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _db.ApplicationUsers.Include(u => u.Company).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach(var user in userList)
            {

                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;


                if(user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }

            return Json( new { data = userList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {  
           
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
