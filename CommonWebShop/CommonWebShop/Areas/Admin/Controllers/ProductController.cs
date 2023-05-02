using CommonWebShop.DataAccess.Data;
using CommonWebShop.DataAccess.Repository.IRepository;
using CommonWebShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommonWebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitWork;

        public ProductController(IUnitOfWork unitWork)
        {
            _unitWork = unitWork;
        }

        public IActionResult Index()
        {
            var objectProductList = _unitWork.product.GetAll().ToList();
            return View(objectProductList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            return View();
        }

        public IActionResult Edit() 
        {
            return View();
        }
    }
}
