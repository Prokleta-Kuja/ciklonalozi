using System.Collections.Generic;

namespace ciklonalozi.Models;

public class ServiceRequestModel
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Tel { get; set; }
    public required string Subject { get; set; }
    public string? Note { get; set; }
    public required string Date { get; set; }
    public required Dictionary<string, int> Services { get; set; }
}