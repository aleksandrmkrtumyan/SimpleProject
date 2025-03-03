using AutoMapper;
using AutoMapper.QueryableExtensions;
using Backoffice.Application.Queries.Administrators.Models;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Backoffice.Application.Queries.Administrators;

public class GetAllAdministratorsQuery
{
    #region Fields
    
    private readonly IRepository<Administrator> administratorRepository;
    private readonly MapperConfiguration mapperConfiguration;

    #endregion Fields
    
    #region Constructor

    public GetAllAdministratorsQuery(
        IRepository<Administrator> administratorRepository,
        MapperConfiguration mapperConfiguration
        )
    {
        this.administratorRepository = administratorRepository;
        this.mapperConfiguration = mapperConfiguration;
    }
    
    #endregion Constructor
    
    #region Method

    public async Task<List<AdministratorModel>> Execute()
    {
        var administrators =
            await administratorRepository.ProjectTo<AdministratorModel>(mapperConfiguration).ToListAsync();
        
        return administrators;

    }
    
    #endregion Methods
}