using AutoMapper;
using Domain;

namespace Backoffice.Application.Queries.Administrators.Models;

public class AdministratorModel
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Username 
    /// </summary>
    public string Username { get; set; }
    
    /// <summary>
    /// E-mail
    /// </summary>
    public string Email { get; set; }

    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Administrator, AdministratorModel>()
                .ForMember(model => model.Name, opts => opts.MapFrom(entity => entity.Name));
        }
    }
}