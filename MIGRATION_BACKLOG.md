# React to Blazor WASM Migration Plan & Backlog

**Project:** Industry Financial Returns Dashboard UI Conversion  
**Date:** January 15, 2026  
**Target:** Convert `react-figma-ui` (React + TypeScript) to `blazor-wasm-ui` (C# Blazor WASM)

---

## Executive Summary

Converting a feature-rich React financial dashboard (24 business components, 46 shadcn/radix-ui components, 12 mock data items) to Blazor WASM requires 12 implementation tickets organized in 5 phases. The React codebase uses direct `useState` hooks for state management (not react-hook-form), implements complex conditional document requirements, multi-step forms, and file uploads with drag-drop support. This plan accounts for architectural differences between React and Blazor while maintaining UI/UX parity.

**Estimated Effort:** ~97 story points (2–3 week sprint for 2 developers)

---

## Styling & CSS Architecture Guidelines

### Completed: Tailwind CSS Buildout (January 15, 2026)

The styling layer has been modernized to use **Tailwind CSS** with the following setup:

#### Build Pipeline
- **Tool:** Tailwind CLI with PostCSS
- **Configuration:** `tailwind.config.js` (root) + `postcss.config.js`
- **Build Scripts:** 
  - `npm run css:build` - Single build
  - `npm run css:watch` - Watch mode during development
- **Output:** `blazor-wasm-ui/wwwroot/css/tailwind.css` (auto-generated)
- **Load Order in index.html:**
  1. Tailwind CSS (generated, utilities only)
  2. Application CSS (app.css, non-Tailwind utilities)

#### Design Tokens
All color tokens, typography, spacing, and shadows match **react-figma-ui**:
- **Colors:** Neutral grays (50-900), semantic status colors (green, red, yellow, blue, purple)
- **Typography:** 12px→3xl scales, 400/500/600/700 font weights
- **Spacing:** 4px increments (1, 2, 3, 4, 6, 8, 12, 16)
- **Radius:** 0.375rem (sm), 0.5rem (default), 0.625rem (lg)

### Styling Standards (Rule 1–3)

**Rule 1: No `<style>` Blocks in `.razor` Files**
- All component styling must use Tailwind utility classes
- Exception: See Rule 2 for non-Tailwind patterns
- Violators are violations of visual parity contract with React app

**Rule 2: Prefer Tailwind Utilities; Use Global CSS Only for:**
- Layout scaffolding (sidebar, header, footer transitions)
- Reusable patterns not worth converting to utilities (badge colors, status badges)
- Third-party CSS overrides
- Styles requiring non-Tailwind properties (z-index management, column widths)
- Reference: `blazor-wasm-ui/wwwroot/css/app.css` (organized by section)

**Rule 3: Single Generated Tailwind CSS Output**
- Do NOT create multiple Tailwind files
- Do NOT import Tailwind CSS multiple times
- All Tailwind output flows through `wwwroot/css/tailwind.css`
- Regenerate after changes: `npm run css:build`

### Files Migrated (January 2026)
| File | Status | Migration Notes |
|------|--------|-----------------|
| `Shared/TopNavigation.razor` | ✅ DONE | Removed 31 lines of CSS; converted to Tailwind flexbox/border utilities |
| `Shared/Footer.razor` | ✅ DONE | Removed 28 lines of CSS; flex layout + Tailwind text/color utilities |
| `Shared/FiltersSection.razor` | ✅ DONE | Removed 120 lines of CSS; grid layout + input/button utilities |
| `Shared/ReturnsTable.razor` | ✅ DONE | Removed 250+ lines of CSS; table utilities + badge colors → app.css |
| `Pages/Filings.razor` | ✅ DONE | Removed 48 lines of CSS; container/typography utilities |
| `Pages/Dashboard.razor` | ⚠️ DEPRECATED | Marked obsolete; Filings.razor is active |

### Future Implementation Notes
- When adding new components, **always use Tailwind first**
- Check `app.css` before duplicating badge or button styles
- Color palette is frozen per React design (see tailwind.config.js extend.colors)
- For component-specific complexity, create local utility classes in app.css (not inline styles)

---

## Key Findings & Assumption Corrections

### Incorrect Assumptions from Draft Plan
| Assumption | Status | Correction |
|-----------|--------|-----------|
| react-hook-form heavily used | ❌ WRONG | React uses direct `useState` hooks + manual validation (no form library) |
| Complex pagination system | ❌ WRONG | Simple UI pagination (12 items, buttons 1-2-3, prev/next) |
| Multiple chart libraries | ❌ WRONG | Only **Recharts** used (ReportsAnalyticsTab only) |
| PaymentForm is optional | ❌ WRONG | **Required** as Step 2 of ExtensionRequestForm (except deferral) |
| ~25 main components | ❌ WRONG | **24 business + 46 UI components** (shadcn/radix-ui library) |
| Global state management needed | ✅ CORRECT | Only local component state required |
| Tailwind CSS only | ✅ CORRECT | No CSS modules or styled-components |

---

## Confirmed Component Inventory

### Pages (Blazor @page components)

| Component | File Path | Purpose | Primary Dependencies |
|-----------|-----------|---------|----------------------|
| **Dashboard** | src/components/ReturnsTable.tsx | Main data grid view with filters | FiltersSection, ReturnsTable, HelpSection, Footer |
| **Extension Request** | src/components/ExtensionRequestForm.tsx | Multi-step form (Step 1) | FundSummary, SubFundsSection, DocumentUpload, ProgressIndicator, Breadcrumb, Guidelines, ContactSupport, ExtensionSummary |
| **Payment** | src/components/PaymentForm.tsx | Payment processing (Step 2) | ProgressIndicator, Breadcrumb, OrderSummary |
| **Confirmation** | src/components/ConfirmationPage.tsx | Success page (Step 3) | ProgressIndicator, Breadcrumb |
| **Extension View** | src/components/ExtensionView.tsx | Display existing extension | ContactSupport, Guidelines |
| **Unlock Request** | src/components/UnlockRequestForm.tsx | Single-page unlock form | FundSummary, DocumentUpload, ContactSupport |
| **Reports/Analytics** | src/components/ReportsAnalyticsTab.tsx | Charts and metrics | Recharts (Bar, Pie) |

### Shared/Layout Components

| Component | File Path | Purpose | Hierarchy |
|-----------|-----------|---------|-----------|
| **App (Root)** | src/App.tsx | Main container, route logic, state mgmt | Parent of all pages |
| **Sidebar** | src/components/Sidebar.tsx | Navigation menu | Fixed sidebar in layout |
| **Header** | src/components/Header.tsx | Top bar with user info | Fixed header in layout |
| **TopNavigation** | src/components/TopNavigation.tsx | Tab switcher (Active/Submitted/Returned/Reports) | Below header, above content |
| **Footer** | src/components/Footer.tsx | Footer content | Bottom of pages |

### Business Logic Components

| Component | File Path | Purpose | Key Props | Key State |
|-----------|-----------|---------|-----------|-----------|
| **FiltersSection** | src/components/FiltersSection.tsx | Filter controls (search, fund type, status, year) | onResetFilters, filter callbacks | fundType, status[], periodYear[], searchQuery |
| **ReturnsTable** | src/components/ReturnsTable.tsx | Data grid table with actions | returns[], activeTab, onViewReturn, onRequestExtension, onUnlock, onViewExtension | Pagination state (12 items/page) |
| **FundSummary** | src/components/FundSummary.tsx | Read-only return details display | returnData: FinancialReturn | — |
| **SubFundsSection** | src/components/SubFundsSection.tsx | Sub-fund list editor (Multi-Fund only) | subFunds[], availableSubFunds[], callbacks | Dynamic list with inline editing |
| **DocumentUpload** | src/components/DocumentUpload.tsx | File upload manager | onFileUpload, onFileRemove, requiredDocs[], uploadedFiles | File states per doc type |
| **ExtensionSummary** | src/components/ExtensionSummary.tsx | Display extension request summary | extensionData, subFunds, paymentData | — |
| **ExtensionRequestModal** | src/components/ExtensionRequestModal.tsx | Modal wrapper | open, onClose, children | Dialog state |
| **HelpSection** | src/components/HelpSection.tsx | Help panel | — | — |
| **ContactSupport** | src/components/ContactSupport.tsx | Support contact info | — | — |
| **Guidelines** | src/components/Guidelines.tsx | Guidelines display | — | — |
| **ProgressIndicator** | src/components/ProgressIndicator.tsx | Multi-step progress bar | currentStep, totalSteps, stepLabels | — |
| **Breadcrumb** | src/components/Breadcrumb.tsx | Navigation breadcrumb | items (with onClick) | — |
| **UnderConstruction** | src/components/UnderConstruction.tsx | Placeholder | — | — |
| **UnlockConfirmationPage** | src/components/UnlockConfirmationPage.tsx | Success confirmation | onBack | — |

### UI Component Library (46 shadcn/radix-ui Components)

`accordion`, `alert-dialog`, `alert`, `aspect-ratio`, `avatar`, `badge`, `breadcrumb`, `button`, `calendar`, `card`, `carousel`, `chart`, `checkbox`, `collapsible`, `command`, `context-menu`, `dialog`, `drawer`, `dropdown-menu`, `form`, `hover-card`, `input-otp`, `input`, `label`, `menubar`, `navigation-menu`, `pagination`, `popover`, `progress`, `radio-group`, `resizable`, `scroll-area`, `select`, `separator`, `sheet`, `sidebar`, `skeleton`, `slider`, `sonner`, `switch`, `table`, `tabs`, `textarea`, `toggle-group`, `toggle`, `tooltip`, `use-mobile`, `utils`

---

## Confirmed Data Models

### Enums (C# string literal types)

```csharp
// Filing Status (9 values)
enum Status 
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

// Fund Type (2 values)
enum FundType 
{
  MutualFund,
  PrivateFund
}

// Fund Structure (2 values)
enum FundStructure 
{
  SingleFund,
  MultiFund
}

// Extension Type (7 values)
enum ExtensionType 
{
  None,
  OneMonth,
  TwoMonth,
  ThreeMonth,
  Waiver,
  Deferral,
  Unlock
}

// Waiver Reason (8 values)
enum WaiverReason 
{
  ExceptionalCircumstances,
  FundDissolvingMerger,
  FundNotLaunchedDeregistered,
  FundLaunchedDeregistered,
  FundInsufficientCapital,
  FundTransferringJurisdiction,
  FundUnableAuditedAccounts,
  FundVoluntarilyLiquidated
}

// Unlock Reason (3 values)
enum UnlockReason 
{
  FundDeferred,
  ReadyToSubmit,
  Other
}

// Payment Method (5 values)
enum PaymentMethod 
{
  Card,
  WireTransfer,
  BankTransfer,
  Escrow,
  None
}

// Request Status (3 values)
enum RequestStatus 
{
  Pending,
  Approved,
  Rejected
}

// Pending Request Type (4 values, nullable)
enum PendingRequestType 
{
  Extension,
  Waiver,
  Deferral,
  null
}
```

### Core Domain Models

#### FinancialReturn.cs
```csharp
public class FinancialReturn
{
  public string Id { get; set; }                           // Filing ID
  public string EntityId { get; set; }                     // Entity/Fund ID
  public string FundName { get; set; }                     // Fund name
  public FundType FundType { get; set; }                   // Enum
  public FundStructure? FundStructure { get; set; }        // Optional
  public string? ParentFundName { get; set; }              // Multi-fund parent
  public string Period { get; set; }                       // e.g., "Q4 2024"
  public string PeriodDescription { get; set; }            // e.g., "Oct 1 - Dec 31, 2024"
  public string DueDate { get; set; }                      // ISO date string
  public string DueDateDescription { get; set; }           // e.g., "5 days remaining"
  public Status Status { get; set; }                       // Enum
  public int ExtensionsUsed { get; set; }                  // Count
  public int UsedExtensionDays { get; set; }               // Total days
  public int MaxExtensions { get; set; }                   // Maximum allowed
  public string ExtensionDescription { get; set; }         // e.g., "All extensions available"
  public bool CanRequestExtension { get; set; }
  public bool IsUrgent { get; set; }
  public PendingRequestType? PendingRequestType { get; set; }
  public RequestStatus? PendingRequestStatus { get; set; }
}
```

#### SubFund.cs
```csharp
public class SubFund
{
  public string Id { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public string CurrentFilingDate { get; set; }            // ISO date
  public ExtensionType ExtensionType { get; set; }
  public WaiverReason? WaiverReason { get; set; }          // Optional
  public int ExtensionsUsed { get; set; }
  public int UsedExtensionDays { get; set; }
  public int MaxExtensions { get; set; }
  public string? EntityId { get; set; }
}
```

#### ExtensionRequest.cs
```csharp
public class ExtensionRequest
{
  public ExtensionType ExtensionType { get; set; }
  public WaiverReason? WaiverReason { get; set; }
  public List<SubFund>? SubFunds { get; set; }             // Multi-fund only
  public FileInfo? CoverLetter { get; set; }               // Required
  public FileInfo? AuditorLetter { get; set; }             // Conditional
  public FileInfo? OperatorAffidavit { get; set; }         // Conditional
  public FileInfo? AdministratorLetter { get; set; }       // Conditional
  public FileInfo? LiquidatorReport { get; set; }          // Conditional
  public FileInfo? OtherDocuments { get; set; }            // Optional
  public string? AdditionalComments { get; set; }          // Optional
}
```

#### PaymentData.cs
```csharp
public class PaymentData
{
  public PaymentMethod PaymentMethod { get; set; }
  public decimal Amount { get; set; }                      // CI$ currency
  public string TransactionId { get; set; }                // Generated
  public string? CardNumber { get; set; }                  // Last 4 or full
  public string? CardholderName { get; set; }
  public Address? BillingAddress { get; set; }
  public string? EscrowAccount { get; set; }
  public DateTime Timestamp { get; set; }
  public string? UploadedReceipt { get; set; }
}

public class Address
{
  public string Street { get; set; }
  public string City { get; set; }
  public string PostalCode { get; set; }
  public string Country { get; set; }
}
```

#### UnlockRequest.cs
```csharp
public class UnlockRequest
{
  public List<FileInfo>? SupportingDocuments { get; set; } // Optional, multiple
  public string? AdditionalComments { get; set; }
  public UnlockReason UnlockReason { get; set; }           // Required
  public string? UnlockReasonOther { get; set; }           // If reason = Other
  public string? UnlockJustification { get; set; }         // Required, max 1000 chars
}
```

---

## Work Breakdown Structure: 12-Ticket Implementation Backlog

### **Ticket 1: Project Setup & Infrastructure**

**Title:** Initialize Blazor WASM project structure with Tailwind CSS and MudBlazor

**Files to Create/Modify:**
- `blazor-wasm-ui/.csproj` — Create with MudBlazor NuGet packages
- `blazor-wasm-ui/Program.cs` — Register MudBlazor services, configure routing
- `blazor-wasm-ui/App.razor` — Root layout with Router
- `blazor-wasm-ui/Shared/MainLayout.razor` — App shell layout
- `blazor-wasm-ui/Shared/Sidebar.razor` — Navigation sidebar component
- `blazor-wasm-ui/Shared/Header.razor` — Top header bar
- `blazor-wasm-ui/wwwroot/css/app.css` — Tailwind CSS integration
- `blazor-wasm-ui/tailwind.config.js` — Tailwind configuration (copy color scheme from React)
- `blazor-wasm-ui/Models/Enums.cs` — Define all 8 enums

**Acceptance Criteria:**
- ✅ Blazor WASM project compiles without errors
- ✅ Tailwind CSS classes render correctly (test with utility class on element)
- ✅ MudBlazor theme initialized and components available
- ✅ Sidebar and Header render on all pages with correct layout
- ✅ All enums defined with exact string values from React code
- ✅ Routing configured for future pages (/filings, /extension-request, /payment, /confirmation, /unlock-request, /extension-view, /reports)

**Parity Risks:**
- Tailwind color mappings (rgb(78, 152, 210) for blue header, neutral grays, red/green/orange status colors) must match React exactly
- Icon library selection: Map lucide-react icons to MudBlazor Material Icons or Font Awesome

---

### **Ticket 2: Data Models & Services**

**Title:** Create C# domain models and mock data service

**Files to Create/Modify:**
- `blazor-wasm-ui/Models/FinancialReturn.cs`
- `blazor-wasm-ui/Models/SubFund.cs`
- `blazor-wasm-ui/Models/ExtensionRequest.cs`
- `blazor-wasm-ui/Models/PaymentData.cs`
- `blazor-wasm-ui/Models/UnlockRequest.cs`
- `blazor-wasm-ui/Models/Address.cs`
- `blazor-wasm-ui/Services/IFinancialReturnService.cs` — Interface with GetReturns(), GetReturnById(), FilterReturns()
- `blazor-wasm-ui/Services/FinancialReturnService.cs` — Mock implementation with 12+ hardcoded items
- `Program.cs` — Register service in DI container

**Acceptance Criteria:**
- ✅ All 6 domain models created with exact property names and types from React
- ✅ Mock data includes 12 FinancialReturn objects (single-fund and multi-fund variants)
- ✅ FilterReturns() method implements exact React filtering logic:
  - Tab-based status filtering (active, submitted, returned, reports tabs)
  - Fund type filtering (all, mutual, private)
  - Status multi-select filtering
  - Year filtering (2025, 2024, 2023)
  - Search text across fundName, entityId, id
  - Sort by isUrgent (true first), then by fund type
- ✅ Mock data includes at least one multi-fund return with SubFunds array
- ✅ Service injected and working in component tests

**Parity Risks:**
- Date format consistency (ISO 8601 for storage, display format matching React "Oct 1 - Dec 31, 2024")
- Enum values must match exact strings from React (hyphens, camelCase for IDs)
- Extension fee calculations: 625 CI$ per 1-month, 1250 for 2-month, 1875 for 3-month + 25 processing fee + wire transfer fee 6.15

---

### **Ticket 3: Dashboard Page (Filters & Table)**

**Title:** Implement Filings dashboard with ReturnsTable and FiltersSection

**Files to Create/Modify:**
- `blazor-wasm-ui/Pages/Filings.razor`
- `blazor-wasm-ui/Components/FiltersSection.razor`
- `blazor-wasm-ui/Components/ReturnsTable.razor`
- `blazor-wasm-ui/Components/HelpSection.razor`
- `blazor-wasm-ui/Shared/Footer.razor`
- `blazor-wasm-ui/Pages/Filings.razor.cs` — Code-behind for state management

**Acceptance Criteria:**
- ✅ Dashboard displays filtered FinancialReturn items in table format (8 columns: Filing ID, Entity Details, Period, Due Date, Status, Extensions, Extension Status, Actions)
- ✅ Status column renders correct color-coded badges:
  - Available → neutral
  - Prepared/Ready to Submit → blue
  - Processed → green
  - Returned → orange
  - Outstanding → red
  - Waived/Deferred → yellow
- ✅ FiltersSection renders all 4 filters:
  - Search text input (real-time filter)
  - Fund Type dropdown (All, Mutual, Private)
  - Status multi-select popover with checkboxes (changes per tab)
  - Period Year multi-select popover (2025, 2024, 2023)
  - Clear Filters button
- ✅ TopNavigation tab switcher shows 4 tabs (Active, Submitted, Returned, Reports/Analytics), switches views, filters adjust per tab
- ✅ Pagination shows "Showing X of Y returns" with numbered buttons (1, 2, 3) and prev/next arrows
- ✅ Action buttons render dynamically based on return status and canRequestExtension flag
- ✅ Clicking action buttons navigates to correct view (extension-request, unlock-request, etc.) and passes selected return to next component
- ✅ HelpSection and Footer render below table
- ✅ Sidebar shows Filings as active menu item

**Parity Risks:**
- Table column widths and responsive behavior (8 columns → truncate on mobile? Use horizontal scroll?)
- Status badge colors must match React (use Tailwind class mapping)
- Action button logic is complex (11 different scenarios based on status + pendingRequest + canRequestExtension)
- Pagination placeholder styling
- Filter popover positioning and accessibility

---

### **Ticket 4: Extension Request Form — Step 1 (Details)**

**Title:** Build ExtensionRequestForm multi-step form, Step 1 (Request Details)

**Files to Create/Modify:**
- `blazor-wasm-ui/Pages/ExtensionRequest.razor`
- `blazor-wasm-ui/Components/ExtensionRequestForm.razor`
- `blazor-wasm-ui/Components/Breadcrumb.razor`
- `blazor-wasm-ui/Components/ProgressIndicator.razor`
- `blazor-wasm-ui/Components/FundSummary.razor`
- `blazor-wasm-ui/Components/SubFundsSection.razor`
- `blazor-wasm-ui/Components/DocumentUpload.razor`
- `blazor-wasm-ui/Components/ExtensionSummary.razor`
- `blazor-wasm-ui/Components/Guidelines.razor`
- `blazor-wasm-ui/Components/ContactSupport.razor`
- `blazor-wasm-ui/Pages/ExtensionRequest.razor.cs` — Code-behind with multi-step state

**Acceptance Criteria:**
- ✅ Page navigates to ExtensionRequest when "Request Extension" action clicked with selectedReturn passed
- ✅ Step 1 displays: Breadcrumb, ProgressIndicator showing Step 1/3
- ✅ FundSummary shows: Filing ID (padded), Fund Name, Fund Type, Period, Due Date, read-only
- ✅ ExtensionType dropdown renders 7 options with selection required
- ✅ If ExtensionType = "waiver", WaiverReason dropdown appears with 8 options, required
- ✅ If Multi-Fund (fundStructure = "Multi-Fund"):
  - SubFundsSection displays table of available sub-funds (initially 1 selected)
  - At least 1 sub-fund required with valid extensionType
- ✅ DocumentUpload section shows upload boxes for required documents (conditional based on extensionType + waiverReason):
  - Cover Letter: Always required
  - Auditor Letter: Required if 2-month or 3-month extension
  - Operator Affidavit: Required if waiver + specific reasons
  - Administrator Letter: Required if waiver + "fund-launched-deregistered"
  - Liquidator Report: Required if waiver + liquidation reasons
  - Other Documents: Optional
- ✅ Textarea for Additional Comments (optional, max 500 chars)
- ✅ ExtensionSummary preview shows: Fund Name, Extension Type, Waiver Reason (if applicable), Fee calculation
- ✅ Continue to Payment button validates and proceeds, preserving form data

**Parity Risks:**
- Document requirements conditional logic is complex (8 waiver reasons × 2 conditions each)
- File upload drag-drop styling
- Tailwind responsive grid for upload boxes
- SubFundsSection table rendering and inline editing UX
- Validation disabled button state until all required fields met

---

### **Ticket 5: Extension Request Form — Step 2 (Payment)**

**Title:** Build ExtensionRequestForm Step 2 (Payment processing)

**Files to Create/Modify:**
- `blazor-wasm-ui/Components/PaymentForm.razor`
- `blazor-wasm-ui/Components/PaymentForm.razor.cs`
- `blazor-wasm-ui/Services/PaymentService.cs`
- Update `blazor-wasm-ui/Pages/ExtensionRequest.razor.cs`

**Acceptance Criteria:**
- ✅ Step 2 displays: Breadcrumb, ProgressIndicator showing Step 2/3
- ✅ Payment Method selection (RadioGroup):
  - Card Payment
  - Wire Transfer
  - Bank Transfer
  - Escrow
- ✅ If Deferral selected in Step 1, skip to Step 3 (confirmation)
- ✅ If Card Payment selected: Card Number (formatted), Expiry MM/YY, CVV, Cardholder Name, Billing Address inputs
- ✅ If Wire/Bank Transfer selected: Transfer instructions + optional receipt file upload
- ✅ If Escrow selected: Service provider dropdown + account number input
- ✅ Order Summary card shows:
  - Extension Fee: calculated based on extensionType (625 × months)
  - If multi-fund, sum fees of all selected sub-funds
  - Processing Fee: 25 CI$
  - Bank Charges (Wire Transfer): 6.15 CI$
  - Total Amount: Bold, larger text
- ✅ Checkbox: "I agree to Terms & Conditions" (required before submit)
- ✅ Continue to Confirmation button validates and proceeds

**Parity Risks:**
- Card number formatting (mask input, allow only digits)
- Expiry date validation (MM/YY format, not past)
- Currency formatting (1,250.00 CI$)
- Terms & Conditions link/modal

---

### **Ticket 6: Extension Request Form — Step 3 (Confirmation)**

**Title:** Build ExtensionRequestForm Step 3 (Confirmation & Success page)

**Files to Create/Modify:**
- `blazor-wasm-ui/Components/ConfirmationPage.razor`
- Update `blazor-wasm-ui/Pages/ExtensionRequest.razor.cs`

**Acceptance Criteria:**
- ✅ Step 3 displays: Breadcrumb, ProgressIndicator showing Step 3/3
- ✅ Confirmation Card displays (read-only):
  - Reference Number: Generated
  - Fund Name, Period, Entity ID
  - Extension Type selected
  - Waiver Reason (if applicable)
  - Sub-funds list (if multi-fund)
  - Payment Method used
  - Total Amount charged
  - Transaction ID
  - Timestamp
- ✅ Green success checkmark icon at top
- ✅ Download Receipt button (stub)
- ✅ Back to Dashboard button (clears all state, returns to Filings page)

**Parity Risks:**
- Reference number generation (timestamp-based)
- Download Receipt implementation
- Centered card responsive design

---

### **Ticket 7: Unlock Request Form**

**Title:** Implement UnlockRequestForm (single-page form with confirmation dialog)

**Files to Create/Modify:**
- `blazor-wasm-ui/Pages/UnlockRequest.razor`
- `blazor-wasm-ui/Components/UnlockRequestForm.razor`
- `blazor-wasm-ui/Components/UnlockConfirmationPage.razor`
- `blazor-wasm-ui/Pages/UnlockRequest.razor.cs`

**Acceptance Criteria:**
- ✅ Page navigates to UnlockRequest when "Unlock Filing" action clicked with selectedReturn passed
- ✅ Form displays:
  - Breadcrumb (Filings → {Fund Name} → Unlock Request)
  - FundSummary: Filing ID, Fund Name, Fund Type, Period, Due Date, Status (read-only)
  - UnlockReason RadioGroup with 3 options: Fund Deferred, Ready to Submit, Other
  - If reason = "Other": TextInput for custom reason (required, max 100 chars)
  - Textarea for Justification (required, max 1000 chars)
  - Textarea for Additional Comments (optional, max 500 chars)
  - FileUpload for Supporting Documents (optional, multiple files)
  - ContactSupport panel
- ✅ Form validation: UnlockReason required, Justification required, custom reason required if Other selected
- ✅ Buttons: Cancel (return to dashboard), Submit Request (validate, show confirmation dialog)
- ✅ ConfirmationDialog displays: Title, Details, Cancel/Confirm buttons
- ✅ After confirmation submit: Generate reference number, Show UnlockConfirmationPage with success message and Back to Dashboard button

**Parity Risks:**
- RadioGroup rendering
- Conditional field visibility
- File upload styling consistency
- Dialog/Modal styling

---

### **Ticket 8: Reports/Analytics Tab**

**Title:** Implement ReportsAnalyticsTab with charts and metrics

**Files to Create/Modify:**
- `blazor-wasm-ui/Pages/ReportsAnalytics.razor`
- `blazor-wasm-ui/Components/ReportsAnalyticsTab.razor`
- `blazor-wasm-ui/Services/AnalyticsService.cs`
- `.csproj` — Add ChartJS.Blazor or MudBlazor Chart NuGet package

**Acceptance Criteria:**
- ✅ Reports tab in TopNavigation navigates to this page
- ✅ Page displays:
  - Metric cards showing: Total Returns, Processed, Pending, Outstanding, Average Days
  - Bar Chart: Returns by Status
  - Pie Chart: Fund Type Distribution
- ✅ Charts render correctly with legend, labels, and tooltip on hover
- ✅ Layout: Cards in responsive grid (2-3 columns), charts below
- ✅ Colors: Status colors match ReturnsTable

**Parity Risks:**
- Chart library selection
- Chart data format and mock data
- Responsive chart sizing

---

### **Ticket 9: Shared UI Components Library**

**Title:** Create reusable Blazor UI wrapper components

**Files to Create/Modify:**
- `blazor-wasm-ui/Components/UI/MudButton.razor`
- `blazor-wasm-ui/Components/UI/MudBadge.razor`
- `blazor-wasm-ui/Components/UI/MudSelect.razor`
- `blazor-wasm-ui/Components/UI/MudCheckbox.razor`
- `blazor-wasm-ui/Components/UI/MudRadio.razor`
- `blazor-wasm-ui/Components/UI/MudTextField.razor`
- `blazor-wasm-ui/Components/UI/MudDialog.razor`
- `blazor-wasm-ui/Components/UI/MudCard.razor`
- `blazor-wasm-ui/Components/UI/MudTable.razor`

**Acceptance Criteria:**
- ✅ All 9 components created with consistent parameter names
- ✅ ButtonVariant enum (default, destructive, outline, secondary, ghost, link) with CSS class mapping
- ✅ Size parameter (sm, md, lg) with Tailwind mapping
- ✅ Color props for badges (status-specific colors)
- ✅ Icons passed via IconName parameter
- ✅ Tailwind classes applied correctly
- ✅ RenderFragment support for children content
- ✅ Components used consistently across Tickets 3-8

**Parity Risks:**
- Variant/size CSS mapping accuracy
- Icon library consistency
- Accessibility (ARIA labels, keyboard navigation)

---

### **Ticket 10: Navigation & Routing**

**Title:** Implement page navigation, tab switching, and state preservation across multi-step forms

**Files to Create/Modify:**
- `blazor-wasm-ui/Pages/_Host.cshtml`
- `blazor-wasm-ui/App.razor`
- `blazor-wasm-ui/Services/NavigationService.cs`
- `blazor-wasm-ui/Services/FormStateService.cs`
- `blazor-wasm-ui/Shared/TopNavigation.razor`
- Update all Page components

**Acceptance Criteria:**
- ✅ Routes defined for all pages:
  - `/filings` — Dashboard (default)
  - `/extension-request` — Step 1
  - `/payment` or `/extension-request/payment` — Step 2
  - `/confirmation` or `/extension-request/confirmation` — Step 3
  - `/unlock-request` — Unlock form
  - `/extension-view` — View existing extension
  - `/reports` — Reports tab
- ✅ TopNavigation tabs switch between main views
- ✅ Back button navigation preserves form state
- ✅ Back to Dashboard button clears state and returns to `/filings`
- ✅ Selected return object passed via CascadingParameter or FormStateService

**Parity Risks:**
- State preservation mechanism choice (CascadingParameter? Service? SessionStorage?)
- Deep linking (can user bookmark `/extension-request` without context?)
- Back button handling (browser back vs app back button)

---

### **Ticket 11: File Upload & Drag-Drop Functionality**

**Title:** Implement drag-drop file upload with validation and preview

**Files to Create/Modify:**
- `blazor-wasm-ui/Components/DocumentUpload.razor` (from Ticket 4)
- `blazor-wasm-ui/Services/FileService.cs`
- `blazor-wasm-ui/Components/UI/FileUploadBox.razor`
- `blazor-wasm-ui/wwwroot/js/file-upload.js` (if needed)

**Acceptance Criteria:**
- ✅ FileUploadBox component renders:
  - Dashed border container (gray default, blue on drag-over)
  - Cloud/Upload icon
  - Drag-drop text and "click to browse" link
  - File input (hidden, accept=".pdf,.doc,.docx")
- ✅ Drag-over behavior: Border color changes to blue
- ✅ Drop behavior:
  - File validated (type, size ≤ 10MB per file, max 5 files)
  - Error toast if invalid
  - Valid files listed below upload box with ✓ checkmark
  - X button to remove each file
- ✅ Click to browse opens file dialog
- ✅ Accepted file types: .pdf, .doc, .docx (validate MIME type and extension)
- ✅ Multi-file support
- ✅ File removal: X button removes from list
- ✅ Parent component receives file list via callback

**Parity Risks:**
- File size validation (10MB per file)
- MIME type matching
- Drag-drop browser compatibility
- Error messaging (toast notifications)

---

### **Ticket 12: Testing, Polish & Documentation**

**Title:** Write bUnit tests, styling refinements, accessibility, and documentation

**Files to Create/Modify:**
- `blazor-wasm-ui/Tests/Pages/FilingsPageTests.cs`
- `blazor-wasm-ui/Tests/Components/ReturnsTableTests.cs`
- `blazor-wasm-ui/Tests/Components/ExtensionRequestFormTests.cs`
- `blazor-wasm-ui/Tests/Components/PaymentFormTests.cs`
- `blazor-wasm-ui/Tests/Services/PaymentServiceTests.cs`
- `blazor-wasm-ui/Tests/Services/FileServiceTests.cs`
- `README.md`
- `MIGRATION_NOTES.md`

**Acceptance Criteria:**
- ✅ At least 30 bUnit tests covering:
  - Dashboard page renders with mock data
  - Filters work correctly (search, fund type, status, year multi-select)
  - ReturnsTable displays correct columns and action buttons
  - ExtensionRequestForm Step 1 validation (required fields, conditional docs)
  - ExtensionRequestForm Step 2 fee calculations
  - Payment method switching shows/hides correct fields
  - File upload accepts/rejects files based on type and size
  - Navigation between steps preserves data
  - UnlockRequest form validation works
- ✅ Style refinements:
  - Responsive design tested on mobile/tablet/desktop
  - Color consistency: Status badges, buttons, links match React colors
  - Padding/spacing match React layout
  - Font sizes and weights match React typography
  - Hover/focus states on buttons and inputs
  - Loading placeholders (skeleton screens) for async operations
- ✅ Accessibility:
  - Form labels associated with inputs
  - ARIA labels on icon buttons
  - Keyboard navigation (Tab through form fields, Enter to submit)
  - Color contrast ratios (WCAG AA minimum)
  - Error messages announce to screen readers
- ✅ Documentation:
  - README with setup, running instructions, project structure
  - Code comments on complex logic
  - MIGRATION_NOTES documenting:
    - React → Blazor component mapping
    - State management pattern changes (useState → @state)
    - Styling approach
    - Known differences
    - Testing approach

**Parity Risks:**
- Cross-browser testing
- Mobile responsiveness
- Performance (WASM initial load time)
- Icon rendering consistency

---

## Implementation Schedule & Dependencies

### Phase 1: Foundation (Week 1, Days 1–2)
- **Ticket 1:** Project Setup & Infrastructure (8 pts)
- **Ticket 2:** Data Models & Services (5 pts)
- **Ticket 9:** UI Components Library (8 pts)

### Phase 2: Data Display (Week 1, Days 3–4)
- **Ticket 3:** Dashboard Page (Filters & Table) (13 pts)
- **Ticket 8:** Reports/Analytics Tab (8 pts)

### Phase 3: Multi-Step Form Workflow (Week 2, Days 1–3)
- **Ticket 4:** Extension Request Step 1 (13 pts)
- **Ticket 5:** Extension Request Step 2 (8 pts)
- **Ticket 6:** Extension Request Step 3 (3 pts)
- **Ticket 11:** File Upload & Drag-Drop (5 pts)
- **Ticket 10:** Navigation & Routing (5 pts)

### Phase 4: Additional Features (Week 2, Days 4–5)
- **Ticket 7:** Unlock Request Form (8 pts)

### Phase 5: Testing & Polish (Week 3)
- **Ticket 12:** Testing, Polish & Documentation (13 pts)

**Total Estimated Effort:** ~97 story points  
**Team:** 2 developers  
**Timeline:** 2–3 week sprint

---

## Critical Parity Risks Summary

| Risk | Severity | Mitigation |
|------|----------|-----------|
| **Status badge color accuracy** | High | Create color reference map, test on actual returns |
| **Complex document requirements logic** | High | Unit test all 8 waiver reasons × conditional combos |
| **Form state preservation across steps** | High | Use FormStateService or CascadingParameters; test navigation |
| **File upload drag-drop behavior** | Medium | Test on Chrome, Firefox, Edge; handle file size/type edge cases |
| **Table responsiveness (8 columns)** | Medium | Mobile: horizontal scroll or column collapsing strategy |
| **Fee calculation accuracy** | Medium | Unit test against React fee logic; verify all payment methods |
| **Icon visual parity** | Medium | Map all lucide-react icons to MudBlazor Material Icons |
| **Tailwind CSS color mapping** | Medium | Use exact RGB/hex values from React inline styles |
| **State persistence (back button)** | Medium | Test back navigation from each form step with data preservation |

---

## Document History

- **2026-01-15:** Initial comprehensive plan created with 12-ticket backlog, data model validation, and component inventory
