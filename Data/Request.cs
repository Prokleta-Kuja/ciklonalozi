using System;
using System.Collections.Generic;
using ciklonalozi.Models;

namespace ciklonalozi.Data;

public class Request
{
    public Request()
    {
        Subject = null!;
        Contact = null!;
        Phone = null!;
        Email = null!;
        Description = null!;
        Note = null!;
    }
    public Request(ServiceRequestModel req)
    {
        Subject = req.Subject;
        Contact = $"{req.FirstName} {req.LastName}";
        Phone = req.Tel;
        Email = req.Email;
        Date = DateOnly.Parse(req.Date);
        Note = req.Note;
        Created = DateTime.UtcNow;

        var items = new List<string>(req.Services.Count);
        foreach (var item in req.Services)
            items.Add(item.Value == 1 ? item.Key : $"{item.Key} x{item.Value}");

        Description = string.Join(", ", items);
    }
    public int RequestId { get; set; }
    public int? OrderId { get; set; }
    public string Subject { get; set; }
    public string Contact { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Description { get; set; }
    public DateOnly Date { get; set; }
    public string? Note { get; set; }
    public DateTime Created { get; set; }

    public Order? Order { get; set; }
}