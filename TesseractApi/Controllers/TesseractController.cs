using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TesseractApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TesseractController : ControllerBase
    {
        private readonly ILogger<TesseractController> _logger;

        public TesseractController(ILogger<TesseractController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        public string Get() => "OK";

        [HttpPost("ocr")]
        public async Task<string> Post(IFormFile file)
        {
            var filePath = $"/tmp/{Guid.NewGuid().ToString("D")}-{file.FileName}";

            using (var stream = System.IO.File.OpenWrite(filePath))
            {
                await file.CopyToAsync(stream);
            }

            string returnValue = ExecuteTesseract(filePath);

            System.IO.File.Delete(filePath);

            return returnValue;
        }

        private string ExecuteTesseract(string inputFileName)
        {
            string result = null;
            var outputFileNameWithoutExtension = System.IO.Path.Combine("/tmp", Guid.NewGuid().ToString("D"));
            var outputFileName = $"{outputFileNameWithoutExtension}.txt";

            string output = null;
            string error = null;

            var tesseractCreateInfo = new System.Diagnostics.ProcessStartInfo("tesseract", $"{inputFileName} {outputFileNameWithoutExtension}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            try
            {
                var tesseractProcess = System.Diagnostics.Process.Start(tesseractCreateInfo);

                output = tesseractProcess.StandardOutput.ReadToEnd();
                error = tesseractProcess.StandardError.ReadToEnd();
                tesseractProcess.WaitForExit(1000 * 30);

                if (tesseractProcess.ExitCode != 0)
                {
                    throw new InvalidOperationException("Errror");
                }

                if (System.IO.File.Exists(outputFileName))
                {
                    result = System.IO.File.ReadAllText(outputFileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
            finally
            {
                System.IO.File.Delete(outputFileName);
            }

            return result;

        }

    }
}
