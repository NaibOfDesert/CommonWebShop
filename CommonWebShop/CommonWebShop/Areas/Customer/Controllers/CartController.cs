using CommonWebShop.DataAccess.Data;
using CommonWebShop.DataAccess.Repository;
using CommonWebShop.DataAccess.Repository.IRepository;
using CommonWebShop.Models;
using CommonWebShop.Models.ViewModels;
using CommonWebShop.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Claims;

namespace CommonWebShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]

    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        [BindProperty]
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

        public CartController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartViewModel = new()
            {
                ShoppingCartList = _unitOfWork.shoppingCart.GetAll(c => c.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new OrderHeader()

            };


            foreach(var c in ShoppingCartViewModel.ShoppingCartList)
            {
                c.Price = GetPriceBasedOnQuantity(c);
                ShoppingCartViewModel.OrderHeader.OrderTotal += c.Price * c.Count; 
            }

            return View(ShoppingCartViewModel);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartViewModel = new()
            {
                ShoppingCartList = _unitOfWork.shoppingCart.GetAll(c => c.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new OrderHeader()

            };
            
            ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.applicationUser.Get(u => u.Id == userId);
            ShoppingCartViewModel.OrderHeader.Name = ShoppingCartViewModel.OrderHeader.ApplicationUser.Name;
            ShoppingCartViewModel.OrderHeader.PhoneNumber = ShoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartViewModel.OrderHeader.StreetAdress = ShoppingCartViewModel.OrderHeader.ApplicationUser.StreetAdress;
            ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.ApplicationUser.City;
            ShoppingCartViewModel.OrderHeader.State = ShoppingCartViewModel.OrderHeader.ApplicationUser.State;
            ShoppingCartViewModel.OrderHeader.PostalCode = ShoppingCartViewModel.OrderHeader.ApplicationUser.PostalCode;

            foreach (var c in ShoppingCartViewModel.ShoppingCartList)
            {
                c.Price = GetPriceBasedOnQuantity(c);
                ShoppingCartViewModel.OrderHeader.OrderTotal += c.Price * c.Count;
            }
            return View(ShoppingCartViewModel);
        }

        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPost()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartViewModel.ShoppingCartList = _unitOfWork.shoppingCart.GetAll(c => c.ApplicationUserId == userId, includeProperties: "Product");
			ShoppingCartViewModel.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartViewModel.OrderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = _unitOfWork.applicationUser.Get(u => u.Id == userId);

			foreach (var c in ShoppingCartViewModel.ShoppingCartList)
			{
				c.Price = GetPriceBasedOnQuantity(c);
				ShoppingCartViewModel.OrderHeader.OrderTotal += c.Price * c.Count;
			}

            if(applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // it is a regular customer
                ShoppingCartViewModel.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
				ShoppingCartViewModel.OrderHeader.OrderStatus = StaticDetails.StatusPending;
			}
			else
            {
                // it is a comapny user
				ShoppingCartViewModel.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
				ShoppingCartViewModel.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
			}

            _unitOfWork.orderHeader.Add(ShoppingCartViewModel.OrderHeader);
            _unitOfWork.Save();

            foreach(var c in ShoppingCartViewModel.ShoppingCartList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = c.ProductId,
                    OrderHeaderId = ShoppingCartViewModel.OrderHeader.Id,
                    Price = c.Price,
                    Count = c.Count
                };
				_unitOfWork.orderDetail.Add(orderDetail);
				_unitOfWork.Save();
			}

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
                // it is a regular customer, need to capture payment
                // stripe logic
                var domain = "https://localhost:7036/";
				var options = new SessionCreateOptions
				{ 
					SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartViewModel.OrderHeader.Id}",
					CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment"
				};

                foreach(var c in ShoppingCartViewModel.ShoppingCartList)
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
                _unitOfWork.orderHeader.UpdateStripePaymentId(ShoppingCartViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
			}

			return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartViewModel.OrderHeader.Id});
		}

        public IActionResult OrderConfirmation(int id)
        {
			OrderHeader orderHeader = _unitOfWork.orderHeader.Get(c => c.Id == id, includeProperties: "ApplicationUser");
            if(orderHeader.PaymentStatus != StaticDetails.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if(session.PaymentStatus.ToLower() == "paid")
                {
					_unitOfWork.orderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.orderHeader.UpdateStatus(id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
			}

			List<ShoppingCart> shoppingCart = _unitOfWork.shoppingCart.GetAll(c => c.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

			_unitOfWork.shoppingCart.RemoveRange(shoppingCart);
			_unitOfWork.Save();

			return View(id);
        }

		public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.shoppingCart.Get(c => c.Id == cartId);
            cartFromDb.Count ++;
            _unitOfWork.shoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.shoppingCart.Get(c => c.Id == cartId);
            if (cartFromDb.Count <= 1)
            {
                _unitOfWork.shoppingCart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count--;
                _unitOfWork.shoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.shoppingCart.Get(c => c.Id == cartId);
            _unitOfWork.shoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if(shoppingCart.Count < 10)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                return shoppingCart.Product.Price10;
            }
        }


    }
}
