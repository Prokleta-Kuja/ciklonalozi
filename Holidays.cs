using System;
using System.Collections.Generic;

namespace ciklonalozi;

public static class Holidays
{
    static Dictionary<DateOnly, string> Dates { get; set; } = new();
    static Holidays()
    {
        // Tu pogledaj svake godine i promijeni ispod https://neradni-dani.com/neradni-dani-2024.php
        // Godina, mjesec, dan  
        Dates.Add(new(2024, 3, 31), "Uskrs");
        Dates.Add(new(2024, 4, 1), "Prvi april");
        Dates.Add(new(2024, 4, 17), "Dan izbora");
        Dates.Add(new(2024, 5, 1), "Dan neradnika"); 
        Dates.Add(new(2024, 5, 30), "Dan državnosti i Tijelovo");
        Dates.Add(new(2024, 6, 22), "Dan antifašističke borbe");
        Dates.Add(new(2024, 8, 5), "Dan pobjede");
        Dates.Add(new(2024, 8, 15), "Velika Gospa");
        Dates.Add(new(2024, 11, 1), "Svi sveti");
        Dates.Add(new(2024, 11, 18), "Dan sjećanja");
        Dates.Add(new(2024, 12, 25), "Žićbo");
        Dates.Add(new(2024, 12, 26), "Štefanje");

        Dates.Add(new(2025, 1, 1), "Nova godina");
        Dates.Add(new(2025, 1, 6), "Tri kralja/srpski badnjak");
        Dates.Add(new(2025, 4, 20), "Uskrs");
        Dates.Add(new(2025, 4, 21), "Uskršnji ponedjeljak");
        Dates.Add(new(2025, 5, 1), "Dan neradnika"); 
        Dates.Add(new(2025, 5, 30), "Dan državnosti");
        Dates.Add(new(2025, 6, 19), "Tijelovo");
        Dates.Add(new(2025, 6, 22), "Dan antifašističke borbe");
        Dates.Add(new(2025, 8, 5), "Dan pobjede");
        Dates.Add(new(2025, 8, 15), "Velika Gospa");
        Dates.Add(new(2025, 11, 1), "Svi sveti");
        Dates.Add(new(2025, 11, 18), "Dan sjećanja");
        Dates.Add(new(2025, 12, 25), "Žićbo");
        Dates.Add(new(2025, 12, 26), "Štefanje");

        Dates.Add(new(2026, 1, 1), "Nova godina");
        Dates.Add(new(2026, 1, 6), "Tri kralja/srpski badnjak");
        Dates.Add(new(2026, 4, 5), "Uskrs");
        Dates.Add(new(2026, 4, 6), "Uskršnji ponedjeljak");
        Dates.Add(new(2026, 5, 1), "Dan neradnika"); 
        Dates.Add(new(2026, 5, 30), "Dan državnosti");
        Dates.Add(new(2026, 6, 4), "Tijelovo");
        Dates.Add(new(2026, 6, 22), "Dan antifašističke borbe");
        Dates.Add(new(2026, 8, 5), "Dan pobjede");
        Dates.Add(new(2026, 8, 15), "Velika Gospa");
        Dates.Add(new(2026, 11, 1), "Svi sveti");
        Dates.Add(new(2026, 11, 18), "Dan sjećanja");
        Dates.Add(new(2026, 12, 25), "Žićbo");
        Dates.Add(new(2026, 12, 26), "Štefanje");
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