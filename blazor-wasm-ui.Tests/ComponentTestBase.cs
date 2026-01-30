using Microsoft.AspNetCore.Components;

namespace blazor_wasm_ui.Tests;

/// <summary>
/// Base class for component tests providing common setup and utilities.
/// </summary>
public abstract class ComponentTestBase<TComponent> where TComponent : ComponentBase, new()
{
    /// <summary>
    /// Gets the component instance for testing.
    /// </summary>
    protected TComponent Component { get; private set; }

    /// <summary>
    /// Initializes a new instance of the component for testing.
    /// </summary>
    protected ComponentTestBase()
    {
        Component = new TComponent();
    }
}