using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ciklonalozi.Data;
using ciklonalozi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ciklonalozi.Controllers;

[ApiController]
[AllowAnonymous]
public class ExtController : ControllerBase
{
    const int DAYS = 14;
    static readonly CultureInfo CI = CultureInfo.GetCultureInfo("HR-hr");
    readonly ILogger<ExtController> _logger;
    readonly AppDbContext _db;
    public ExtController(ILogger<ExtController> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }
    [HttpOptions(C.Routes.ApiDates)]
    [HttpOptions(C.Routes.ApiRequest)]
    public IActionResult PreflightRoute() => NoContent();

    [HttpPost(C.Routes.ApiDates)]
    public async Task<IActionResult> GetAvailableDates(DateRequestModel req)
    {
        var from = DateTime.UtcNow.Date.AddDays(1);
        var to = from.AddDays(DAYS);
        var orders = await _db.Orders
            .Where(o => !o.Removed && o.Arrival > from && o.Arrival < to)
            .ToListAsync();

        var availability = Enumerable.Range(0, DAYS - 1).ToDictionary(i => from.AddDays(i), _ => C.MaxEffort);
        foreach (var order in orders)
            availability[order.Arrival.Date] -= order.Effort;

        var results = new List<DateResponseModel>();
        // Obviously we can't use Sunday and holidays
        // In case customer wants more than fits in a day (max effort), we'll return days which are empty
        foreach (var day in availability)
            if (day.Key.DayOfWeek != DayOfWeek.Sunday && day.Key.DayOfWeek != DayOfWeek.Saturday && !Holidays.IsHoliday(day.Key))
                if (day.Value > req.Effort || day.Value == C.MaxEffort)
                    results.Add(new() { Date = day.Key.ToString("yyyy-MM-dd"), Text = GetText(day.Key) });

        return Ok(results);
    }
    [HttpPost(C.Routes.ApiRequest)]
    public async Task<IActionResult> SaveRequest(ServiceRequestModel req)
    {
        var request = new Request(req);
        _db.Requests.Add(request);
        await _db.SaveChangesAsync();

        RequestPublisher.Raise();

        return Ok();
    }
    [HttpGet(C.Routes.ApiQr)]
    public async Task<IActionResult> Qr(string orderHash)
    {
        var orderId = C.Hasher.Ids.DecodeSingle(orderHash.ToUpper());
        var order = await _db.Orders.FindAsync(orderId);
        if (order == null)
            return NotFound();

        var model = new QrModel(order);
        return Ok(model);
    }
    [HttpPost(C.Routes.ApiQr)]
    public async Task<IActionResult> Qr(string orderHash, [FromForm] QrUpdateModel model)
    {
        var orderId = C.Hasher.Ids.DecodeSingle(orderHash.ToUpper());
        var order = await _db.Orders.FindAsync(orderId);
        if (order == null)
            return NotFound();

        order.ContactEmail = model.Email.Trim();

        await _db.SaveChangesAsync();

        var vm = new QrModel(order);
        return Ok(vm);
    }
    static string GetText(DateTime dt)
        => $"{dt.ToString(CI.DateTimeFormat.ShortDatePattern)} - {CI.DateTimeFormat.GetDayName(dt.DayOfWeek)}";
}