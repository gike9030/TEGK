using FlashcardsApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers
{
    public class FileReaderController : Controller
    {
		private FileData _data = new FileData() { };
		public IActionResult Index()
		{
            return View(_data);
		}
		
        [HttpPost]
        public IActionResult Index(IFormFile files)
        {
            IFormFile file = files;

			if (file == null)
			{
				return View(_data);
			}

            long size = file.Length;
            var filePath = Path.GetTempFileName();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            _data.FileName = filePath;
            _data.Text = new StreamReader(filePath).ReadToEnd();
            _data.FileSize = (int) size;

            return View(_data);

        }
    }
}
