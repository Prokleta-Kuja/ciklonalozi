using System;
using System.Collections.Generic;
using System.Linq;

namespace ciklonalozi.Models
{
    public class CreateOrderModel
    {
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public DateTime? ArrivalOrArrived { get; set; }
        public bool IsArrival { get; set; }
        public decimal? EstimatedPrice { get; set; }

        public Dictionary<string, string>? Validate()
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(ContactName))
                errors.Add(nameof(ContactName), "Obavezno");

            if (string.IsNullOrWhiteSpace(Subject))
                errors.Add(nameof(Subject), "Obavezno");

            if (!ArrivalOrArrived.HasValue)
                errors.Add(nameof(ArrivalOrArrived), "Obavezno");

            if (errors.Any())
                return errors;

            return null;
        }
    }
}