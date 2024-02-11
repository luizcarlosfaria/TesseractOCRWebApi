using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesseractApi.Services;

public class TesseractService
{
    private readonly ILogger<TesseractService> logger;

    public TesseractService(ILogger<TesseractService> logger)
    {
        this.logger = logger;
    }


    public string GetVersion()
    {
        int exitCode; string output; string error;

        (exitCode, output, error) = this.ExecuteTesseractProcess("--version");

        return output;
    }

    public string GetTextOfImageFile(string inputFileName)
    {
        if (!File.Exists(inputFileName))
            throw new FileNotFoundException("Input file does not exists", inputFileName);

        //Tesseract adiciona sozinho o .txt no nome do arquivo de output.
        string outputFileNameWithoutExtension = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));

        using DisposableFile outputFileName = $"{outputFileNameWithoutExtension}.txt";

        string returnValue = null;

        this.ExecuteTesseractProcess($"\"{inputFileName}\" {outputFileNameWithoutExtension}");

        if (outputFileName.File.Exists)
        {
            returnValue = File.ReadAllText(outputFileName.File.FullName);
        }

        return returnValue;
    }

    private (int exitCode, string output, string error) ExecuteTesseractProcess(string args)
    {
        var tesseractCreateInfo = new ProcessStartInfo("tesseract", args)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        var tesseractProcess = Process.Start(tesseractCreateInfo);

        string output = tesseractProcess.StandardOutput.ReadToEnd();

        string error = tesseractProcess.StandardError.ReadToEnd();

        TimeSpan timeout = TimeSpan.FromSeconds(5);

        tesseractProcess.WaitForExit((int)timeout.TotalMicroseconds);

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(tesseractProcess.ExitCode == 0 ? "Success" : "Error");
        stringBuilder.AppendLine($"ExitCode: {tesseractProcess.ExitCode}");
        stringBuilder.AppendLine($"Executed Process: '{tesseractCreateInfo.FileName}'");
        stringBuilder.AppendLine($"Args: '{tesseractCreateInfo.Arguments}'");
        stringBuilder.AppendLine($"Timeout: '{timeout}'");
        stringBuilder.AppendLine($"StdOut: '{output}'");
        stringBuilder.AppendLine($"StdErr: '{error}'");

        if (tesseractProcess.ExitCode != 0)
        {
            logger.LogError(stringBuilder.ToString());
            throw new InvalidOperationException($"Error on execute {tesseractCreateInfo.FileName} with args '{tesseractCreateInfo.Arguments}', exit code {tesseractProcess.ExitCode}. Output: '{output}' Error: '{error}'");
        }
        logger.LogInformation(stringBuilder.ToString());

        return (tesseractProcess.ExitCode, output, error);
    }
}
