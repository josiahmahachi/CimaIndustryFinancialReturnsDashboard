using blazor_wasm_ui.Models;
using blazor_wasm_ui.Services;
using Microsoft.AspNetCore.Components;

namespace blazor_wasm_ui.Pages;

/// <summary>
/// Filings dashboard page code-behind
/// Manages state and filtering logic mirroring React App.tsx
/// </summary>
public partial class Filings : ComponentBase
{
    [Inject]
    private IReturnsDataService? ReturnsDataService { get; set; }

    // State properties
    private List<FinancialReturn> AllReturns { get; set; } = new();
    private List<FinancialReturn> FilteredReturns { get; set; } = new();

    // Filter state
    private string ActiveTab { get; set; } = "active";
    private string FundType { get; set; } = "all";
    private List<string> Status { get; set; } = new();
    private List<string> PeriodYear { get; set; } = new();
    private string SearchQuery { get; set; } = "";

    // Pagination state
    private int CurrentPage { get; set; } = 1;
    private const int ItemsPerPage = 12;
    private int TotalPages { get; set; } = 1;

    /// <summary>
    /// Get the list of years available from the data
    /// </summary>
    private List<string> AvailableYears { get; set; } = new() { "2025", "2024", "2023" };

    /// <summary>
    /// Get tab content (title, description, showStatusFilter)
    /// </summary>
    private (string Title, string Description, bool ShowStatusFilter) GetTabContent() => ActiveTab switch
    {
        "active" => ("Active Filings", "Filings that are actively being worked on or pending action. Includes available, prepared, ready to submit, outstanding, and deferred filings.", true),
        "submitted" => ("Submitted Filings", "Filings that have been successfully processed by the authority. View your filing history and approved extensions.", false),
        "returned" => ("Returned Filings", "Filings that were rejected or sent back by the authority requiring amendments. Review feedback and resubmit your filings.", false),
        _ => ("Returns Filing & Extension Management Portal", "File your financial returns or request deadline extensions as needed.", true)
    };

    /// <summary>
    /// Lifecycle: Initialize component and load data
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        if (ReturnsDataService != null)
        {
            AllReturns = await ReturnsDataService.GetReturnsAsync();
            await ApplyFiltersAsync();
        }
    }

    /// <summary>
    /// Apply all filters and update FilteredReturns
    /// Mirrors React App.tsx filtering logic
    /// </summary>
    private async Task ApplyFiltersAsync()
    {
        await Task.Run(() =>
        {
            var filtered = AllReturns.AsEnumerable();

            // 1. Tab-based filtering
            filtered = ActiveTab switch
            {
                "active" => filtered.Where(r =>
                    r.Status == ReturnStatus.Available ||
                    r.Status == ReturnStatus.Prepared ||
                    r.Status == ReturnStatus.ReadyToSubmit ||
                    r.Status == ReturnStatus.Outstanding ||
                    r.Status == ReturnStatus.Deferred),
                "submitted" => filtered.Where(r => r.Status == ReturnStatus.Processed),
                "returned" => filtered.Where(r => r.Status == ReturnStatus.Returned),
                "reports-analytics" => filtered, // No filtering for reports tab
                _ => filtered
            };

            // 2. Fund type filtering (skip for reports-analytics)
            if (ActiveTab != "reports-analytics" && FundType != "all")
            {
                var targetFundType = FundType == "mutual" ? Models.FundType.MutualFund : Models.FundType.PrivateFund;
                filtered = filtered.Where(r => r.FundType == targetFundType);
            }

            // 3. Status filtering (only for active tab)
            if (ActiveTab == "active" && Status.Count > 0)
            {
                filtered = filtered.Where(r =>
                    Status.Contains(r.Status.ToString().ToLower())
                );
            }

            // 4. Period year filtering
            if (PeriodYear.Count > 0)
            {
                filtered = filtered.Where(r =>
                {
                    var match = System.Text.RegularExpressions.Regex.Match(r.Period, @"\d{4}");
                    return match.Success && PeriodYear.Contains(match.Value);
                });
            }

            // 5. Search query filtering
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var searchLower = SearchQuery.ToLower();
                filtered = filtered.Where(r =>
                {
                    var entityIdPadded = r.EntityId.PadLeft(6, '0');
                    var idPadded = r.Id.PadLeft(6, '0');

                    return r.FundName.ToLower().Contains(searchLower) ||
                           r.EntityId.ToLower().Contains(searchLower) ||
                           entityIdPadded.Contains(searchLower) ||
                           r.Id.ToLower().Contains(searchLower) ||
                           idPadded.Contains(searchLower);
                });
            }

            FilteredReturns = filtered.ToList();
            CalculatePagination();
        });
    }

    /// <summary>
    /// Calculate pagination info
    /// </summary>
    private void CalculatePagination()
    {
        TotalPages = (int)Math.Ceiling((double)FilteredReturns.Count / ItemsPerPage);
        if (TotalPages == 0) TotalPages = 1;
        
        if (CurrentPage > TotalPages)
            CurrentPage = TotalPages;
    }

    /// <summary>
    /// Get the current page of returns
    /// </summary>
    private List<FinancialReturn> GetPagedReturns()
    {
        var startIndex = (CurrentPage - 1) * ItemsPerPage;
        return FilteredReturns.Skip(startIndex).Take(ItemsPerPage).ToList();
    }

    /// <summary>
    /// Handle tab change
    /// </summary>
    private async Task OnTabChangeAsync(string tab)
    {
        ActiveTab = tab;
        CurrentPage = 1;
        Status.Clear(); // Clear status filter when changing tabs
        await ApplyFiltersAsync();
    }

    /// <summary>
    /// Handle fund type filter change
    /// </summary>
    private async Task OnFundTypeChangeAsync(string value)
    {
        FundType = value;
        CurrentPage = 1;
        await ApplyFiltersAsync();
    }

    /// <summary>
    /// Handle status filter change
    /// </summary>
    private async Task OnStatusChangeAsync(List<string> values)
    {
        Status = values;
        CurrentPage = 1;
        await ApplyFiltersAsync();
    }

    /// <summary>
    /// Handle period year filter change
    /// </summary>
    private async Task OnPeriodYearChangeAsync(List<string> values)
    {
        PeriodYear = values;
        CurrentPage = 1;
        await ApplyFiltersAsync();
    }

    /// <summary>
    /// Handle search query change
    /// </summary>
    private async Task OnSearchChangeAsync(string value)
    {
        SearchQuery = value;
        CurrentPage = 1;
        await ApplyFiltersAsync();
    }

    /// <summary>
    /// Handle reset filters
    /// </summary>
    private async Task OnResetFiltersAsync()
    {
        FundType = "all";
        Status.Clear();
        PeriodYear.Clear();
        SearchQuery = "";
        CurrentPage = 1;
        await ApplyFiltersAsync();
    }

    /// <summary>
    /// Handle pagination: go to next page
    /// </summary>
    private async Task NextPageAsync()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Handle pagination: go to previous page
    /// </summary>
    private async Task PrevPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Handle pagination: go to specific page
    /// </summary>
    private async Task GoToPageAsync(int page)
    {
        if (page >= 1 && page <= TotalPages)
        {
            CurrentPage = page;
            await Task.CompletedTask;
        }
    }

    // Action callbacks (to be implemented in future tickets)

    private void OnViewReturn(string returnId)
    {
        // TODO: Implement view return details (Ticket 4+)
        Console.WriteLine($"View return: {returnId}");
    }

    private void OnViewExtension(string returnId)
    {
        // TODO: Implement view extension details (Ticket 4+)
        Console.WriteLine($"View extension for return: {returnId}");
    }

    private void OnFileReturn(string returnId)
    {
        // TODO: Implement file return workflow (Ticket 4+)
        Console.WriteLine($"File return: {returnId}");
    }

    private void OnRequestUnlock(string returnId)
    {
        // TODO: Implement unlock request workflow (Ticket 4+)
        Console.WriteLine($"Request unlock for return: {returnId}");
    }
}
