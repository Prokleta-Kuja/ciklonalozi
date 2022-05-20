using System;
using System.Globalization;
using System.Linq;

namespace ciklonalozi.Data
{
    public class Order
    {
        public Order(string contactName, string? contactPhone, string subject, DateTime arrival)
        {
            ContactName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(contactName);
            if (!string.IsNullOrWhiteSpace(contactPhone))
            {
                ContactPhone = contactPhone;
                ContactPhoneNormalized = string.Concat(contactPhone.Where(char.IsDigit));
            }
            Subject = subject;
            Arrival = arrival;
        }

        public int OrderId { get; set; }
        public string ContactName { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactPhoneNormalized { get; set; }
        public string Subject { get; set; }
        public string? Description { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime? Arrived { get; set; }
        public DateTime? Completed { get; set; }
    }
}