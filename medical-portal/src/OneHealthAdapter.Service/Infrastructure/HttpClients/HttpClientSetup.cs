namespace OneHealthAdapter.Infrastructure.HttpClients;

using IdentityModel.Client;
using OneHealthAdapter.Infrastructure.Auth;
using OneHealthAdapter.Extensions;
using OneHealthAdapter.Endorsement.Services.Interfaces;
using OneHealthAdapter.Endorsement.Services;

public static class HttpClientSetup
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services, Configuration config)
    {
        services.AddHttpClient<IAccessTokenClient, AccessTokenClient>();

        services.AddHttpClientWithBaseAddress<IEndorsement, Endorsement>(config.PidpEndorsementAPI.Url)
            .WithBearerToken(new PidpEndorsmentClientCredentials
            {
                Address = config.PidpEndorsementAPI.TokenUrl,
                ClientId = config.PidpEndorsementAPI.ClientId,
                ClientSecret = config.PidpEndorsementAPI.ClientSecret,
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
