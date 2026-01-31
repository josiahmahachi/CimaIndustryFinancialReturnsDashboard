using blazor_wasm_ui;
using blazor_wasm_ui.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IReturnsDataService, ReturnsDataService>();
builder.Services.AddScoped<IFileValidationService, FileValidationService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();

await builder.Build().RunAsync();
