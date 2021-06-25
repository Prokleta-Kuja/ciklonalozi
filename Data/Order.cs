using System;
using System.Collections.Generic;

namespace ciklonalozi.Data
{
    public class Order
    {
        public Order(string contactName, string? contactPhone, string subject, DateTime arrival)
        {
            ContactName = contactName;
            ContactPhone = contactPhone;
            Subject = subject;
            Arrival = arrival;
        }

        public int OrderId { get; set; }
        public string ContactName { get; set; }
        public string? ContactPhone { get; set; }
        public string Subject { get; set; }
        public string? Note { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime? Arrived { get; set; }
        public DateTime? Completed { get; set; }

        public ICollection<OrderItem>? Items { get; set; }
    }
}