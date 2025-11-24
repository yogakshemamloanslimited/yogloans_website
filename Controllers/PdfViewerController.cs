using Microsoft.AspNetCore.Mvc;

namespace yogloansdotnet.Controllers
{
    public class PdfViewerController : Controller
    {
        public IActionResult Index(string pdf)
        {
            // pdf could be a relative path like "/pdfs/sample.pdf"
            ViewBag.PdfFile = pdf;
            return View("~/Views/pdf_view/pdfviewer.cshtml");
        }

    }
}
