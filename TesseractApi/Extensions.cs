using TesseractApi.Services;

namespace TesseractApi;

public static class Extensions
{
    public static async Task<DisposableFile> SaveFileOnTempDirectory(this IFormFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        DisposableFile disposableFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():D}-{file.FileName}");

        using (FileStream stream = disposableFile.File.OpenWrite())
        {
            await file.CopyToAsync(stream).ConfigureAwait(false);
        }

        return disposableFile;
    }

    public static void MapTesseractEndpoints(this WebApplication app)
    {
        var tesseract = app.MapGroup("/tesseract")
            .DisableAntiforgery(); //desabilitado intencionalmente. Use um API Gateway para proteger a API.

        tesseract.MapGet("/", (TesseractService tesseractService) =>
        {
            return tesseractService.GetVersion();
        });

        tesseract.MapPost("/ocr-by-upload", async (IFormFile file, TesseractService tesseractService) =>
        {
            using DisposableFile disposableFile = await file.SaveFileOnTempDirectory().ConfigureAwait(false);

            string returnValue = tesseractService.GetTextOfImageFile(disposableFile.File.FullName);

            return returnValue;
        });

        tesseract.MapPost("/ocr-by-filepath", (string fileName, TesseractService tesseractService) =>
        {
            string returnValue = tesseractService.GetTextOfImageFile(fileName);

            return returnValue;
        });
    }

}
