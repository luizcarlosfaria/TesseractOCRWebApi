using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using TesseractApi.Services;

namespace TesseractApi;

public static partial class Extensions
{
    public static async Task<DisposableFile> SaveFileOnTempDirectory(this IFormFile formFile)
    {
        ArgumentNullException.ThrowIfNull(formFile);

        //não pode usar o using aqui, pois o arquivo será deletado antes de ser lido
        DisposableFile disposableFile = formFile.GetTempFileName();

        await formFile.WriteOnFile(disposableFile).ConfigureAwait(false);

        return disposableFile;
    }

    private static async Task WriteOnFile(this IFormFile formFile, FileInfo fileInfo)
    {
        using FileStream stream = fileInfo.OpenWrite();

        await formFile.CopyToAsync(stream).ConfigureAwait(false);
    }

    private static string GetTempFileName(this IFormFile file)
    {
        return Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():D}-{file.FileName}");
    }

}
