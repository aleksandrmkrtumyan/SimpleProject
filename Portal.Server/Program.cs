using Portal.Server.Services;
using Microsoft.AspNetCore.Builder;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

builder.Services.AddGrpc();
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();

app.UseBlazorFrameworkFiles();

app.UseGrpcWeb();
// app.MapGrpcService<ExecutorService>()
//     .EnableGrpcWeb();
// app.MapControllers();
// app.MapFallbackToFile("index.html");
app.UseEndpoints(endpoints =>
{
    endpoints
        .MapGrpcService<ExecutorService>()
        .EnableGrpcWeb();

    endpoints.MapControllers();
   
    endpoints.MapFallbackToFile("index.html");
});
app.UseStaticFiles();
app.Run();