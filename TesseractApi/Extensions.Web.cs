using Microsoft.AspNetCore.Mvc;
using TesseractApi.Services;

namespace TesseractApi;

public static partial class Extensions
{


    public static void MapTesseractEndpoints(this WebApplication app)
    {
        var tesseract = app.MapGroup("/tesseract").DisableAntiforgery();
        //Desabilitado Antiforgery intencionalmente. 
        //Use um API Gateway para proteger a API.

        tesseract.MapGet("/", async (TesseractService tesseractService) =>
        {
            return await tesseractService.GetVersionAsync();
        });

        tesseract.MapPost("/ocr-by-upload", async (IFormFile file, TesseractService tesseractService, [FromQuery] string lang = "eng") =>
        {
            using DisposableFile disposableFile = await file.SaveFileOnTempDirectory().ConfigureAwait(true);

            string returnValue = await tesseractService.GetTextOfImageFileAsync(disposableFile.File.FullName, lang);

            return returnValue;
        });

        tesseract.MapPost("/ocr-by-filepath", async ([FromForm]string fileName, TesseractService tesseractService, [FromQuery] string lang = "eng") =>
        {
            string returnValue = await tesseractService.GetTextOfImageFileAsync(fileName, lang);

            return returnValue;
        });
    }

}
