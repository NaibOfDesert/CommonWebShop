using CommonWebShop.DataAccess.Data;
using CommonWebShop.DataAccess.Repository;
using CommonWebShop.DataAccess.Repository.IRepository;
using CommonWebShop.Models;
using CommonWebShop.Models.ViewModels;
using CommonWebShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;
using System.IO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CommonWebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitWork)
        {
            _unitOfWork = unitWork;
        }

        public IActionResult Index()
        {
            var objectCompanyList = _unitOfWork.company.GetAll().ToList();

            return View(objectCompanyList);
        }

        public IActionResult Upsert(int? id)
        {
         
            if(id == null ||  id == 0)
            {
                //create
                return View(new Company());
            }
            else
            {
                //update
                Company company = _unitOfWork.company.Get(p => p.Id == id);
                return View(company);
            }

        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if(company.Id == 0)
                {
                    _unitOfWork.company.Add(company);
                    TempData["success"] = "Product created successfully";

                }
                else
                {
                    _unitOfWork.company.Update(company);
                    TempData["success"] = "Product updated successfully";

                }
                _unitOfWork.Save();
                return RedirectToAction("Index", "Product");
            }
            else //when we don't want to use [ValidationNever]
            {
                return View(company);
            }
        }

        #region ApiCall
        [HttpGet]
        public IActionResult GetAll()
        {
            var objectCompanyList = _unitOfWork.company.GetAll().ToList();

            return Json(new { data = objectCompanyList });

        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var company = _unitOfWork.company.Get(p => p.Id == id);
            if (company == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.company.Remove(company);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleting succesful" });

        }

        #endregion
    }
}
