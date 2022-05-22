using System;
using System.IO;

namespace ciklonalozi
{
    public static class C
    {
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
            public static string DataPath => Path.Combine(Environment.CurrentDirectory, "data", "app.db");
            public static readonly string AppDbConnectionString = $"Data Source={DataPath};Cache=Shared";
        }
    }
}