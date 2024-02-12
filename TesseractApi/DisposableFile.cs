namespace TesseractApi;

public class DisposableFile(FileInfo file) : IDisposable
{
    private bool disposedValue;

    public FileInfo File { get; private set; } = file ?? throw new ArgumentNullException(nameof(file));

    public string ReadAllText()
    {
        string returnValue = null;

        if (this.File.Exists)
        {
            returnValue = System.IO.File.ReadAllText(this.File.FullName);
        }

        return returnValue;
    }

    public byte[] ReadAllBytes()
    {
        byte[] returnValue = null;

        if (this.File.Exists)
        {
            returnValue = System.IO.File.ReadAllBytes(this.File.FullName);
        }

        return returnValue;
    }

    #region Idisposable Support

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (this.File?.Exists ?? false)
                {
                    this.File.Delete();
                    this.File = null;
                }
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion

    #region Operators

    public static implicit operator FileInfo(DisposableFile disposableFile)
    {
        return disposableFile?.File;
    }

    public static implicit operator DisposableFile(FileInfo file)
    {
        return new DisposableFile(file);
    }

    public static implicit operator DisposableFile(string file)
    {
        return new DisposableFile(new FileInfo(file));
    }

    #endregion
}
