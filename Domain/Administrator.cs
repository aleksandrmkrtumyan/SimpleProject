using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Administrator
{
    /// <summary>
    /// Id
    /// </summary>
    [Display(Name = "Id")]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Username 
    /// </summary>
    [Display(Name = "Username")]
    public string Username { get; set; }
    
    /// <summary>
    /// Password hash
    /// </summary>
    [Display(Name = "Password")]
    public string PasswordHash{ get; set;}
    
    /// <summary>
    /// E-mail
    /// </summary>
    [Display(Name = "Email")]
    public string Email { get; set; }
    
    /// <summary>
    /// AuthTokenId
    /// </summary>
    [Display(Name = "AuthTokenId")]
    public Guid AuthTokenId { get; set; }
}