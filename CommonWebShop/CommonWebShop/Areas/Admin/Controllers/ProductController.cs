using CommonWebShop.DataAccess.Data;
using CommonWebShop.DataAccess.Repository;
using CommonWebShop.DataAccess.Repository.IRepository;
using CommonWebShop.Models;
using CommonWebShop.Models.ViewModels;
using CommonWebShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace CommonWebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var objectProductList = _unitOfWork.product.GetAll(includeProperties:"Category").ToList();

            return View(objectProductList);
        }

        //public IActionResult Upser()
        //{
        //    return View();
        //}
        public IActionResult Upsert(int? id) //Update and Insert
        {
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CategoryList"] = CategoryList;
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.category.GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });

            ProductViewModel productViewModel = new ProductViewModel()
            {
                CategoryList = CategoryList,
                Product = new Product()
            };            
            if(id == null ||  id == 0)
            {
                //create
                return View(productViewModel);
            }
            else
            {
                //update
                productViewModel.Product = _unitOfWork.product.Get(p => p.Id == id);
                return View(productViewModel);
            }

        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel productViewModel, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string folderPath = @"images\product";
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(wwwRootPath, folderPath);

                    DeleteUrlImage(productViewModel.Product, wwwRootPath);

                    using (var fileStream = new FileStream(Path.Combine(filePath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productViewModel.Product.ImageUrl = @"\" + folderPath + @"\" + fileName;
                }
                if(productViewModel.Product.Id == 0)
                {
                    _unitOfWork.product.Add(productViewModel.Product);
                    TempData["success"] = "Product created successfully";

                }
                else
                {
                    _unitOfWork.product.Update(productViewModel.Product);
                    TempData["success"] = "Product updated successfully";

                }
                _unitOfWork.Save();
                return RedirectToAction("Index", "Product");
            }
            else //when we don't want to use [ValidationNever]
            {
                productViewModel.CategoryList = _unitOfWork.category.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
                return View(productViewModel);
            }
        }


        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? product = _unitOfWork.product.Get(p => p.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{
        //    Product? product = _unitOfWork.product.Get(p => p.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.product.Remove(product);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Category deleted successfully";
        //    return RedirectToAction("Index");
        //}

        #region ApiCall
        [HttpGet]
        public IActionResult GetAll() 
        {
            var objectProductList = _unitOfWork.product.GetAll(includeProperties: "Category").ToList();

            return Json(new {data = objectProductList});

        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var objectProduct = _unitOfWork.product.Get(p => p.Id == id);
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (objectProduct == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            DeleteUrlImage(objectProduct, wwwRootPath);

            _unitOfWork.product.Remove(objectProduct); 
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleting succesful" });

        }

        #endregion

        #region AuxiliaryMethods
        public void DeleteUrlImage(Product product, string wwwRootPath)
        {
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var filePathOld = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(filePathOld))
                {
                    System.IO.File.Delete(filePathOld);
                }
            }
        }
        #endregion
    }
}
