using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ImageUploader.Entities;
using ImageUploader.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ImageUploader.Controllers
{
    public class HomeController : Controller
    {
        private const string PicturesContainerName = "pictures";
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ApplicationContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            IConfiguration configuration,
            ApplicationContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var storageAccountUrl = _configuration.GetValue<string>("StorageAccountUrl");

            var images = await _context.Images.Select(image => new ImageModel()
            {
                Title = image.Title,
                Description = image.Description,
                ImageUrl = $"{storageAccountUrl}{image.Filename}",
                Id = image.Id
            })
            .ToListAsync();

            return View(new HomeViewModel()
            {
                Images = images
            });
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(HomeViewModel uploadImageModel)
        {
            var id = Guid.NewGuid();
            var extension = uploadImageModel.Upload.File.FileName.Split('.').Last();
            var filename = $"{id}.{extension}";

            var image = new Image()
            {
                Id = id,
                Title = uploadImageModel.Upload.Title,
                Description = uploadImageModel.Upload.Description,
                Filename = filename
            };

            _context.Add(image);
            await _context.SaveChangesAsync();

            var blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("StorageAccount"));
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(PicturesContainerName);
     
            var blobClient = blobContainerClient.GetBlobClient(filename);

            await blobClient.UploadAsync(uploadImageModel.Upload.File.OpenReadStream(), new BlobHttpHeaders { ContentType = uploadImageModel.Upload.File.ContentType });

            return RedirectToAction("Index");
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
    }
}
