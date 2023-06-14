using ciklonalozi.Data;

namespace ciklonalozi.Models;

public class QrModel
{
    public QrModel(Order order)
    {
        Id = order.OrderId;
        Hash = C.Hasher.Ids.Encode(order.OrderId);
        Arrival = C.Display(order.Arrival);
        if (order.Arrived.HasValue)
            Arrived = C.Display(order.Arrived);
        if (order.Returned.HasValue)
            Returned = C.Display(order.Returned);
        if (order.Completed.HasValue)
            Completed = C.Display(order.Completed);
        Contact = order.ContactName;
        Subject = order.Subject;
        Description = order.Description;
        if (order.EstimatedPrice.HasValue)
            EstimatedPrice = C.Display(order.EstimatedPrice);
        if (order.RealPrice.HasValue)
            RealPrice = C.Display(order.RealPrice);
        Email = order.ContactEmail;
    }
    public int Id { get; set; }
    public string? Hash { get; set; }
    public string? Arrival { get; set; }
    public string? Arrived { get; set; }
    public string? Returned { get; set; }
    public string? Completed { get; set; }
    public string? Contact { get; set; }
    public string? Subject { get; set; }
    public string? Description { get; set; }
    public string? EstimatedPrice { get; set; }
    public string? RealPrice { get; set; }
    public string? Email { get; set; }
}