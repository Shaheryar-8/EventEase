using EventEase;
using EventEase.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// In Blazor WebAssembly "scoped" means one instance per browser tab, which is exactly the app state we want.
builder.Services.AddScoped<BrowserStorage>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<UserSessionService>();

await builder.Build().RunAsync();
