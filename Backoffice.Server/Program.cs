using Application.AuthOptions;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Backoffice.Application.Queries.Administrators;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

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
    
    b.RegisterType(typeof(ChangesSaver<>))
        .As(typeof(IChangesSaver))
        .InstancePerLifetimeScope();
});

var app = builder.Build();

app.Run();