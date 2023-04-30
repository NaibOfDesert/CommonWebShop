using CommonWebShopRazor.Data;
using CommonWebShopRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CommonWebShopRazor.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Category category { get; set; }
        public EditModel(ApplicationDbContext db)
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
            if (ModelState.IsValid)
            {
                _db.Categories.Update(category);
                _db.SaveChanges();
                TempData["success"] = "Category edited successfully";
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
