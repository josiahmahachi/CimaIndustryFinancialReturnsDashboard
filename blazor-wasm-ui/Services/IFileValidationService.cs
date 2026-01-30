using Microsoft.AspNetCore.Components.Forms;

namespace blazor_wasm_ui.Services;

/// <summary>
/// Service for file upload validation and processing.
/// </summary>
public interface IFileValidationService
{
    /// <summary>
    /// Maximum allowed file size in bytes (10 MB).
    /// </summary>
    long MaxFileSizeBytes { get; }

    /// <summary>
    /// Allowed file types (MIME types).
    /// </summary>
    IReadOnlyList<string> AllowedMimeTypes { get; }

    /// <summary>
    /// Validates a file before upload.
    /// </summary>
    FileValidationResult ValidateFile(IBrowserFile file);

    /// <summary>
    /// Gets a user-friendly list of allowed file types.
    /// </summary>
    string GetAllowedFileTypesDescription();
}

/// <summary>
/// Result of file validation.
/// </summary>
public class FileValidationResult
{
    public bool IsValid { get; set; } = true;
    public string? ErrorMessage { get; set; }

    public static FileValidationResult Success() => new() { IsValid = true };
    public static FileValidationResult Failure(string message) => new() { IsValid = false, ErrorMessage = message };
}

/// <summary>
/// Implementation of file validation service.
/// </summary>
public class FileValidationService : IFileValidationService
{
    private readonly ILogger<FileValidationService> _logger;

    public long MaxFileSizeBytes => 10 * 1024 * 1024; // 10 MB

    // Standard business document MIME types
    public IReadOnlyList<string> AllowedMimeTypes => new List<string>
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "image/jpeg",
        "image/png",
        "image/tiff",
        "text/plain"
    }.AsReadOnly();

    public FileValidationService(ILogger<FileValidationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public FileValidationResult ValidateFile(IBrowserFile file)
    {
        if (file == null)
        {
            return FileValidationResult.Failure("File is required.");
        }

        _logger.LogDebug("Validating file: {FileName}, Size: {Size} bytes, ContentType: {ContentType}", 
            file.Name, file.Size, file.ContentType);

        // Check file size
        if (file.Size > MaxFileSizeBytes)
        {
            var maxSizeMB = MaxFileSizeBytes / (1024 * 1024);
            var message = $"File size ({FormatBytes(file.Size)}) exceeds maximum allowed size of {maxSizeMB} MB.";
            _logger.LogWarning(message);
            return FileValidationResult.Failure(message);
        }

        // Check file type
        if (!AllowedMimeTypes.Contains(file.ContentType))
        {
            var message = $"File type '{file.ContentType}' is not allowed. Allowed types: {GetAllowedFileTypesDescription()}";
            _logger.LogWarning(message);
            return FileValidationResult.Failure(message);
        }

        _logger.LogDebug("File validation passed for: {FileName}", file.Name);
        return FileValidationResult.Success();
    }

    public string GetAllowedFileTypesDescription()
    {
        return "PDF, Word (DOC/DOCX), Excel (XLS/XLSX), Images (JPEG/PNG/TIFF), Text";
    }

    /// <summary>
    /// Formats bytes into human-readable format.
    /// </summary>
    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
