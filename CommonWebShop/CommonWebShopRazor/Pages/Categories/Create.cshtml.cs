using CommonWebShopRazor.Data;
using CommonWebShopRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommonWebShopRazor.Pages.Categories
{
    public class CreateModel : PageModel
    {
        // [BindProperties]
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public Category category { get; set; }
        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {


        }

        public IActionResult OnPost() 
        {
            _db.Categories.Add(category);    
            _db.SaveChanges();
            return RedirectToPage("Index");
        }
    }
}
