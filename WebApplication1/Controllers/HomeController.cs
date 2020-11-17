using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly CvManager _cvManager;

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _cvManager = new CvManager("cv/");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UploadPage()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadVM uploadVM)
        {
            // Check if the provided file is valid
            if (uploadVM.File == null)
                throw new NotImplementedException("No file was provided");

            if (!uploadVM.File.FileName.EndsWith(".pdf"))
                throw new NotImplementedException("The provided file was not a .pdf file");

            // Store the file
            Guid id = _cvManager.GenerateNewId();
            var filePath = _cvManager.GetFilePath(id);
            using (var fileSteam = new FileStream(Path.Combine(_hostingEnvironment.WebRootPath, filePath), FileMode.Create))
                await uploadVM.File.CopyToAsync(fileSteam);

            // TODO: Remove previous cv

            // Turn PDF into binary stream
            byte[] data = System.IO.File.ReadAllBytes(Path.Combine(_hostingEnvironment.WebRootPath, filePath));
            
            // Upload PDF to database
            MySQLDatabaseConnection conn = new MySQLDatabaseConnection(
                    "studmysql01.fhict.local",
                    "dbi434169",
                    "DRCXzm9rULNZ4Rq"
                );
            Pdf pdf = new Pdf(0, data);
            conn.AddPDF(pdf);
            conn.CloseConnection();

            return View(new PreviewVM { URL = $"\\{filePath}" });
        }
    }
}
