using blazor_wasm_ui.Models;

namespace blazor_wasm_ui.Services;

/// <summary>
/// Service interface for managing financial return data
/// </summary>
public interface IReturnsDataService
{
    /// <summary>
    /// Get all financial returns
    /// </summary>
    /// <returns>List of FinancialReturn objects</returns>
    Task<List<FinancialReturn>> GetReturnsAsync();

    /// <summary>
    /// Get a specific return by ID
    /// </summary>
    /// <param name="id">Return ID</param>
    /// <returns>FinancialReturn object or null if not found</returns>
    Task<FinancialReturn?> GetReturnByIdAsync(string id);

    /// <summary>
    /// Filter returns based on criteria
    /// </summary>
    /// <param name="fundType">Filter by fund type (null for all)</param>
    /// <param name="status">Filter by status (null for all)</param>
    /// <param name="searchQuery">Search query for fund name or entity ID</param>
    /// <returns>Filtered list of FinancialReturn objects</returns>
    Task<List<FinancialReturn>> FilterReturnsAsync(FundType? fundType = null, ReturnStatus? status = null, string? searchQuery = null);
}
