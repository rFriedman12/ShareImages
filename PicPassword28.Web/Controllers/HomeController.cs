using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
                ImageLink = $"https://localhost:44329/home/view?id={id}",
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

        public IActionResult ViewImage(int id, string password)
        {
            var imagesRepo = new ImagesRepository(_connString);
            Image image = imagesRepo.GetImage(id);
            bool canView = false;

            List<int> permitted = HttpContext.Session.Get<List<int>>("Permitted");
            if (permitted == null)
            {
                permitted = new List<int>();
            }

            if (permitted.Contains(id))
            {
                canView = true;
            }
            else
            {
                if (password == image.Password)
                {
                    canView = true;
                }
                permitted.Add(id);
                HttpContext.Session.Set("Permitted", permitted);
            }

            if (canView)
            {
                imagesRepo.IncreaseViews(id);
                ViewImageViewModel model = new()
                {
                    Image = new()
                    {
                        Id = image.Id,
                        FileName = image.FileName,
                        Password = image.Password,
                        Views = image.Views + 1
                    }
                };
                return View(model);
            }   
            return Redirect($"/home/view?id={id}");
        }
    }


    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
}
