using Bookify.Data.Repository.IRepository;
using Bookify.Models;
using Bookify.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookifyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AuthorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AuthorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Index
        public IActionResult Index()
        {
            List<Author> objAuthList = _unitOfWork.Author.GetAll().ToList();
            return View(objAuthList);
        }
        #endregion

        #region Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Author obj)
        {
            var existingAuthor = _unitOfWork.Author.Get(c => c.FullName.ToLower() == obj.FullName.ToLower());
            if (existingAuthor != null)
            {
                ModelState.AddModelError("FullName", "The Author Already Exists");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Author.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Author created successfully";
                return RedirectToAction("Index");
            }
            return View();

        }
        #endregion

        #region Edit
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Author? authFromDb = _unitOfWork.Author.Get(c => c.Id == id);
            if (authFromDb == null)
            {
                return NotFound();
            }
            return View(authFromDb);

        }

        [HttpPost]

        public IActionResult Edit(Author obj)
        {
            var existingAuthor = _unitOfWork.Author.Get(c => c.FullName.ToLower() == obj.FullName.ToLower());
            if (existingAuthor != null)
            {
                ModelState.AddModelError("FullName", "The Author Already Exists");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Author.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Author updated successfully";
                return RedirectToAction("Index");
            }
            return View();

        }
        #endregion

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Author> objAuthList = _unitOfWork.Author.GetAll().ToList();
            return Json(new { data = objAuthList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var authorToBeDeleted = _unitOfWork.Author.Get(c => c.Id == id);
            if (authorToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Author.Remove(authorToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted Successfully" });

        }

        #endregion

    }
}