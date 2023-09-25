using System;
using System.Collections.Generic;
using System.Linq;

namespace ciklonalozi.Models
{
    public class EditOrderModel
    {
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public DateTime? Arrival { get; set; }
        public DateTime? Arrived { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime? Returned { get; set; }
        public decimal? EstimatedPrice { get; set; }
        public int? Effort { get; set; }
        public decimal? RealPrice { get; set; }
        public string? OfferNumber { get; set; }
        public bool Removed { get; set; }

        public Dictionary<string, string>? Validate()
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(ContactName))
                errors.Add(nameof(ContactName), "Obavezno");

            if (string.IsNullOrWhiteSpace(Subject))
                errors.Add(nameof(Subject), "Obavezno");

            if (!Arrival.HasValue)
                errors.Add(nameof(Arrival), "Obavezno");
            else if (Holidays.TryGetHoliday(Arrival.Value, out var arrivalHoliday))
                errors.Add(nameof(Arrival), arrivalHoliday);

            if (Arrived.HasValue && Holidays.TryGetHoliday(Arrived.Value, out var arrivedHoliday))
                errors.Add(nameof(Arrived), arrivedHoliday);

            if (Completed.HasValue && Holidays.TryGetHoliday(Completed.Value, out var completedHoliday))
                errors.Add(nameof(Completed), completedHoliday);

            if (Returned.HasValue && Holidays.TryGetHoliday(Returned.Value, out var returnedHoliday))
                errors.Add(nameof(Returned), returnedHoliday);

            if (Completed.HasValue && !RealPrice.HasValue)
                errors.Add(nameof(RealPrice), "Obavezno za zatvoreni nalog");

            if (!Effort.HasValue)
                errors.Add(nameof(Effort), "Obavezno");

            if (errors.Any())
                return errors;

            return null;
        }
    }
}