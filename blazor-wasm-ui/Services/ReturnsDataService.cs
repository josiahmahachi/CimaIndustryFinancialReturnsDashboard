using blazor_wasm_ui.Models;

namespace blazor_wasm_ui.Services;

/// <summary>
/// Service implementation with mock data from React App.tsx
/// Provides financial return data and filtering capabilities
/// </summary>
public class ReturnsDataService : IReturnsDataService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReturnsDataService> _logger;
    private List<FinancialReturn>? _mockData;

    public ReturnsDataService(HttpClient httpClient, ILogger<ReturnsDataService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Get all mock financial returns
    /// </summary>
    public async Task<List<FinancialReturn>> GetReturnsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all returns");
            return await Task.FromResult(GetMockData());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching returns");
            throw;
        }
    }

    /// <summary>
    /// Get a specific return by ID
    /// </summary>
    public async Task<FinancialReturn?> GetReturnByIdAsync(string id)
    {
        try
        {
            _logger.LogInformation("Fetching return with ID: {Id}", id);
            return await Task.FromResult(GetMockData().FirstOrDefault(r => r.Id == id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching return with ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Filter returns based on criteria
    /// </summary>
    public async Task<List<FinancialReturn>> FilterReturnsAsync(FundType? fundType = null, ReturnStatus? status = null, string? searchQuery = null)
    {
        try
        {
            _logger.LogInformation("Filtering returns - FundType: {FundType}, Status: {Status}, SearchQuery: {SearchQuery}", 
                fundType, status, searchQuery);
            
            var returns = GetMockData();
            
            if (fundType.HasValue && fundType != FundType.None)
            {
                returns = returns.Where(r => r.FundType == fundType).ToList();
            }
            
            if (status.HasValue)
            {
                returns = returns.Where(r => r.Status == status).ToList();
            }
            
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var searchLower = searchQuery.ToLower();
                returns = returns.Where(r =>
                    r.FundName.ToLower().Contains(searchLower) ||
                    r.EntityId.ToLower().Contains(searchLower) ||
                    r.Id.ToLower().Contains(searchLower)
                ).ToList();
            }
            
            return await Task.FromResult(returns);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering returns");
            throw;
        }
    }

    /// <summary>
    /// Generate mock financial returns data matching React mockReturnsData
    /// </summary>
    private List<FinancialReturn> GetMockData()
    {
        if (_mockData != null)
            return _mockData;

        _mockData = new List<FinancialReturn>
        {
            new()
            {
                Id = "1",
                EntityId = "101",
                FundName = "Healthcare Innovation Fund",
                FundType = FundType.MutualFund,
                FundStructure = FundStructure.SingleFund,
                Period = "Q4 2024",
                PeriodDescription = "Oct 1 - Dec 31, 2024",
                DueDate = "March 31, 2025",
                DueDateDescription = "5 days remaining",
                Status = ReturnStatus.Available,
                ExtensionsUsed = 0,
                UsedExtensionDays = 0,
                MaxExtensions = 3,
                ExtensionDescription = "All extensions available",
                CanRequestExtension = true,
                IsUrgent = false
            },
            new()
            {
                Id = "2",
                EntityId = "102",
                FundName = "Technology Growth Fund",
                FundType = FundType.PrivateFund,
                Period = "FY 2024",
                PeriodDescription = "Jan 1 - Dec 31, 2024",
                DueDate = "June 30, 2025",
                DueDateDescription = "125 days remaining",
                Status = ReturnStatus.Available,
                ExtensionsUsed = 1,
                UsedExtensionDays = 30,
                MaxExtensions = 3,
                ExtensionDescription = "60 days remaining",
                CanRequestExtension = true,
                IsUrgent = false,
                PendingRequestType = PendingRequestType.Extension,
                PendingRequestStatus = PendingRequestStatus.Pending
            },
            new()
            {
                Id = "3",
                EntityId = "103",
                FundName = "Emerging Markets Equity Fund",
                FundType = FundType.MutualFund,
                FundStructure = FundStructure.SingleFund,
                Period = "FY 2024",
                PeriodDescription = "Jan 1 - Dec 31, 2024",
                DueDate = "September 30, 2024",
                DueDateDescription = "91 days past filing due date",
                Status = ReturnStatus.Outstanding,
                ExtensionsUsed = 3,
                UsedExtensionDays = 90,
                MaxExtensions = 3,
                ExtensionDescription = "All extensions used",
                CanRequestExtension = false,
                IsUrgent = false
            },
            new()
            {
                Id = "4",
                EntityId = "104",
                FundName = "Real Estate Investment Fund",
                FundType = FundType.PrivateFund,
                Period = "Q2 2024",
                PeriodDescription = "Apr 1 - Jun 30, 2024",
                DueDate = "November 30, 2024",
                DueDateDescription = "Filed on December 30, 2024 with extension",
                Status = ReturnStatus.Processed,
                ExtensionsUsed = 1,
                UsedExtensionDays = 30,
                MaxExtensions = 3,
                ExtensionDescription = "Extension approved - 30 days",
                CanRequestExtension = false,
                IsUrgent = false
            },
            new()
            {
                Id = "5",
                EntityId = "105",
                FundName = "Global Equity Sub-Fund",
                FundType = FundType.MutualFund,
                FundStructure = FundStructure.MultiFund,
                ParentFundName = "Sustainable Growth Multi-Fund Complex",
                Period = "Q4 2024",
                PeriodDescription = "Oct 1 - Dec 31, 2024",
                DueDate = "March 31, 2025",
                DueDateDescription = "45 days remaining",
                Status = ReturnStatus.Available,
                ExtensionsUsed = 0,
                UsedExtensionDays = 0,
                MaxExtensions = 3,
                ExtensionDescription = "All extensions available",
                CanRequestExtension = true,
                IsUrgent = false,
                PendingRequestType = PendingRequestType.Waiver,
                PendingRequestStatus = PendingRequestStatus.Pending
            },
            new()
            {
                Id = "6",
                EntityId = "106",
                FundName = "Infrastructure Development Fund",
                FundType = FundType.PrivateFund,
                Period = "Q3 2024",
                PeriodDescription = "Jul 1 - Sep 30, 2024",
                DueDate = "March 31, 2025",
                DueDateDescription = "Prepared for submission",
                Status = ReturnStatus.Prepared,
                ExtensionsUsed = 1,
                UsedExtensionDays = 30,
                MaxExtensions = 3,
                ExtensionDescription = "Prepared by filing team",
                CanRequestExtension = true,
                IsUrgent = false
            },
            new()
            {
                Id = "7",
                EntityId = "107",
                FundName = "Small Cap Growth Fund",
                FundType = FundType.MutualFund,
                FundStructure = FundStructure.SingleFund,
                Period = "Q2 2024",
                PeriodDescription = "Apr 1 - Jun 30, 2024",
                DueDate = "December 20, 2024",
                DueDateDescription = "Ready for submission",
                Status = ReturnStatus.ReadyToSubmit,
                ExtensionsUsed = 0,
                UsedExtensionDays = 0,
                MaxExtensions = 3,
                ExtensionDescription = "Awaiting final submission",
                CanRequestExtension = true,
                IsUrgent = false,
                PendingRequestType = PendingRequestType.Deferral,
                PendingRequestStatus = PendingRequestStatus.Pending
            },
            new()
            {
                Id = "8",
                EntityId = "108",
                FundName = "Asia Pacific Growth Fund",
                FundType = FundType.MutualFund,
                FundStructure = FundStructure.SingleFund,
                Period = "Q3 2024",
                PeriodDescription = "Jul 1 - Sep 30, 2024",
                DueDate = "January 15, 2025",
                DueDateDescription = "Returned on January 20, 2025",
                Status = ReturnStatus.Returned,
                ExtensionsUsed = 1,
                UsedExtensionDays = 30,
                MaxExtensions = 3,
                ExtensionDescription = "Amendments required - resubmit within 30 days",
                CanRequestExtension = false,
                IsUrgent = false
            },
            new()
            {
                Id = "9",
                EntityId = "109",
                FundName = "Sustainable Energy Fund",
                FundType = FundType.PrivateFund,
                Period = "FY 2023",
                PeriodDescription = "Jan 1 - Dec 31, 2023",
                DueDate = "September 30, 2024",
                DueDateDescription = "Returned on November 5, 2024",
                Status = ReturnStatus.Returned,
                ExtensionsUsed = 2,
                UsedExtensionDays = 60,
                MaxExtensions = 3,
                ExtensionDescription = "Incomplete documentation - 1 extension remaining",
                CanRequestExtension = true,
                IsUrgent = false
            },
            new()
            {
                Id = "10",
                EntityId = "110",
                FundName = "European Bond Fund",
                FundType = FundType.MutualFund,
                FundStructure = FundStructure.SingleFund,
                Period = "Q1 2024",
                PeriodDescription = "Jan 1 - Mar 31, 2024",
                DueDate = "July 31, 2024",
                DueDateDescription = "Returned on October 10, 2024",
                Status = ReturnStatus.Returned,
                ExtensionsUsed = 0,
                UsedExtensionDays = 0,
                MaxExtensions = 3,
                ExtensionDescription = "Calculation errors identified - all extensions available",
                CanRequestExtension = true,
                IsUrgent = false
            },
            new()
            {
                Id = "11",
                EntityId = "111",
                FundName = "Emerging Markets Bond Fund",
                FundType = FundType.MutualFund,
                FundStructure = FundStructure.SingleFund,
                Period = "Q4 2024",
                PeriodDescription = "Oct 1 - Dec 31, 2024",
                DueDate = "March 31, 2025",
                DueDateDescription = "92 days remaining",
                Status = ReturnStatus.Available,
                ExtensionsUsed = 2,
                UsedExtensionDays = 60,
                MaxExtensions = 3,
                ExtensionDescription = "30 days remaining",
                CanRequestExtension = true,
                IsUrgent = false
            },
            new()
            {
                Id = "12",
                EntityId = "112",
                FundName = "Commodity Trading Fund",
                FundType = FundType.PrivateFund,
                Period = "FY 2024",
                PeriodDescription = "Jan 1 - Dec 31, 2024",
                DueDate = "June 30, 2025",
                DueDateDescription = "182 days remaining",
                Status = ReturnStatus.Available,
                ExtensionsUsed = 0,
                UsedExtensionDays = 0,
                MaxExtensions = 3,
                ExtensionDescription = "All extensions available",
                CanRequestExtension = true,
                IsUrgent = false
            }
        };

        return _mockData;
    }
}
