namespace TesseractApi;

public class DisposableFile : IDisposable
{
    private bool disposedValue;
    private FileInfo file;

    public DisposableFile(FileInfo file)
    {
        this.file = file;
    }
    public FileInfo File => this.file;

    #region Idisposable Support

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (this.file.Exists)
                    this.file.Delete();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~DisposableFile()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion

    #region Operators

    public static implicit operator FileInfo(DisposableFile disposableFile)
    {
        return disposableFile?.file;
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
