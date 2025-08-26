using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using yogloansdotnet.Models;
using yogloansdotnet.Data;
using Microsoft.EntityFrameworkCore;


namespace yogloansdotnet.Controllers
{
    [Authorize]
    [Route("admin/[controller]")]
    public class GalleryadminController : Controller
    {
        private readonly ILogger<GalleryadminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GalleryadminController(ILogger<GalleryadminController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var galleries = await _context.Gallery.ToListAsync();
            return View("~/Views/admin/Galleryadmin/index.cshtml", galleries);
        }
          [HttpGet("Gallery")]
        public async Task<IActionResult> Gallery()
        {
            var galleries = await _context.GalleryImages.ToListAsync();
            return View("~/Views/admin/Galleryadmin/gallery.cshtml", galleries);
        }
        [Route("add-images-welcome")]
        [HttpPost]
        public async Task<IActionResult> addGallery([FromForm] string Title, [FromForm] string Description, [FromForm] string ImageTitle, IFormFile image, [FromForm] string ids)
        {
            try
            {
                // Count existing gallery entries
                int galleryCount = await _context.Gallery.CountAsync();
                if (string.IsNullOrEmpty(ids) && galleryCount >= 4)
                {
                    return Json(new
                    {
                        success = false,
                        message = "You cannot add more than 4 gallery entries. Edit or delete existing ones instead."
                    });
                }

                string filePath = await SavePdfAsync(image);

                if (string.IsNullOrEmpty(ids))
                {
                    // Create new gallery entry
                    var gallery = new Gallery
                    {
                        Title = Title,
                        Description = Description,
                        ImageTitle = ImageTitle,
                        FilePath = filePath
                    };

                    _context.Gallery.Add(gallery);
                    await _context.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        message = "Gallery entry added successfully",
                        id = gallery.Id,
                        title = gallery.Title,
                        description = gallery.Description,
                        filePath = gallery.FilePath
                    });
                }
                else
                {
                    // Update existing gallery entry
                    var existingGallery = await _context.Gallery.FindAsync(int.Parse(ids));
                    if (existingGallery == null)
                    {
                        return Json(new { success = false, message = "Gallery entry not found" });
                    }

                    // Delete old file if it exists
                    if (!string.IsNullOrEmpty(existingGallery.FilePath))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingGallery.FilePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    existingGallery.Title = Title;
                    existingGallery.Description = Description; // ✅ Added missing update
                    existingGallery.ImageTitle = ImageTitle;   // ✅ Added missing update
                    existingGallery.FilePath = filePath;

                    await _context.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        message = "Gallery entry updated successfully",
                        id = existingGallery.Id,
                        title = existingGallery.Title,
                        description = existingGallery.Description,
                        imageTitle = existingGallery.ImageTitle,
                        filePath = existingGallery.FilePath
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding/updating gallery entry");
                return Json(new { success = false, message = ex.Message });
            }
        }

        private async Task<string> SavePdfAsync(IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Gallery-welcome");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/Gallery-welcome/" + fileName;
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var gallery = await _context.Gallery.FindAsync(id);
                if (gallery == null)
                {
                    return Json(new { success = false, message = "Gallery entry not found" });
                }

                // Delete the associated file if it exists
                if (!string.IsNullOrEmpty(gallery.FilePath))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, gallery.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Gallery.Remove(gallery);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Gallery entry deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting gallery entry");
                return Json(new { success = false, message = ex.Message });
            }

        }
        /* add other all images */

        [Route("add-images")]
        [HttpPost]
        public async Task<IActionResult> add([FromForm] string ImageTitle, IFormFile image, [FromForm] string ids)
        {
            try
            {
                 string filePath = await SavePdfAsync(image);
                if (string.IsNullOrEmpty(ids))
                {
                    // Create new gallery entry
                    var gallery = new GalleryImagesModel
                    {
                        ImageTitle = ImageTitle,
                        FilePath = filePath
                    };

                    _context.GalleryImages.Add(gallery);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Gallery entry added successfully" });
                }
                else
                {
                    var existingGallery = await _context.GalleryImages.FindAsync(int.Parse(ids));
                    if (existingGallery == null)
                    {
                        return Json(new { success = false, message = "Gallery entry not found" });
                    }

                    // Update title
                    existingGallery.ImageTitle = ImageTitle;

                    // If new file uploaded, replace old one
                    if (image != null && image.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existingGallery.FilePath))
                        {
                            var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingGallery.FilePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                        existingGallery.FilePath = await SavePdfAsync(image);
                    }

                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Gallery entry updated successfully" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding/updating gallery entry");
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        [HttpPost("delete2/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete2(int id)
        {
            try
            {
                var gallery = await _context.GalleryImages.FindAsync(id);
                if (gallery == null)
                {
                    return Json(new { success = false, message = "Gallery entry not found" });
                }

                // Delete the associated file if it exists
                if (!string.IsNullOrEmpty(gallery.FilePath))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, gallery.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.GalleryImages.Remove(gallery);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Gallery entry deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting gallery entry");
                return Json(new { success = false, message = ex.Message });
            }

        }
    }
}
