using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TesseractApi.Services;
using System.ComponentModel.DataAnnotations;

namespace TesseractApi.Controllers;

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
    public string Get() 
    {
        return this.tesseractService.GetVersion();
    }

    [HttpPost("ocr-by-upload")]
    public async Task<string> OcrByUpload(IFormFile file)
    {
        using DisposableFile disposableFile = await file.SaveFileOnTempDirectory().ConfigureAwait(false);

        string returnValue = tesseractService.GetTextOfImageFile(disposableFile.File.FullName);

        return returnValue;
    }

    [HttpPost("ocr-by-filepath")]
    public string OcrByFilePath([FromForm][Required] string fileName)
    {
        string returnValue = this.tesseractService.GetTextOfImageFile(fileName);

        return returnValue;
    }


}
