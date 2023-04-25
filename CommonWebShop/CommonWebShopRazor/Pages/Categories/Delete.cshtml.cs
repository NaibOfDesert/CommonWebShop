using CommonWebShopRazor.Data;
using CommonWebShopRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommonWebShopRazor.Pages.Categories
{
    [BindProperties]

    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Category category { get; set; }
        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            if (id.HasValue && id != 0)
            {
                category = _db.Categories.FirstOrDefault(c => c.Id == id.Value);

            }
        }
        public IActionResult OnPost()
        {
            if(category == null)
            {
                return NotFound();
            }

            _db.Categories.Remove(category);
            _db.SaveChanges();
            // TempData["success"] = "Category edited successfully";
            return RedirectToPage("Index");
        }

    }
}
