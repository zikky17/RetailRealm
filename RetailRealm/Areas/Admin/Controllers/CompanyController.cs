using DataAccessLibrary.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModelsLibrary.Models;
using ModelsLibrary.ViewModels;
using System.IO;
using UtilitiesLibrary;

namespace RetailRealm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var allCompanys = _unitOfWork.CompanyRepository.GetAll().ToList();

            return View(allCompanys);
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new Company());
            }
            {
                Company companyToUpdate = _unitOfWork.CompanyRepository.GetOne(x => x.Id == id);
                return View(companyToUpdate);
            }

        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {

            if (ModelState.IsValid)
            {
              
                if (company.Id == 0)
                {
                    _unitOfWork.CompanyRepository.Add(company);
                    TempData["success"] = "Company created successfully";   
                }
                else
                {
                    _unitOfWork.CompanyRepository.Update(company);
                    TempData["success"] = "Company updated successfully";
                }

                _unitOfWork.Save();
                return RedirectToAction("Index", "Company");
            }
            return View(company);
        }

        #region API CALLS


        [HttpGet]
        public IActionResult GetAll()
        {
            var allCompanys = _unitOfWork.CompanyRepository.GetAll().ToList();
            return Json( new { data = allCompanys });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToDelete = _unitOfWork.CompanyRepository.GetOne(u => u.Id == id);
          
            _unitOfWork.CompanyRepository.Remove(CompanyToDelete);
            _unitOfWork.Save();

           
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
