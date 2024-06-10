namespace PidpAdapter.Infrastructure.HttpClients;

using IdentityModel.Client;
using PidpAdapter.Extensions;
using PidpAdapter.Endorsement.Services.Interfaces;
using PidpAdapter.Endorsement.Services;

public static class HttpClientSetup
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IAccessTokenClient, AccessTokenClient>();

        services.AddHttpClientWithBaseAddress<IPidpHttpClient, PidpHttpClient>(configuration["PIDP_URL"])
            .WithBearerToken(new ClientCredentialsTokenRequest
            {
                Address = configuration["PIDP_TOKEN_URL"],
                ClientId = configuration["PIDP_CLIENT_ID"],
                ClientSecret = configuration["PIDP_CLIENT_SECRET"],
            });

        return services;
    }

    public static IHttpClientBuilder AddHttpClientWithBaseAddress<TClient, TImplementation>(this IServiceCollection services, string baseAddress)
        where TClient : class
        where TImplementation : class, TClient
        => services.AddHttpClient<TClient, TImplementation>(client => client.BaseAddress = new Uri(baseAddress.EnsureTrailingSlash()));

    public static IHttpClientBuilder WithBearerToken<T>(this IHttpClientBuilder builder, T credentials) where T : ClientCredentialsTokenRequest
    {
        builder.Services.AddSingleton(credentials)
            .AddTransient<BearerTokenHandler<T>>();
        builder.AddHttpMessageHandler<BearerTokenHandler<T>>();
        return builder;
    }
}
