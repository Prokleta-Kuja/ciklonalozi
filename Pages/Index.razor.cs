using System;
using System.Collections.Generic;
using ciklonalozi.Data;

namespace ciklonalozi.Pages
{
    public partial class Index
    {
        DateTime Today { get; } = DateTime.UtcNow.Date;
        DateTime Yesterday { get; } = DateTime.UtcNow.AddDays(-1).Date;
        List<Order> Orders { get; set; } = new();
        void AddClicked() { }
    }
}