using CommonWebShop.DataAccess.Repository.IRepository;
using CommonWebShop.Models;
using CommonWebShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace CommonWebShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.product.GetAll(includeProperties: "Category");
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork.product.Get(p => p.Id == id, includeProperties: "Category"),
                Count = 1,
                ProductId = _unitOfWork.product.Get(p => p.Id == id, includeProperties: "Category").Id
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCart shoppingCartNew = new()
            {
                Count = shoppingCart.Count,
                ProductId = shoppingCart.ProductId,
                ApplicationUserId = userId
            };
            ShoppingCart shoppingCartFromDb = _unitOfWork.shoppingCart.Get(u => u.ApplicationUserId == shoppingCartNew.ApplicationUserId && u.ProductId == shoppingCartNew.ProductId);

            if(shoppingCartFromDb != null)
            {
                shoppingCartFromDb.Count += shoppingCartNew.Count;
                _unitOfWork.shoppingCart.Update(shoppingCartFromDb);

            }
            else
            {
                _unitOfWork.shoppingCart.Add(shoppingCartNew);
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.shoppingCart.Get(u => u.ApplicationUserId == shoppingCartNew.ApplicationUserId).Count);
            }
            _unitOfWork.Save();
            TempData["success"] = "Cart updated successfully";
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}