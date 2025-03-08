using Application.AuthOptions;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Backoffice.Application.Commands.Administrators;
using Backoffice.Application.Queries.Administrators;
using Backoffice.Server.Services;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<CreateDefaultAdministratorCommand>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddGrpc(options =>
{
    options.MaxReceiveMessageSize = 50 * 1024 * 1024;
});
builder.Services.AddControllers();

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

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseGrpcWeb();

app.MapGrpcService<ExecutorService>()
    .EnableGrpcWeb();
app.MapControllers();

app.MapFallbackToFile("index.html");
app.Run();