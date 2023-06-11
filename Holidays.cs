using System;
using System.Collections.Generic;

namespace ciklonalozi;

public static class Holidays
{
    static Dictionary<DateOnly, string> Dates { get; set; } = new();
    static Holidays()
    {
        // Tu pogledaj svake godine i promijeni ispod https://neradni-dani.com/neradni-dani-2023.php
        // Godina, mjesec, dan  
        Dates.Add(new(2023, 5, 30), "Dan državnosti");
        Dates.Add(new(2023, 6, 8), "Tijelovo");
        Dates.Add(new(2023, 6, 22), "Dan antifašističke borbe");
        Dates.Add(new(2023, 8, 5), "Dan pobjede");
        Dates.Add(new(2023, 8, 15), "Velika Gospa");
        Dates.Add(new(2023, 11, 1), "Svi sveti");
        Dates.Add(new(2023, 11, 18), "Dan sjećanja");
        Dates.Add(new(2023, 12, 25), "Žićbo");
        Dates.Add(new(2023, 12, 26), "Štefanje");
    }
    public static bool IsHoliday(DateTime dt)
    {
        if (dt.Kind == DateTimeKind.Utc)
            dt = TimeZoneInfo.ConvertTimeFromUtc(dt, C.TZ);
        return Dates.ContainsKey(DateOnly.FromDateTime(dt));
    }
    public static bool TryGetHoliday(DateTime dt, out string holiday)
    {
        if (dt.Kind == DateTimeKind.Utc)
            dt = TimeZoneInfo.ConvertTimeFromUtc(dt, C.TZ);
        return Dates.TryGetValue(DateOnly.FromDateTime(dt), out holiday!);
    }
}