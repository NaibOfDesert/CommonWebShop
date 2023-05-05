using CommonWebShop.DataAccess.Data;
using CommonWebShop.DataAccess.Repository;
using CommonWebShop.DataAccess.Repository.IRepository;
using CommonWebShop.Models;
using CommonWebShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CommonWebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitWork)
        {
            _unitOfWork = unitWork;
        }

        public IActionResult Index()
        {
            var objectProductList = _unitOfWork.product.GetAll().ToList();

            return View(objectProductList);
        }

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
                _unitOfWork.product.Add(productViewModel.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
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

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? product = _unitOfWork.product.Get(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? product = _unitOfWork.product.Get(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            _unitOfWork.product.Remove(product);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
