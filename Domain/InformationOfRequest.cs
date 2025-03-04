using System.ComponentModel.DataAnnotations;

namespace Domain;

public class InformationOfRequest
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Date and time
    /// </summary>
    [Display(Name = "Date and time")]
    [Required]
    public DateTime DateAndTime { get; set; }

    /// <summary>
    /// Command or query
    /// </summary>
    [Display(Name = "Command or query")]
    [Required]
    public string CommandOrRequest { get; set; }

    /// <summary>
    /// Input params
    /// </summary>
    [Display(Name = "Input params")]
    [Required]
    public string InputParams { get; set; }


    /// <summary>
    /// Response time
    /// </summary>
    [Display(Name = "Response time")]
    [Required]
    public double ResponseTime { get; set; }

    /// <summary>
    /// Response size
    /// </summary>
    [Display(Name = "Response size")]
    [Required]
    public long ResponseSize { get; set; }

    /// <summary>
    /// Scope Id
    /// </summary>
    public Guid? ScopeId { get; set; }
}