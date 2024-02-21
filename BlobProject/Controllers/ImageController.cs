using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BlobProject.Controllers
{
    public class ImageController : Controller
    {
        private readonly ImageService _imageService;
        public ImageController(ImageService imageService)
        {

            _imageService = imageService;

        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult ImageUpload(IFormFile file,string formData, string customerSNO, string filePath, string docIMG, string Cid)
        {
            if(file != null && file.Length > 0)
            {
                if (file.ContentType.StartsWith("image/"))
                {
                    var fName = Path.GetFileName(filePath);
                    var str = formData.Split(',');
                    var img = Convert.FromBase64String(str[1]);
                    Image image1;
                    using (var ms = new MemoryStream(img))
                    {
                        image1 = Image.FromStream(ms);
                    }
                    var newimage = ResizeImage(image1, new Size(200, 200));
                    string fileName = "Test_" + fName;
                    byte[] bytes = (byte[])(new ImageConverter()).ConvertTo(newimage, typeof(byte[]));
                    string imageurl = _imageService.UploadImageToBlob(bytes, fileName, "p");
                    return Json(new { imageurl });
                }
                else
                {
                    var fileName = Path.GetFileName(file.FileName);
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyToAsync(memoryStream);
                        var bytes = memoryStream.ToArray();
                        return Json(new { fileName });
                    }
                }
            }
            else
            {
                return Json(new { error = "No file uploaded" });
            }
        }
        public System.Drawing.Image ResizeImage(System.Drawing.Image imgToResize, Size size)
        {
            //Get the image current width
            int sourceWidth = imgToResize.Width;
            //Get the image current height
            int sourceHeight = imgToResize.Height;

            int destWidth = imgToResize.Width;
            //New Height
            int destHeight = imgToResize.Height;
            System.Drawing.Image img;

            Bitmap b = new Bitmap(destWidth, destHeight);

            b.SetResolution(1, 1);
            using (Graphics g = Graphics.FromImage((System.Drawing.Image)b))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                // Draw image with new width and height
                g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            }
            img = b;


            return img;
        }

    }
}
