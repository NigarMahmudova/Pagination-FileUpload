using Microsoft.AspNetCore.Mvc;
using PustokBookStore.Areas.Manage.ViewModels;
using PustokBookStore.DAL;
using PustokBookStore.Entities;
using PustokBookStore.Helpers;

namespace PustokBookStore.Areas.Manage.Controllers
{
    [Area("manage")]

    public class SliderController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(PustokDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int page = 1)
        {
            var query = _context.Sliders.AsQueryable();
            return View(PaginatedList<Slider>.Create(query,page,2));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Slider slider, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (slider.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "ImageFile is required");
                return View();
            }

            if(slider.ImageFile.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("ImageFile", "Max size of ImageFile is 2MB");
                return View();
            }

            if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
            {
                ModelState.AddModelError("ImageFile", "ImageFile must be .jpg, .jpeg or .png");
                return View();
            }



            slider.ImageName = FileManager.Save(slider.ImageFile, _env.WebRootPath, "/manage/uploads/sliders/");

            _context.Sliders.Add(slider);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
