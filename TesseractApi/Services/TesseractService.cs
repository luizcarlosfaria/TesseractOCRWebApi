using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TesseractApi.Services
{
    public class TesseractService
    {
        private readonly ILogger<TesseractService> logger;

        public TesseractService(ILogger<TesseractService> logger)
        {
            this.logger = logger;
        }


        public string DecodeFile(string inputFileName)
        {
            if (!File.Exists(inputFileName))
                throw new FileNotFoundException("Input file does not exists", inputFileName);

            var outputFileNameWithoutExtension = System.IO.Path.Combine("/tmp", Guid.NewGuid().ToString("D"));

            var outputFileName = $"{outputFileNameWithoutExtension}.txt";

            string returnValue = null;

            try
            {
                this.ExecuteTesseractProcess($"{inputFileName} {outputFileNameWithoutExtension}");

                if (System.IO.File.Exists(outputFileName))
                {
                    returnValue = System.IO.File.ReadAllText(outputFileName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw;
            }
            finally
            {
                System.IO.File.Delete(outputFileName);
            }

            return returnValue;
        }

        private int ExecuteTesseractProcess(string args)
        {
            var tesseractCreateInfo = new System.Diagnostics.ProcessStartInfo("tesseract", args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            var tesseractProcess = Process.Start(tesseractCreateInfo);

            string output = tesseractProcess.StandardOutput.ReadToEnd();
            string error = tesseractProcess.StandardError.ReadToEnd();

            tesseractProcess.WaitForExit(1000 * 30);

            logger.LogError($"Executed Process: {tesseractCreateInfo.FileName}");
            logger.LogError($"Args: {tesseractCreateInfo.Arguments}");
            logger.LogError($"Output: {output}");
            logger.LogError($"Error: {error}");

            if (tesseractProcess.ExitCode != 0)
            {
                throw new InvalidOperationException($"Error on execute {tesseractCreateInfo.FileName} with args '{tesseractCreateInfo.Arguments}', exit code {tesseractProcess.ExitCode}");
            }

            return tesseractProcess.ExitCode;
        }
    }
}
