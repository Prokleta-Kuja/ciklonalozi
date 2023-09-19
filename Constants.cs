using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HashidsNet;
using WebPush;

namespace ciklonalozi
{
    public static class C
    {
        public const string CorsPolicy = nameof(CorsPolicy);
        public static readonly TimeZoneInfo TZ = TimeZoneInfo.FindSystemTimeZoneById("Europe/Zagreb");
        public static readonly CultureInfo CI = CultureInfo.GetCultureInfo("hr-HR");
        public const int MaxEffort = 100;
        public static string Display(DateTime? dt, bool showTime = true, string empty = "-")
        {
            if (!dt.HasValue)
                return empty;

            var printDt = TimeZoneInfo.ConvertTimeFromUtc(dt.Value, C.TZ);
            var format = C.CI.DateTimeFormat.ShortDatePattern;

            if (showTime)
                format += $" {C.CI.DateTimeFormat.ShortTimePattern}";

            return printDt.ToString(format);
        }
        public static string Display(Decimal? num)
        {
            if (num.HasValue)
                return num.Value.ToString("#,##0.00");
            else
                return "-";
        }
        public static string Normalize(string str)
        {//ĐURO???
            var normalizedString = str.ToUpperInvariant().Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(normalizedString.Length);
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }
            var normalized = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
            return normalized;
        }
        public static class Hasher
        {
            public static readonly Hashids Ids = new(Env.SALT, 4, Env.ALPHABET);
            public static string GetQrUrl(int id)
            {
                var hash = Ids.Encode(id);
                var url = Debugger.IsAttached ? $"http://localhost:7347/nalog/?qr={hash}" : $"https://ciklo-sport.hr/nalog/?qr={hash}";
                return url;
            }
        }
        public static class Env
        {
            public static readonly string URL = Environment.GetEnvironmentVariable(nameof(URL)) ?? "http://localhost:5000/";
            public static readonly string SALT = Environment.GetEnvironmentVariable(nameof(SALT)) ?? "promijeni me";
            public static readonly string ALPHABET = Environment.GetEnvironmentVariable(nameof(ALPHABET)) ?? "ABCDEFGHIJKLMNOPRSTUVZ0123456789";
        }
        public static class Routes
        {
            public const string Root = "/";
            public const string Requests = "/requests";
            public const string Request = "/requests/{Id:int}";
            public static string RequestFor(int id) => $"{Requests}/{id}";
            public const string Analysis = "/anal";
            public const string ApiDates = "/api/dates";
            public const string ApiRequest = "/api/request";
            public const string ApiQr = "/api/qr/{orderHash}";
        }
        public static class Settings
        {
            public static string VapidPath => Path.Combine(Environment.CurrentDirectory, "data", "vapid.json");
            public static string DataPath => Path.Combine(Environment.CurrentDirectory, "data", "app.db");
            public static readonly string AppDbConnectionString = $"Data Source={DataPath};Cache=Shared";
        }
    }
}