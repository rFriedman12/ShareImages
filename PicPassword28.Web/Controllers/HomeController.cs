using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PicPassword28.Data;
using PicPassword28.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PicPassword28.Web.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment _webHostEnvironment;

        private string _connString = @"Data Source=.\sqlexpress;Initial Catalog=General; Integrated Security=true;";

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile imageFile, string password)
        {
            string fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", fileName);

            using FileStream stream = new FileStream(filePath, FileMode.CreateNew);
            imageFile.CopyTo(stream);

            var imagesRepo = new ImagesRepository(_connString);
            int id = imagesRepo.AddImage(new Image
            {
                FileName = fileName,
                FilePath = filePath,
                Password = password
            });

            UploadViewModel model = new()
            {
                ImageLink = $"home/view?id={id}",
                Password = password
            };
            return View(model);
        }

        public IActionResult View(int id)
        {
            ViewViewModel model = new()
            {
                Id = id
            };
            return View(model);
        }
    }
}
