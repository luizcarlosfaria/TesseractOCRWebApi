using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TesseractApi
{
    public static class Extensions
    {
        public static async Task SaveFileOnTempDirectoryAndRun(this IFormFile file, Action<string> action)
        {
            var filePath = await file.SaveFileOnTempDirectory();

            try
            {
                action(filePath);
            }
            finally
            {
                System.IO.File.Delete(filePath);
            }
        }


        public static async Task<string> SaveFileOnTempDirectory(this IFormFile file)
        {
            var filePath = System.IO.Path.Combine("/tmp/", $"{Guid.NewGuid():D}-{file.FileName}");

            using (var stream = System.IO.File.OpenWrite(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

    }
}
