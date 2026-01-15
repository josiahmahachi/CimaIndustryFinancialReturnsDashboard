namespace blazor_wasm_ui.Models;

/// <summary>
/// Fund type enumeration - derived from React mockReturnsData
/// </summary>
public enum FundType
{
    MutualFund,
    PrivateFund
}

/// <summary>
/// Fund structure enumeration - single or multi-fund
/// </summary>
public enum FundStructure
{
    SingleFund,
    MultiFund
}

/// <summary>
/// Financial return status enumeration
/// Confirmed values from React ReturnStatus type
/// </summary>
public enum ReturnStatus
{
    Available,
    Prepared,
    ReadyToSubmit,
    Processed,
    Returned,
    Waived,
    Outstanding,
    Deferred,
    UnderReview
}

/// <summary>
/// Type of pending request (extension, waiver, deferral)
/// </summary>
public enum PendingRequestType
{
    Extension,
    Waiver,
    Deferral
}

/// <summary>
/// Status of a pending request
/// </summary>
public enum PendingRequestStatus
{
    Pending,
    Approved,
    Rejected
}
