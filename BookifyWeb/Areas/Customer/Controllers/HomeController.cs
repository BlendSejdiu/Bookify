using Bookify.Data.Repository.IRepository;
using Bookify.Models;
using Bookify.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BookifyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        #region Index
        public async Task<IActionResult> Index(string searchString)
        {
            IEnumerable<Book> bookList;

            if (!string.IsNullOrEmpty(searchString))
            {
                // filter books based on the title
                bookList = _unitOfWork.Book.GetAll(
                    filter: b =>
                        b.Title.ToLower().Contains(searchString.ToLower()) //||
                        //b.Author.FullName.ToLower().Contains(searchString.ToLower()) ||
                        //b.Category.Name.ToLower().Contains(searchString.ToLower())
                        ,
                includeProperties: "Category,Author"
                );

                if (!bookList.Any())
                {
                    bookList = _unitOfWork.Book.GetAll(includeProperties: "Category,Author");
                    TempData["warning"] = "No books were found!";
                }
            }
            else
            {
                // If no search string, get all books
                bookList = _unitOfWork.Book.GetAll(includeProperties: "Category,Author");
            }

            var user = await _userManager.GetUserAsync(User) as ApplicationUser;
            if (user != null)
            {
                var role = await _userManager.GetRolesAsync(user);
                if (role.FirstOrDefault() == SD.Role_Admin)
                {
                    //return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
                    return RedirectToAction("Index", "Order", new { area = "Admin" });
                }
            }

            // Pass the filtered or unfiltered book list to the view
            return View(bookList);
        }
        #endregion

        #region Details
        [Authorize(Policy = "NonAdminAccess")]
        public IActionResult Details(int bookId)
        {
            ShoppingCart cart = new()
            {
                Book = _unitOfWork.Book.Get(u => u.Id == bookId, includeProperties: "Category,Author"),
                Count = 1,
                BookId = bookId
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.BookId == shoppingCart.BookId);
            if (cartFromDb != null)
            {
                //shopping cart already exists
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            {
                //add cart record
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());

            }
            TempData["success"] = "Cart updated successfully";

            return RedirectToAction(nameof(Index));
        }
        #endregion

    }
}
