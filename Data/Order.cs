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
            ContactNameNormalized = C.Normalize(ContactName);
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
        public string ContactNameNormalized { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhoneNormalized { get; set; }
        public string Subject { get; set; }
        public string? Description { get; set; }
        public string? DescriptionNormalized { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime? Arrived { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime? Returned { get; set; }
        public decimal? EstimatedPrice { get; set; }
        public decimal? RealPrice { get; set; }
        public string? OfferNumber { get; set; }
        public int Effort { get; set; }
        public bool Removed { get; set; }

        public Request? Request { get; set; }

        // display
        public string IsReverse => OrderId % 2 == 0 ? "flex-row-reverse" : string.Empty;
        public string IsLeft => OrderId % 2 == 0 ? "text-left" : string.Empty;
    }
}