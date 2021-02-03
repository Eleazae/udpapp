using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageUploader.Models
{
    public class UploadImageModel
    {
        public string Title { set; get; }
        public string Description { set; get; }
        public IFormFile File { set; get; }
    }
}
