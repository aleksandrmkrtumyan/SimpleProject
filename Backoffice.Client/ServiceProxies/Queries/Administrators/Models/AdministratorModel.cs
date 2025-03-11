namespace Backoffice.Client.ServiceProxies.Queries.Administrators.Models;

public partial class AdministratorModel
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
}