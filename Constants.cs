using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using HashidsNet;
using WebPush;

namespace ciklonalozi
{
    public static class C
    {
        public static readonly TimeZoneInfo TZ = TimeZoneInfo.FindSystemTimeZoneById("Europe/Zagreb");
        public static readonly CultureInfo CI = CultureInfo.GetCultureInfo("hr-HR");
        public static class Hasher
        {
            public static readonly Hashids Ids = new(Env.SALT, 4, Env.ALPHABET);
            public static string GetQrUrl(int id)
            {
                var hash = Ids.Encode(id);
                return $"{Env.URL.TrimEnd('/')}/{hash}";
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
            public const string Invoices = "/invoices";
            public const string Invoice = "/invoices/{Id:int}";
            public static string InvoiceFor(int id) => $"{Invoices}/{id}";
            public const string InvoicePrint = "/invoices/{Id:int}/print";
            public static string InvoicePrintFor(int id) => $"{Invoices}/{id}/print";
        }
        public static class Settings
        {
            public static string VapidPath => Path.Combine(Environment.CurrentDirectory, "data", "vapid.json");
            public static string DataPath => Path.Combine(Environment.CurrentDirectory, "data", "app.db");
            public static readonly string AppDbConnectionString = $"Data Source={DataPath};Cache=Shared";
        }
        public static class Vapid
        {
            static readonly FileInfo file = new(Settings.VapidPath);
            static readonly JsonSerializerOptions serializerOptions = new()
            {
                WriteIndented = true,
                IgnoreReadOnlyProperties = true,
            };
            public static VapidDetails Current { get; private set; } = new();
            public static async ValueTask LoadAsync()
            {
                if (file.Exists)
                {
                    Current = await ReadAsync();
                    Current.Subject = Env.URL;
                }
                else
                {
                    Current = VapidHelper.GenerateVapidKeys();
                    Current.Subject = Env.URL;

                    await WriteAsync(Current);
                }
            }
            public static async Task<VapidDetails> ReadAsync()
            {
                var contents = await File.ReadAllTextAsync(file.FullName);
                var vapid = JsonSerializer.Deserialize<VapidDetails>(contents) ?? throw new JsonException("Could not load vapid file");
                return vapid;
            }
            public static async ValueTask WriteAsync(VapidDetails vapid)
            {
                var contents = JsonSerializer.Serialize(vapid, serializerOptions);
                await File.WriteAllTextAsync(file.FullName, contents);
            }
        }
    }
}