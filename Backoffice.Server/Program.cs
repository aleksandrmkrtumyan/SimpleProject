using Application.AuthOptions;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Backoffice.Application.Commands.Administrators;
using Backoffice.Application.Queries.Administrators;
using Backoffice.Server.Services;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddScoped<CreateDefaultAdministratorCommand>();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddGrpc(options =>
{
    options.MaxReceiveMessageSize = 50 * 1024 * 1024;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(b =>
{
    b.RegisterAssemblyTypes(typeof(GetAllAdministratorsQuery).Assembly)
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();
    b.RegisterAssemblyTypes(typeof(PortalAuthOptions).Assembly)
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();
    
    b.RegisterGeneric(typeof(Repository<>))
        .As(typeof(IRepository<>))
        .InstancePerLifetimeScope();
    
    b.RegisterType<ChangesSaver<ApplicationDbContext>>()
        .As<IChangesSaver>()
        .InstancePerLifetimeScope();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
    
    var createDefaultAdministratorCommand = scope.ServiceProvider.GetRequiredService<CreateDefaultAdministratorCommand>();
    await createDefaultAdministratorCommand.Execute();
}

app.UseRouting();
app.UseBlazorFrameworkFiles();
app.UseGrpcWeb();

app.MapGrpcService<ExecutorService>()
    .EnableGrpcWeb();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.UseStaticFiles();
app.Run();