using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
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
                File.Delete(filePath);
            }
        }


        public static async Task<string> SaveFileOnTempDirectory(this IFormFile file)
        {
            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():D}-{file.FileName}");

            using (var stream = File.OpenWrite(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

    }
}
