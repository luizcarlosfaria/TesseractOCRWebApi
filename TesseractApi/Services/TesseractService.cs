using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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


    public async Task<string> GetVersionAsync()
    {
        int exitCode; string output; string error;

        (exitCode, output, error) = await this.ExecuteTesseractProcessAsync("--version");

        return output;
    }

    public async Task<string> GetTextOfImageFileAsync(string inputFileName)
    {
        CheckInputFile(inputFileName);

        //Tesseract adiciona sozinho o .txt no nome do arquivo de output.
        string outputFileNameWithoutExtension = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));

        using DisposableFile outputFile = $"{outputFileNameWithoutExtension}.txt";

        await this.ExecuteTesseractProcessAsync($"\"{inputFileName}\" {outputFileNameWithoutExtension}");

        string returnValue = outputFile.ReadAllText();

        return returnValue;
    }

    private static void CheckInputFile(string inputFileName)
    {
        if (string.IsNullOrWhiteSpace(inputFileName)) 
            throw new ArgumentException($"'{nameof(inputFileName)}' cannot be null or whitespace.", nameof(inputFileName));


        string[] permittedDirectories = new string[] { Path.GetTempPath(), "/data/" };

        FileInfo fileInfo = new FileInfo(inputFileName);
        if (!permittedDirectories.Any(permittedDirectory => $"{fileInfo.Directory.FullName}/".StartsWith(permittedDirectory, StringComparison.InvariantCultureIgnoreCase)))
            throw new UnauthorizedAccessException("Input file must be in a permitted directory");

        if (!File.Exists(inputFileName))
            throw new FileNotFoundException("Input file does not exists", inputFileName);

    }

    private async Task<(int exitCode, string output, string error)> ExecuteTesseractProcessAsync(string args)
    {
        var tesseractCreateInfo = new ProcessStartInfo("tesseract", args)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        var tesseractProcess = Process.Start(tesseractCreateInfo);

        string output = tesseractProcess.StandardOutput.ReadToEnd();

        string error = tesseractProcess.StandardError.ReadToEnd();

        await tesseractProcess.WaitForExitAsync();

        int exitCode = tesseractProcess.ExitCode;

        StringBuilder stringBuilder = new StringBuilder()
            .AppendLine(exitCode == 0 ? "Success" : "Error")
            .AppendLine(CultureInfo.InvariantCulture, $"ExitCode: {exitCode}")
            .AppendLine(CultureInfo.InvariantCulture, $"Executed Process: '{tesseractCreateInfo.FileName}'")
            .AppendLine(CultureInfo.InvariantCulture, $"Args: '{tesseractCreateInfo.Arguments}'")
            .AppendLine(CultureInfo.InvariantCulture, $"StdOut: '{output}'")
            .AppendLine(CultureInfo.InvariantCulture, $"StdErr: '{error}'");

        string logMessage = stringBuilder.ToString();

        if (exitCode != 0)
        {
            logger.LogError(logMessage);

            throw new InvalidOperationException($"Error on execute {tesseractCreateInfo.FileName} with args '{tesseractCreateInfo.Arguments}', exit code {tesseractProcess.ExitCode}. Output: '{output}' Error: '{error}'");
        }

        logger.LogInformation(logMessage);

        return (exitCode, output, error);
    }
}
