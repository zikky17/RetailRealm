using DataAccessLibrary.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModelsLibrary.Models;
using ModelsLibrary.ViewModels;

namespace RetailRealm.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var allProducts = _unitOfWork.ProductRepository.GetAll("Category").ToList();

            return View(allProducts);
        }

        public IActionResult Upsert(int? id)
        {

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.CategoryRepository
               .GetAll().Select(x => new SelectListItem
               {
                   Text = x.Name,
                   Value = x.CategoryId.ToString()
               }),
                Product = new Product()
            };

            if (id == null || id == 0)
            {
                return View(productVM);
            }
            {
                productVM.Product = _unitOfWork.ProductRepository.GetOne(x => x.Id == id);
                return View(productVM);
            }

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string path = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(path, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(path, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.ProductRepository.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.ProductRepository.Update(productVM.Product);
                }

                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.CategoryRepository
               .GetAll().Select(x => new SelectListItem
               {
                   Text = x.Name,
                   Value = x.CategoryId.ToString()
               });
            }
            return View(productVM);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product product = _unitOfWork.ProductRepository.GetOne(u => u.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product obj = _unitOfWork.ProductRepository.GetOne(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.ProductRepository.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
