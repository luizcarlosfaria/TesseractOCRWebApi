using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TesseractApi.Services;

namespace TesseractApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TesseractController : ControllerBase
    {
        private readonly TesseractService tesseractService;

        public TesseractController(TesseractService tesseractService)
        {
            this.tesseractService = tesseractService;
        }

        [HttpGet()]
        public string Get() => "OK";

        [HttpPost("ocr-by-upload")]
        public async Task<string> OcrByUpload(IFormFile file)
        {
            string returnValue = null;

            await file.SaveFileOnTempDirectoryAndRun(filePath =>
            {
                returnValue = tesseractService.DecodeFile(filePath);
            });

            return returnValue;
        }

        [HttpPost("ocr-by-filepath")]
        public string OcrByFilePath([FromForm][System.ComponentModel.DataAnnotations.Required] string fileName)
        {
            string returnValue = tesseractService.DecodeFile(fileName);

            return returnValue;
        }


    }
}
