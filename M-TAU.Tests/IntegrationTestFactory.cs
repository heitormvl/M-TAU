using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace M_TAU.Tests;

public sealed class IntegrationTestFactory : WebApplicationFactory<Program>
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"mtau-test-{Guid.NewGuid()}.db");

    public string DatabasePath => _dbPath;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = $"Data Source={_dbPath}",
                ["Jwt:Key"] = "M-TAU_Tests_Super_Secret_Key_For_Integration_2026!!",
                ["Jwt:Issuer"] = "M-TAU.API",
                ["Jwt:Audience"] = "M-TAU.Client",
                ["Payments:Provider"] = "MercadoPago-Sandbox",
                ["Payments:CheckoutBaseUrl"] = "https://sandbox.mercadopago.com.br/checkout/v1/redirect"
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try { File.Delete(_dbPath); } catch { }
        }
        base.Dispose(disposing);
    }
}
