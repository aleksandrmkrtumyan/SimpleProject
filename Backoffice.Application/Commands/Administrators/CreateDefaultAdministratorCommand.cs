using Application.Utilities;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Backoffice.Application.Commands.Administrators;

public class CreateDefaultAdministratorCommand
{
    #region Fields
    
    private readonly IRepository<Administrator> administratorRepository;
    private readonly IChangesSaver changesSaver;

    #endregion Fields
    
    #region Constructor

    public CreateDefaultAdministratorCommand
        (
            IRepository<Administrator> administratorRepository,
            IChangesSaver changesSaver
            )
    {
        this.administratorRepository = administratorRepository;
        this.changesSaver = changesSaver;
    }
    
    #endregion Constructor
    
    #region Methods

    public async Task Execute()
    {
        var hasAny = await administratorRepository.AnyAsync();
        if (!hasAny)
        {
            return;
        }

        var passwordHash = HashHelper.GetHashString("admin");
        
        var administrator = new Administrator
        {
            Username = "admin",
            Email = "admin@admin.com",
            AuthTokenId = Guid.NewGuid(),
            PasswordHash = passwordHash
        };
        
    }
    
    #endregion Methods
}