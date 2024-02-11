using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

}
