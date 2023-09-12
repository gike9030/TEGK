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
        public IActionResult Index(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.GetTempFileName();
                    filePaths.Add(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                }
            }
            _data.FileName = filePaths[0];
            _data.Text = new StreamReader(filePaths[0]).ReadToEnd();
            _data.FileSize = _data.Text.Length;

            return View(_data);

        }
    }
}
