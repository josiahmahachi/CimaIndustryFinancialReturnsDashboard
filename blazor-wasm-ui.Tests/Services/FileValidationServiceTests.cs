using blazor_wasm_ui.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace blazor_wasm_ui.Tests.Services;

public class FileValidationServiceTests
{
    private readonly IFileValidationService _service;
    private readonly ILogger<FileValidationService> _logger;

    public FileValidationServiceTests()
    {
        _logger = new LoggerFactory().CreateLogger<FileValidationService>();
        _service = new FileValidationService(_logger);
    }

    #region File Size Tests

    [Fact]
    public void ValidateFile_WithValidSize_ReturnsSuccess()
    {
        // Arrange
        var mockFile = CreateMockFile("test.pdf", "application/pdf", 1024 * 1024); // 1 MB

        // Act
        var result = _service.ValidateFile(mockFile);

        // Assert
        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ValidateFile_ExceedsMaxSize_ReturnsFails()
    {
        // Arrange
        var mockFile = CreateMockFile("test.pdf", "application/pdf", 11 * 1024 * 1024); // 11 MB (exceeds 10 MB limit)

        // Act
        var result = _service.ValidateFile(mockFile);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("exceeds maximum allowed size", result.ErrorMessage);
    }

    [Fact]
    public void ValidateFile_ExactlyAtMaxSize_ReturnsSuccess()
    {
        // Arrange
        var mockFile = CreateMockFile("test.pdf", "application/pdf", 10 * 1024 * 1024); // Exactly 10 MB

        // Act
        var result = _service.ValidateFile(mockFile);

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion

    #region File Type Tests

    [Theory]
    [InlineData("test.pdf", "application/pdf")]
    [InlineData("document.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [InlineData("spreadsheet.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [InlineData("image.jpg", "image/jpeg")]
    [InlineData("image.png", "image/png")]
    [InlineData("notes.txt", "text/plain")]
    public void ValidateFile_WithAllowedTypes_ReturnsSuccess(string fileName, string contentType)
    {
        // Arrange
        var mockFile = CreateMockFile(fileName, contentType, 1024);

        // Act
        var result = _service.ValidateFile(mockFile);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("test.exe", "application/x-msdownload")]
    [InlineData("script.zip", "application/zip")]
    [InlineData("video.mp4", "video/mp4")]
    [InlineData("audio.mp3", "audio/mpeg")]
    public void ValidateFile_WithNotAllowedTypes_ReturnsFails(string fileName, string contentType)
    {
        // Arrange
        var mockFile = CreateMockFile(fileName, contentType, 1024);

        // Act
        var result = _service.ValidateFile(mockFile);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("not allowed", result.ErrorMessage);
    }

    #endregion

    #region Null File Tests

    [Fact]
    public void ValidateFile_WithNullFile_ReturnsFails()
    {
        // Act
        var result = _service.ValidateFile(null!);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("required", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Constants Tests

    [Fact]
    public void MaxFileSizeBytes_Returns10MB()
    {
        // Assert
        Assert.Equal(10 * 1024 * 1024, _service.MaxFileSizeBytes);
    }

    [Fact]
    public void AllowedMimeTypes_NotEmpty()
    {
        // Assert
        Assert.NotEmpty(_service.AllowedMimeTypes);
        Assert.Contains("application/pdf", _service.AllowedMimeTypes);
    }

    [Fact]
    public void GetAllowedFileTypesDescription_ReturnsDescription()
    {
        // Act
        var description = _service.GetAllowedFileTypesDescription();

        // Assert
        Assert.NotEmpty(description);
        Assert.Contains("PDF", description);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Creates a mock IBrowserFile for testing.
    /// </summary>
    private static IBrowserFile CreateMockFile(string name, string contentType, long size)
    {
        var mockFile = new Mock<IBrowserFile>();
        mockFile.Setup(f => f.Name).Returns(name);
        mockFile.Setup(f => f.ContentType).Returns(contentType);
        mockFile.Setup(f => f.Size).Returns(size);
        return mockFile.Object;
    }

    #endregion
}
