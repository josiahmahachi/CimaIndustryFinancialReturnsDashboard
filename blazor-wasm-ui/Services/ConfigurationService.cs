using blazor_wasm_ui.Models;

namespace blazor_wasm_ui.Services;

/// <summary>
/// Implementation of the configuration service.
/// Centralizes all business rules, fees, and validation constants.
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly ILogger<ConfigurationService> _logger;

    public decimal BaseExtensionFeePerMonth => 625m;

    public int MaxExtensionsPerFund => 3;

    public int MaxExtensionDaysPerFund => 90;

    public Dictionary<ExtensionType, int> ExtensionDaysByType => new()
    {
        { ExtensionType.None, 0 },
        { ExtensionType.OneMonth, 30 },
        { ExtensionType.TwoMonth, 60 },
        { ExtensionType.ThreeMonth, 90 },
        { ExtensionType.Waiver, 0 }, // Waiver doesn't add extension days
        { ExtensionType.Deferral, 180 }, // Deferral provides 6 months
        { ExtensionType.Unlock, 0 } // Unlock doesn't add days
    };

    private readonly Dictionary<ExtensionType, ExtensionTypeInfo> _extensionTypes = new()
    {
        {
            ExtensionType.OneMonth, new ExtensionTypeInfo
            {
                Label = "1-Month Extension",
                DaysAdded = 30,
                FeeMultiplier = 1m,
                RequiresWaiverReason = false,
                Description = "Extends the filing deadline by one month. Requires auditor letter only if selected with 2-month or 3-month for other sub-funds."
            }
        },
        {
            ExtensionType.TwoMonth, new ExtensionTypeInfo
            {
                Label = "2-Month Extension",
                DaysAdded = 60,
                FeeMultiplier = 2m,
                RequiresWaiverReason = false,
                Description = "Extends the filing deadline by two months. Requires auditor letter."
            }
        },
        {
            ExtensionType.ThreeMonth, new ExtensionTypeInfo
            {
                Label = "3-Month Extension",
                DaysAdded = 90,
                FeeMultiplier = 3m,
                RequiresWaiverReason = false,
                Description = "Extends the filing deadline by three months. Requires auditor letter. Note: Maximum 3 extensions per fund with max 90 total days."
            }
        },
        {
            ExtensionType.Waiver, new ExtensionTypeInfo
            {
                Label = "Waiver",
                DaysAdded = 0,
                FeeMultiplier = 1m,
                RequiresWaiverReason = true,
                Description = "Request a waiver of the filing deadline under exceptional circumstances. Requires waiver reason and supporting documentation."
            }
        },
        {
            ExtensionType.Deferral, new ExtensionTypeInfo
            {
                Label = "Deferral",
                DaysAdded = 180,
                FeeMultiplier = 0m, // Deferral has no fee
                RequiresWaiverReason = false,
                Description = "Requests a deferral of the filing deadline for up to six months. No fee required."
            }
        },
        {
            ExtensionType.Unlock, new ExtensionTypeInfo
            {
                Label = "Unlock Row",
                DaysAdded = 0,
                FeeMultiplier = 0m,
                RequiresWaiverReason = false,
                Description = "Unlocks a previously locked filing row for editing."
            }
        }
    };

    private readonly Dictionary<WaiverReason, WaiverReasonInfo> _waiverReasons = new()
    {
        {
            WaiverReason.ExceptionalCircumstances, new WaiverReasonInfo
            {
                Label = "Exceptional Circumstances",
                Description = "Request waiver due to exceptional or unforeseen circumstances."
            }
        },
        {
            WaiverReason.FundDissolvingMerger, new WaiverReasonInfo
            {
                Label = "Fund Dissolving/Merger",
                Description = "Fund is undergoing dissolution or merger proceedings."
            }
        },
        {
            WaiverReason.FundNotLaunchedDeregistered, new WaiverReasonInfo
            {
                Label = "Fund Not Launched/Deregistered",
                Description = "Fund has not yet launched or has been deregistered."
            }
        },
        {
            WaiverReason.FundLaunchedDeregistered, new WaiverReasonInfo
            {
                Label = "Fund Launched/Deregistered",
                Description = "Fund was launched and subsequently deregistered."
            }
        },
        {
            WaiverReason.FundInsufficientCapital, new WaiverReasonInfo
            {
                Label = "Fund Insufficient Capital",
                Description = "Fund does not have sufficient capital to meet filing requirements."
            }
        },
        {
            WaiverReason.FundTransferringJurisdiction, new WaiverReasonInfo
            {
                Label = "Fund Transferring Jurisdiction",
                Description = "Fund is transferring to another jurisdiction."
            }
        },
        {
            WaiverReason.FundUnableAuditedAccounts, new WaiverReasonInfo
            {
                Label = "Fund Unable to Provide Audited Accounts",
                Description = "Fund is unable to provide audited accounts."
            }
        },
        {
            WaiverReason.FundVoluntarilyLiquidated, new WaiverReasonInfo
            {
                Label = "Fund Voluntarily Liquidated",
                Description = "Fund has been voluntarily liquidated."
            }
        }
    };

    public IReadOnlyDictionary<ExtensionType, ExtensionTypeInfo> ExtensionTypes => _extensionTypes.AsReadOnly();
    public IReadOnlyDictionary<WaiverReason, WaiverReasonInfo> WaiverReasons => _waiverReasons.AsReadOnly();

    public ConfigurationService(ILogger<ConfigurationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public DocumentRequirements GetDocumentRequirements(ExtensionType extensionType, WaiverReason? waiverReason = null)
    {
        _logger.LogDebug("Getting document requirements for extension type: {ExtensionType}, waiver reason: {WaiverReason}", 
            extensionType, waiverReason);

        return extensionType switch
        {
            ExtensionType.OneMonth => new DocumentRequirements
            {
                CoverLetterRequired = true,
                AuditorLetterRequired = false, // Only required for 2-month or 3-month
                OperatorAffidavitRequired = false,
                AdministratorLetterRequired = false,
                LiquidatorReportRequired = false,
                OtherDocumentsAllowed = true
            },
            ExtensionType.TwoMonth => new DocumentRequirements
            {
                CoverLetterRequired = true,
                AuditorLetterRequired = true, // Required for 2-month extension
                OperatorAffidavitRequired = false,
                AdministratorLetterRequired = false,
                LiquidatorReportRequired = false,
                OtherDocumentsAllowed = true
            },
            ExtensionType.ThreeMonth => new DocumentRequirements
            {
                CoverLetterRequired = true,
                AuditorLetterRequired = true, // Required for 3-month extension
                OperatorAffidavitRequired = false,
                AdministratorLetterRequired = false,
                LiquidatorReportRequired = false,
                OtherDocumentsAllowed = true
            },
            ExtensionType.Waiver => GetWaiverDocumentRequirements(waiverReason),
            ExtensionType.Deferral => new DocumentRequirements
            {
                CoverLetterRequired = true,
                AuditorLetterRequired = false,
                OperatorAffidavitRequired = false,
                AdministratorLetterRequired = false,
                LiquidatorReportRequired = false,
                OtherDocumentsAllowed = true
            },
            ExtensionType.Unlock => new DocumentRequirements
            {
                CoverLetterRequired = false,
                AuditorLetterRequired = false,
                OperatorAffidavitRequired = false,
                AdministratorLetterRequired = false,
                LiquidatorReportRequired = false,
                OtherDocumentsAllowed = false
            },
            _ => new DocumentRequirements()
        };
    }

    private DocumentRequirements GetWaiverDocumentRequirements(WaiverReason? waiverReason)
    {
        return waiverReason switch
        {
            WaiverReason.ExceptionalCircumstances => new DocumentRequirements
            {
                CoverLetterRequired = true,
                AuditorLetterRequired = false,
                OperatorAffidavitRequired = true,
                AdministratorLetterRequired = false,
                LiquidatorReportRequired = false,
                OtherDocumentsAllowed = true
            },
            WaiverReason.FundDissolvingMerger => new DocumentRequirements
            {
                CoverLetterRequired = true,
                AuditorLetterRequired = false,
                OperatorAffidavitRequired = true,
                AdministratorLetterRequired = false,
                LiquidatorReportRequired = false,
                OtherDocumentsAllowed = true
            },
            WaiverReason.FundNotLaunchedDeregistered => new DocumentRequirements
            {
                CoverLetterRequired = true,
                AuditorLetterRequired = false,
                OperatorAffidavitRequired = true,
                AdministratorLetterRequired = false,
                LiquidatorReportRequired = false,
                OtherDocumentsAllowed = true
            },
            WaiverReason.FundLaunchedDeregistered => new DocumentRequirements
            {
                CoverLetterRequired = true,
                AuditorLetterRequired = false,
                OperatorAffidavitRequired = true,
                AdministratorLetterRequired = true, // Required for this reason
                LiquidatorReportRequired = false,
                OtherDocumentsAllowed = true
            },
            WaiverReason.FundInsufficientCapital => new DocumentRequirements
            {
                CoverLetterRequired = true,
                AuditorLetterRequired = false,
                OperatorAffidavitRequired = true,
                AdministratorLetterRequired = false,
                LiquidatorReportRequired = false,
                OtherDocumentsAllowed = true
            },
            WaiverReason.FundTransferringJurisdiction => new DocumentRequirements
            {
                CoverLetterRequired = true,
                AuditorLetterRequired = false,
                OperatorAffidavitRequired = true,
                AdministratorLetterRequired = false,
                LiquidatorReportRequired = false,
                OtherDocumentsAllowed = true
            },
            WaiverReason.FundUnableAuditedAccounts => new DocumentRequirements
            {
                CoverLetterRequired = true,
                AuditorLetterRequired = false,
                OperatorAffidavitRequired = false,
                AdministratorLetterRequired = false,
                LiquidatorReportRequired = true, // Required for this reason
                OtherDocumentsAllowed = true
            },
            WaiverReason.FundVoluntarilyLiquidated => new DocumentRequirements
            {
                CoverLetterRequired = true,
                AuditorLetterRequired = false,
                OperatorAffidavitRequired = false,
                AdministratorLetterRequired = false,
                LiquidatorReportRequired = true, // Required for this reason
                OtherDocumentsAllowed = true
            },
            _ => new DocumentRequirements()
        };
    }

    public decimal CalculateExtensionFee(ExtensionType extensionType, int subFundCount = 1)
    {
        _logger.LogDebug("Calculating extension fee for type: {ExtensionType}, sub-funds: {SubFundCount}", 
            extensionType, subFundCount);

        if (!_extensionTypes.TryGetValue(extensionType, out var typeInfo))
        {
            _logger.LogWarning("Unknown extension type: {ExtensionType}", extensionType);
            return 0m;
        }

        var feePerFund = BaseExtensionFeePerMonth * typeInfo.FeeMultiplier;
        var totalFee = feePerFund * subFundCount;

        _logger.LogDebug("Calculated fee: {TotalFee} (base: {FeePerFund}, count: {SubFundCount})", 
            totalFee, feePerFund, subFundCount);

        return totalFee;
    }

    public ValidationResult ValidateExtensionRequest(int extensionsUsed, int usedExtensionDays, ExtensionType requestedExtensionType)
    {
        _logger.LogDebug("Validating extension request: used={ExtensionsUsed}, days={UsedExtensionDays}, type={ExtensionType}", 
            extensionsUsed, usedExtensionDays, requestedExtensionType);

        // Check if maximum extensions already used
        if (extensionsUsed >= MaxExtensionsPerFund)
        {
            var message = $"Maximum number of extensions ({MaxExtensionsPerFund}) already used.";
            _logger.LogWarning(message);
            return ValidationResult.Failure(message);
        }

        // Get days that would be added by this extension
        if (!ExtensionDaysByType.TryGetValue(requestedExtensionType, out var daysToAdd))
        {
            var message = $"Unknown extension type: {requestedExtensionType}";
            _logger.LogWarning(message);
            return ValidationResult.Failure(message);
        }

        // Check if adding this extension would exceed maximum days
        var totalDaysAfterRequest = usedExtensionDays + daysToAdd;
        if (totalDaysAfterRequest > MaxExtensionDaysPerFund && daysToAdd > 0)
        {
            var message = $"Adding this extension would exceed the maximum of {MaxExtensionDaysPerFund} days. " +
                          $"Current: {usedExtensionDays} days, would be: {totalDaysAfterRequest} days.";
            _logger.LogWarning(message);
            return ValidationResult.Failure(message);
        }

        _logger.LogDebug("Extension request validation passed");
        return ValidationResult.Success();
    }

    public string GetExtensionTypeLabel(ExtensionType type)
    {
        return _extensionTypes.TryGetValue(type, out var info) ? info.Label : type.ToString();
    }

    public string GetWaiverReasonLabel(WaiverReason reason)
    {
        return _waiverReasons.TryGetValue(reason, out var info) ? info.Label : reason.ToString();
    }

    public ExtensionType ParseExtensionType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ExtensionType.None;

        return value.ToLowerInvariant() switch
        {
            "1-month" or "onemonth" => ExtensionType.OneMonth,
            "2-month" or "twomonth" => ExtensionType.TwoMonth,
            "3-month" or "threemonth" => ExtensionType.ThreeMonth,
            "waiver" => ExtensionType.Waiver,
            "deferral" => ExtensionType.Deferral,
            "unlock" => ExtensionType.Unlock,
            _ => ExtensionType.None
        };
    }

    public WaiverReason ParseWaiverReason(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return WaiverReason.ExceptionalCircumstances;

        return value.ToLowerInvariant() switch
        {
            "exceptional-circumstances" or "exceptionalcircumstances" => WaiverReason.ExceptionalCircumstances,
            "fund-dissolving-merger" or "funddissolving merger" => WaiverReason.FundDissolvingMerger,
            "fund-not-launched-deregistered" or "fundnotlaunchedderegistered" => WaiverReason.FundNotLaunchedDeregistered,
            "fund-launched-deregistered" or "fundlaunchedderegistered" => WaiverReason.FundLaunchedDeregistered,
            "fund-insufficient-capital" or "fundinsufficientcapital" => WaiverReason.FundInsufficientCapital,
            "fund-transferring-jurisdiction" or "fundtransferringjurisdiction" => WaiverReason.FundTransferringJurisdiction,
            "fund-unable-audited-accounts" or "fundunableauditedaccounts" => WaiverReason.FundUnableAuditedAccounts,
            "fund-voluntarily-liquidated" or "fundvoluntarilyliquidated" => WaiverReason.FundVoluntarilyLiquidated,
            _ => WaiverReason.ExceptionalCircumstances
        };
    }
}
