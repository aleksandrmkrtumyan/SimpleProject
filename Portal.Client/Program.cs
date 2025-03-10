using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portal;
using Portal.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//
// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
// // палач!
builder.Services.AddScoped(provider =>
    new Portal.Client.ServiceProxies.Executor(provider.GetService<Executor.ExecutorClient>() ));
//
//
builder.Services.AddGrpcClient<Executor.ExecutorClient>((_, options) =>
    {
        options.Address = new Uri(builder.HostEnvironment.BaseAddress);
        options.ChannelOptionsActions.Add(o => {o.MaxReceiveMessageSize = 10 * 1024 * 1024;});
    })
    .ConfigurePrimaryHttpMessageHandler(
        () => new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler()));
builder.Services.AddScoped(sp => new HttpClient{BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

await builder.Build().RunAsync();