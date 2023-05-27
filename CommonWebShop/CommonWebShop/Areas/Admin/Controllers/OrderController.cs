using CommonWebShop.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CommonWebShop.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork; 

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region ApiCall
        [HttpGet]
        public IActionResult GetAll()
        {
            var objectOrderList = _unitOfWork.orderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            return Json(new { data = objectOrderList });

        }

        #endregion
    }
}
