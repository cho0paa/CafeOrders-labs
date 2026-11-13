using System.Globalization;

namespace CafeOrders.App.Infrastructure;

/// <summary>Журнал аудиту: дописує рядок «час;операція;документ» у файл. Звільняє файл у Dispose.</summary>
internal sealed class FileAuditLog : IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed;

    /// <summary>Відкриває файл журналу на дописування.</summary>
    /// <param name="path">Шлях до файлу журналу.</param>
    /// <exception cref="IOException">Файл заблоковано іншим процесом або екземпляром.</exception>
    public FileAuditLog(string path)
    {
        _writer = new StreamWriter(path, append: true);
    }

    /// <summary>Дописує рядок аудиту.</summary>
    /// <param name="operation">Назва операції.</param>
    /// <param name="documentId">Номер документа.</param>
    public void Write(string operation, int documentId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _writer.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"{DateTimeOffset.Now:O};{operation};{documentId}"));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _writer.Flush();
        _writer.Dispose();
        _disposed = true;
    }
}
