using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Backoffice.Client;
using Backoffice.Client.ServiceProxies;
using Grpc.Net.Client.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
// builder.Services.AddScoped(sp =>new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddServiceProxies();

builder.Services.AddScoped(provider => 
    new Executor(provider.GetService<Backoffice.Executor.ExecutorClient>()));

builder.Services.AddGrpcClient<Backoffice.Executor.ExecutorClient>((_, options) =>
{
    options.Address = new Uri(builder.HostEnvironment.BaseAddress);
    options.ChannelOptionsActions.Add(o =>
    {
        o.MaxReceiveMessageSize = 50 * 1024 * 1024;
    });
})
.ConfigurePrimaryHttpMessageHandler(()=> 
    new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler()));
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();