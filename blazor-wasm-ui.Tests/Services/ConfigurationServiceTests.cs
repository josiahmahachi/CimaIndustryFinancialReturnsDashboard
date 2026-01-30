using blazor_wasm_ui.Models;
using blazor_wasm_ui.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace blazor_wasm_ui.Tests.Services;

public class ConfigurationServiceTests
{
    private readonly IConfigurationService _service;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationServiceTests()
    {
        _logger = new LoggerFactory().CreateLogger<ConfigurationService>();
        _service = new ConfigurationService(_logger);
    }

    #region Fee Calculation Tests

    [Fact]
    public void CalculateExtensionFee_OneMonth_SingleFund_Returns625()
    {
        // Act
        var fee = _service.CalculateExtensionFee(ExtensionType.OneMonth, 1);

        // Assert
        Assert.Equal(625m, fee);
    }

    [Fact]
    public void CalculateExtensionFee_TwoMonth_SingleFund_Returns1250()
    {
        // Act
        var fee = _service.CalculateExtensionFee(ExtensionType.TwoMonth, 1);

        // Assert
        Assert.Equal(1250m, fee);
    }

    [Fact]
    public void CalculateExtensionFee_ThreeMonth_SingleFund_Returns1875()
    {
        // Act
        var fee = _service.CalculateExtensionFee(ExtensionType.ThreeMonth, 1);

        // Assert
        Assert.Equal(1875m, fee);
    }

    [Fact]
    public void CalculateExtensionFee_Waiver_SingleFund_Returns625()
    {
        // Act
        var fee = _service.CalculateExtensionFee(ExtensionType.Waiver, 1);

        // Assert
        Assert.Equal(625m, fee);
    }

    [Fact]
    public void CalculateExtensionFee_Deferral_SingleFund_Returns0()
    {
        // Act
        var fee = _service.CalculateExtensionFee(ExtensionType.Deferral, 1);

        // Assert
        Assert.Equal(0m, fee);
    }

    [Fact]
    public void CalculateExtensionFee_OneMonth_MultipleFunds_MultiplyByFundCount()
    {
        // Act
        var fee = _service.CalculateExtensionFee(ExtensionType.OneMonth, 3);

        // Assert - 625 * 3 = 1875
        Assert.Equal(1875m, fee);
    }

    [Fact]
    public void CalculateExtensionFee_TwoMonth_MultipleFunds_MultiplyByFundCount()
    {
        // Act
        var fee = _service.CalculateExtensionFee(ExtensionType.TwoMonth, 2);

        // Assert - 1250 * 2 = 2500
        Assert.Equal(2500m, fee);
    }

    [Fact]
    public void CalculateExtensionFee_Unlock_Returns0()
    {
        // Act
        var fee = _service.CalculateExtensionFee(ExtensionType.Unlock, 1);

        // Assert
        Assert.Equal(0m, fee);
    }

    #endregion

    #region Extension Validation Tests

    [Fact]
    public void ValidateExtensionRequest_WithinLimits_ReturnsSuccess()
    {
        // Arrange
        int extensionsUsed = 1;
        int usedDays = 30;
        var extensionType = ExtensionType.OneMonth;

        // Act
        var result = _service.ValidateExtensionRequest(extensionsUsed, usedDays, extensionType);

        // Assert
        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ValidateExtensionRequest_MaxExtensionsExceeded_ReturnsFail()
    {
        // Arrange
        int extensionsUsed = 3; // Already at max
        int usedDays = 90;
        var extensionType = ExtensionType.OneMonth;

        // Act
        var result = _service.ValidateExtensionRequest(extensionsUsed, usedDays, extensionType);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Maximum number of extensions", result.ErrorMessage);
    }

    [Fact]
    public void ValidateExtensionRequest_ExceedsMaxDays_ReturnsFail()
    {
        // Arrange
        int extensionsUsed = 2;
        int usedDays = 60; // Already used 60 days
        var extensionType = ExtensionType.ThreeMonth; // Would add 90 more days = 150 total (exceeds 90 max)

        // Act
        var result = _service.ValidateExtensionRequest(extensionsUsed, usedDays, extensionType);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("exceed the maximum of 90 days", result.ErrorMessage);
    }

    [Fact]
    public void ValidateExtensionRequest_WaiverDoesntCountDays_ReturnsSuccess()
    {
        // Arrange
        int extensionsUsed = 1;
        int usedDays = 90; // Already at max days
        var extensionType = ExtensionType.Waiver; // Waiver doesn't add days

        // Act
        var result = _service.ValidateExtensionRequest(extensionsUsed, usedDays, extensionType);

        // Assert - Should succeed because waiver adds 0 days
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateExtensionRequest_DeferralDoesntCountDays_ReturnsSuccess()
    {
        // Arrange
        int extensionsUsed = 1;
        int usedDays = 90; // Already at max days
        var extensionType = ExtensionType.Deferral; // Deferral adds 180 days but is separate concept

        // Act
        var result = _service.ValidateExtensionRequest(extensionsUsed, usedDays, extensionType);

        // Assert - Deferral adds 180 days, so this should fail
        Assert.False(result.IsValid);
    }

    #endregion

    #region Document Requirements Tests

    [Fact]
    public void GetDocumentRequirements_OneMonth_CoverLetterOnlyRequired()
    {
        // Act
        var requirements = _service.GetDocumentRequirements(ExtensionType.OneMonth);

        // Assert
        Assert.True(requirements.CoverLetterRequired);
        Assert.False(requirements.AuditorLetterRequired);
        Assert.False(requirements.OperatorAffidavitRequired);
        Assert.False(requirements.AdministratorLetterRequired);
        Assert.False(requirements.LiquidatorReportRequired);
    }

    [Fact]
    public void GetDocumentRequirements_TwoMonth_CoverAndAuditorRequired()
    {
        // Act
        var requirements = _service.GetDocumentRequirements(ExtensionType.TwoMonth);

        // Assert
        Assert.True(requirements.CoverLetterRequired);
        Assert.True(requirements.AuditorLetterRequired);
    }

    [Fact]
    public void GetDocumentRequirements_ThreeMonth_CoverAndAuditorRequired()
    {
        // Act
        var requirements = _service.GetDocumentRequirements(ExtensionType.ThreeMonth);

        // Assert
        Assert.True(requirements.CoverLetterRequired);
        Assert.True(requirements.AuditorLetterRequired);
    }

    [Fact]
    public void GetDocumentRequirements_WaiverWithExceptionalCircumstances_OperatorAffidavitRequired()
    {
        // Act
        var requirements = _service.GetDocumentRequirements(ExtensionType.Waiver, WaiverReason.ExceptionalCircumstances);

        // Assert
        Assert.True(requirements.CoverLetterRequired);
        Assert.True(requirements.OperatorAffidavitRequired);
        Assert.False(requirements.AuditorLetterRequired);
    }

    [Fact]
    public void GetDocumentRequirements_WaiverWithFundLaunchedDeregistered_AdministratorLetterRequired()
    {
        // Act
        var requirements = _service.GetDocumentRequirements(ExtensionType.Waiver, WaiverReason.FundLaunchedDeregistered);

        // Assert
        Assert.True(requirements.CoverLetterRequired);
        Assert.True(requirements.OperatorAffidavitRequired);
        Assert.True(requirements.AdministratorLetterRequired);
    }

    [Fact]
    public void GetDocumentRequirements_WaiverWithFundVoluntarilyLiquidated_LiquidatorReportRequired()
    {
        // Act
        var requirements = _service.GetDocumentRequirements(ExtensionType.Waiver, WaiverReason.FundVoluntarilyLiquidated);

        // Assert
        Assert.True(requirements.CoverLetterRequired);
        Assert.True(requirements.LiquidatorReportRequired);
        Assert.False(requirements.OperatorAffidavitRequired);
    }

    [Fact]
    public void DocumentRequirements_GetRequiredDocuments_ReturnsOnlyRequiredDocs()
    {
        // Arrange
        var requirements = _service.GetDocumentRequirements(ExtensionType.TwoMonth);

        // Act
        var required = requirements.GetRequiredDocuments();

        // Assert
        Assert.Contains("Cover Letter", required);
        Assert.Contains("Auditor Letter", required);
        Assert.Equal(2, required.Count);
    }

    #endregion

    #region Label Tests

    [Fact]
    public void GetExtensionTypeLabel_OneMonth_ReturnsCorrectLabel()
    {
        // Act
        var label = _service.GetExtensionTypeLabel(ExtensionType.OneMonth);

        // Assert
        Assert.Equal("1-Month Extension", label);
    }

    [Fact]
    public void GetExtensionTypeLabel_TwoMonth_ReturnsCorrectLabel()
    {
        // Act
        var label = _service.GetExtensionTypeLabel(ExtensionType.TwoMonth);

        // Assert
        Assert.Equal("2-Month Extension", label);
    }

    [Fact]
    public void GetWaiverReasonLabel_ExceptionalCircumstances_ReturnsCorrectLabel()
    {
        // Act
        var label = _service.GetWaiverReasonLabel(WaiverReason.ExceptionalCircumstances);

        // Assert
        Assert.Equal("Exceptional Circumstances", label);
    }

    #endregion

    #region Parse Tests

    [Theory]
    [InlineData("1-month", ExtensionType.OneMonth)]
    [InlineData("1-Month", ExtensionType.OneMonth)]
    [InlineData("onemonth", ExtensionType.OneMonth)]
    [InlineData("2-month", ExtensionType.TwoMonth)]
    [InlineData("3-month", ExtensionType.ThreeMonth)]
    [InlineData("waiver", ExtensionType.Waiver)]
    [InlineData("deferral", ExtensionType.Deferral)]
    [InlineData("unlock", ExtensionType.Unlock)]
    [InlineData("invalid", ExtensionType.None)]
    [InlineData(null, ExtensionType.None)]
    [InlineData("", ExtensionType.None)]
    public void ParseExtensionType_VariousFormats_ReturnCorrectType(string? input, ExtensionType expected)
    {
        // Act
        var result = _service.ParseExtensionType(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("exceptional-circumstances", WaiverReason.ExceptionalCircumstances)]
    [InlineData("fund-dissolving-merger", WaiverReason.FundDissolvingMerger)]
    [InlineData("fund-voluntarily-liquidated", WaiverReason.FundVoluntarilyLiquidated)]
    public void ParseWaiverReason_VariousFormats_ReturnCorrectReason(string input, WaiverReason expected)
    {
        // Act
        var result = _service.ParseWaiverReason(input);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Constants Tests

    [Fact]
    public void BaseExtensionFeePerMonth_Returns625()
    {
        // Assert
        Assert.Equal(625m, _service.BaseExtensionFeePerMonth);
    }

    [Fact]
    public void MaxExtensionsPerFund_Returns3()
    {
        // Assert
        Assert.Equal(3, _service.MaxExtensionsPerFund);
    }

    [Fact]
    public void MaxExtensionDaysPerFund_Returns90()
    {
        // Assert
        Assert.Equal(90, _service.MaxExtensionDaysPerFund);
    }

    [Fact]
    public void ExtensionDaysByType_AllTypesConfigured()
    {
        // Assert
        Assert.NotNull(_service.ExtensionDaysByType);
        Assert.Equal(30, _service.ExtensionDaysByType[ExtensionType.OneMonth]);
        Assert.Equal(60, _service.ExtensionDaysByType[ExtensionType.TwoMonth]);
        Assert.Equal(90, _service.ExtensionDaysByType[ExtensionType.ThreeMonth]);
        Assert.Equal(180, _service.ExtensionDaysByType[ExtensionType.Deferral]);
    }

    #endregion
}
