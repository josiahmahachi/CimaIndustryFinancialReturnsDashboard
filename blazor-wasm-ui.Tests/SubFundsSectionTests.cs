using Xunit;
using Moq;
using blazor_wasm_ui.Components;
using blazor_wasm_ui.Models;
using blazor_wasm_ui.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace blazor_wasm_ui.Tests.Components;

public class SubFundsSectionTests : ComponentTestBase<SubFundsSection>
{
    private readonly Mock<IConfigurationService> _configurationServiceMock;

    public SubFundsSectionTests()
    {
        _configurationServiceMock = new Mock<IConfigurationService>();
        _configurationServiceMock.Setup(x => x.MaxExtensionDaysPerFund).Returns(90);
        _configurationServiceMock.Setup(x => x.WaiverReasons).Returns(new Dictionary<WaiverReason, WaiverReasonInfo>
        {
            [WaiverReason.ExceptionalCircumstances] = new WaiverReasonInfo { Label = "Exceptional Circumstances", Description = "Test" },
            [WaiverReason.FundDissolvingMerger] = new WaiverReasonInfo { Label = "Fund dissolving by way of merger", Description = "Test" }
        });

        Component.ConfigurationService = _configurationServiceMock.Object;
    }

    [Fact]
    public void HasWaiverExtensions_ReturnsTrue_WhenSubFundHasWaiver()
    {
        // Arrange
        Component.SubFunds = new List<SubFund>
        {
            new SubFund { Id = "1", ExtensionType = "waiver" },
            new SubFund { Id = "2", ExtensionType = "1-month" }
        };

        // Act
        var result = Component.HasWaiverExtensions;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasWaiverExtensions_ReturnsFalse_WhenNoSubFundHasWaiver()
    {
        // Arrange
        Component.SubFunds = new List<SubFund>
        {
            new SubFund { Id = "1", ExtensionType = "1-month" },
            new SubFund { Id = "2", ExtensionType = "deferral" }
        };

        // Act
        var result = Component.HasWaiverExtensions;

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("1-month", "May 31, 2025")]
    [InlineData("2-month", "June 30, 2025")]
    [InlineData("3-month", "July 31, 2025")]
    [InlineData("waiver", "Waiver under review")]
    [InlineData("deferral", "September 30, 2025")]
    [InlineData("none", "Select extension type to view new date")]
    public void GetNewFilingDate_ReturnsCorrectDate(string extensionType, string expected)
    {
        // Act
        var result = Component.GetNewFilingDate(extensionType);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("1-month", 1)]
    [InlineData("2-month", 2)]
    [InlineData("3-month", 3)]
    [InlineData("waiver", 0)]
    [InlineData("deferral", 1)]
    [InlineData("none", 0)]
    public void GetExtensionsToUse_ReturnsCorrectCount(string extensionType, int expected)
    {
        // Act
        var result = Component.GetExtensionsToUse(extensionType);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetUpdatedExtensionsUsed_IncludesNewExtension()
    {
        // Arrange
        var subFund = new SubFund { ExtensionsUsed = 1, ExtensionType = "2-month" };

        // Act
        var result = Component.GetUpdatedExtensionsUsed(subFund);

        // Assert
        Assert.Equal(3, result); // 1 + 2
    }

    [Fact]
    public void GetUpdatedExtensionsUsed_ReturnsCurrent_WhenNoExtensionType()
    {
        // Arrange
        var subFund = new SubFund { ExtensionsUsed = 2, ExtensionType = null };

        // Act
        var result = Component.GetUpdatedExtensionsUsed(subFund);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public void ValidateSubFunds_ReturnsFalse_WhenNoSubFunds()
    {
        // Arrange
        Component.SubFunds = new List<SubFund>();

        // Act
        var result = Component.ValidateSubFunds();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateSubFunds_ReturnsFalse_WhenSubFundHasNoExtensionType()
    {
        // Arrange
        Component.SubFunds = new List<SubFund>
        {
            new SubFund { Id = "1", ExtensionType = null, MaxExtensions = 3, ExtensionsUsed = 0, UsedExtensionDays = 0 }
        };

        // Act
        var result = Component.ValidateSubFunds();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateSubFunds_ReturnsFalse_WhenSubFundHasNoneExtensionType()
    {
        // Arrange
        Component.SubFunds = new List<SubFund>
        {
            new SubFund { Id = "1", ExtensionType = "none", MaxExtensions = 3, ExtensionsUsed = 0, UsedExtensionDays = 0 }
        };

        // Act
        var result = Component.ValidateSubFunds();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateSubFunds_ReturnsFalse_WhenWaiverWithoutReason()
    {
        // Arrange
        Component.SubFunds = new List<SubFund>
        {
            new SubFund { Id = "1", ExtensionType = "waiver", WaiverReason = null, MaxExtensions = 3, ExtensionsUsed = 0, UsedExtensionDays = 0 }
        };

        // Act
        var result = Component.ValidateSubFunds();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateSubFunds_ReturnsFalse_WhenExceedsMaxExtensions()
    {
        // Arrange
        Component.SubFunds = new List<SubFund>
        {
            new SubFund { Id = "1", ExtensionType = "3-month", MaxExtensions = 3, ExtensionsUsed = 2, UsedExtensionDays = 0 }
        };

        // Act
        var result = Component.ValidateSubFunds();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateSubFunds_ReturnsFalse_WhenExceedsMaxDays()
    {
        // Arrange
        Component.SubFunds = new List<SubFund>
        {
            new SubFund { Id = "1", ExtensionType = "3-month", MaxExtensions = 3, ExtensionsUsed = 0, UsedExtensionDays = 30 }
        };

        // Act
        var result = Component.ValidateSubFunds();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateSubFunds_ReturnsTrue_WhenValidConfiguration()
    {
        // Arrange
        Component.SubFunds = new List<SubFund>
        {
            new SubFund { Id = "1", ExtensionType = "1-month", WaiverReason = null, MaxExtensions = 3, ExtensionsUsed = 0, UsedExtensionDays = 0 },
            new SubFund { Id = "2", ExtensionType = "waiver", WaiverReason = "exceptional-circumstances", MaxExtensions = 3, ExtensionsUsed = 0, UsedExtensionDays = 0 }
        };

        // Act
        var result = Component.ValidateSubFunds();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GetAvailableExtensionTypes_IncludesAll_WhenNoLimits()
    {
        // Arrange
        var subFund = new SubFund { MaxExtensions = 3, ExtensionsUsed = 0, UsedExtensionDays = 0 };

        // Act
        var result = Component.GetAvailableExtensionTypes(subFund);

        // Assert
        Assert.Contains("none", result);
        Assert.Contains("1-month", result);
        Assert.Contains("2-month", result);
        Assert.Contains("3-month", result);
        Assert.Contains("waiver", result);
        Assert.Contains("deferral", result);
    }

    [Fact]
    public void GetAvailableExtensionTypes_ExcludesUnavailable_WhenLimitsReached()
    {
        // Arrange
        var subFund = new SubFund { MaxExtensions = 3, ExtensionsUsed = 2, UsedExtensionDays = 60 };

        // Act
        var result = Component.GetAvailableExtensionTypes(subFund);

        // Assert
        Assert.Contains("none", result);
        Assert.Contains("1-month", result); // Should be available (uses 1 extension, 1 remaining)
        Assert.Contains("waiver", result);
        Assert.Contains("deferral", result);
        Assert.DoesNotContain("2-month", result); // Would exceed max extensions (uses 2, only 1 remaining)
        Assert.DoesNotContain("3-month", result); // Would exceed max extensions (uses 3, only 1 remaining)
    }

    [Theory]
    [InlineData("none", "No Extension")]
    [InlineData("1-month", "1 Month")]
    [InlineData("2-month", "2 Months")]
    [InlineData("3-month", "3 Months")]
    [InlineData("waiver", "Request Waiver")]
    [InlineData("deferral", "Deferral")]
    public void GetExtensionTypeLabel_ReturnsCorrectLabel(string extensionType, string expected)
    {
        // Act
        var result = Component.GetExtensionTypeLabel(extensionType);

        // Assert
        Assert.Equal(expected, result);
    }
}