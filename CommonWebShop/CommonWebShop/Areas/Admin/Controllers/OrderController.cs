using CommonWebShop.DataAccess.Repository.IRepository;
using CommonWebShop.Models;
using CommonWebShop.Models.ViewModels;
using CommonWebShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace CommonWebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderViewModel orderViewModel { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            orderViewModel = new()
            {
                OrderHeader = _unitOfWork.orderHeader.Get(o => o.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.orderDetail.GetAll(o => o.OrderHeaderId == orderId, includeProperties: "Product")
            };
            return View(orderViewModel);
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _unitOfWork.orderHeader.Get(o => o.Id == orderViewModel.OrderHeader.Id);
            orderHeaderFromDb.Name = orderViewModel.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = orderViewModel.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAdress = orderViewModel.OrderHeader.StreetAdress;
            orderHeaderFromDb.City = orderViewModel.OrderHeader.City;
            orderHeaderFromDb.State = orderViewModel.OrderHeader.State;
            orderHeaderFromDb.PostalCode = orderViewModel.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(orderViewModel.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = orderViewModel.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(orderViewModel.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = orderViewModel.OrderHeader.TrackingNumber;
            }
            _unitOfWork.orderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully."; 

            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.orderHeader.UpdateStatus(orderViewModel.OrderHeader.Id, StaticDetails.StatusProcessing);
            _unitOfWork.Save();

            TempData["Success"] = "Order Started Processing Successfully.";

            return RedirectToAction(nameof(Details), new { orderId = orderViewModel.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeaderFromDb = _unitOfWork.orderHeader.Get(o => o.Id == orderViewModel.OrderHeader.Id);
            orderHeaderFromDb.TrackingNumber = orderViewModel.OrderHeader.TrackingNumber;
            orderHeaderFromDb.Carrier = orderViewModel.OrderHeader.Carrier;
            orderHeaderFromDb.OrderStatus = StaticDetails.StatusShipped;
            orderHeaderFromDb.ShippingDate = DateTime.Now;
            if (orderHeaderFromDb.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
            {
                orderHeaderFromDb.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            _unitOfWork.orderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Order Shipped Successfully.";

            return RedirectToAction(nameof(Details), new { orderId = orderViewModel.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeaderFromDb = _unitOfWork.orderHeader.Get(o => o.Id == orderViewModel.OrderHeader.Id);
            if (orderHeaderFromDb.PaymentStatus == StaticDetails.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeaderFromDb.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.orderHeader.UpdateStatus(orderHeaderFromDb.Id, StaticDetails.StatusCancelled, StaticDetails.StatusRefunded);
            }
            else
            {
                _unitOfWork.orderHeader.UpdateStatus(orderHeaderFromDb.Id, StaticDetails.StatusCancelled, StaticDetails.StatusCancelled);

            }

            _unitOfWork.Save();

            TempData["Success"] = "Order Canceled Successfully.";

            return RedirectToAction(nameof(Details), new { orderId = orderViewModel.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult PayNowOrder()
        {
            orderViewModel.OrderHeader = _unitOfWork.orderHeader.Get(o => o.Id == orderViewModel.OrderHeader.Id, includeProperties: "ApplicationUser");
            orderViewModel.OrderDetails = _unitOfWork.orderDetail.GetAll(o => o.OrderHeaderId == orderViewModel.OrderHeader.Id, includeProperties: "Product");

            var domain = "https://localhost:7036/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={orderViewModel.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={orderViewModel.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment"
            };

            foreach (var c in orderViewModel.OrderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(c.Price * 100),
                        Currency = "PLN",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = c.Product.Title
                        }
                    },
                    Quantity = c.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.orderHeader.UpdateStripePaymentId(orderViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = _unitOfWork.orderHeader.Get(c => c.Id == orderHeaderId, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.orderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.orderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, StaticDetails.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }

            return View(orderHeaderId);
        }

        #region ApiCall
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objectOrderList;
            
            if(User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
            {
                objectOrderList = _unitOfWork.orderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objectOrderList = _unitOfWork.orderHeader.GetAll(o => o.ApplicationUserId == userId, includeProperties: "ApplicationUser").ToList();
            }

            switch (status)
            {
                case "inprocess":
                    objectOrderList = objectOrderList.Where(o => o.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment);
                    break;
                case "pending":
                    objectOrderList = objectOrderList.Where(o => o.OrderStatus == StaticDetails.StatusProcessing);
                    break;
                case "completed":
                    objectOrderList = objectOrderList.Where(o => o.OrderStatus == StaticDetails.StatusShipped);
                    break;
                case "approved":
                    objectOrderList = objectOrderList.Where(o => o.OrderStatus == StaticDetails.StatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data = objectOrderList });

        }

        #endregion
    }
}
