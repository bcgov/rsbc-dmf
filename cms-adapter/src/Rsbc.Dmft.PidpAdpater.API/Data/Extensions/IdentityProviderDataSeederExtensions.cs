using pdipadapter.Data.Seed;

namespace pdipadapter.Data.Extensions
{
    public static class IdentityProviderDataSeederExtensions
    {
        public static async Task EnsureSeedData(this IHostBuilder builder, string[] args)
        {
            builder.ConfigureLogging(x =>
            {
                x.AddConsole();
            });
            var host = builder.Build();

            var services = host.Services.CreateScope();
            var service = services.ServiceProvider.GetService<IdentityProviderDataSeeder>();

            await service.Seed();
        }
    }
}
