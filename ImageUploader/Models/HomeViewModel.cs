using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageUploader.Models
{
    public class HomeViewModel
    {
        public UploadImageModel Upload { get; set; }
        public IEnumerable<ImageModel> Images { get; set; }
    }
}
