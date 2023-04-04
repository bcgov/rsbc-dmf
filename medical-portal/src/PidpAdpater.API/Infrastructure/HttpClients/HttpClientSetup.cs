namespace pdipadapter.Infrastructure.HttpClients;

using IdentityModel.Client;

using pdipadapter.Infrastructure.Auth;
using pdipadapter.Infrastructure.HttpClients.Keycloak;
using pdipadapter.Infrastructure.HttpClients.Mail;
using pdipadapter.Extensions;
using pdipadapter.Infrastructure.HttpClients.JustinParticipant;
using MedicalPortal.API.Features.Endorsement.Services.Interfaces;
using MedicalPortal.API.Features.Endorsement.Services;
using Microsoft.Identity.Client;

public static class HttpClientSetup
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services, PdipadapterConfiguration config)
    {
        services.AddHttpClient<IAccessTokenClient, AccessTokenClient>();

        //services.AddHttpClientWithBaseAddress<IAddressAutocompleteClient, AddressAutocompleteClient>(config.AddressAutocompleteClient.Url);

        services.AddHttpClientWithBaseAddress<IChesClient, ChesClient>(config.ChesClient.Url)
            .WithBearerToken(new ChesClientCredentials
            {
                Address = config.ChesClient.TokenUrl,
                ClientId = config.ChesClient.ClientId,
                ClientSecret = config.ChesClient.ClientSecret
            });

        services.AddHttpClientWithBaseAddress<IEndorsement, Endorsement>(config.PidpEndorsementAPI.Url)
            .WithBearerToken(new PidpEndorsmentClientCredentials
            {
                Address = config.PidpEndorsementAPI.TokenUrl,
                ClientId = config.PidpEndorsementAPI.ClientId,
                ClientSecret = config.PidpEndorsementAPI.ClientSecret
            });

        //services.AddHttpClientWithBaseAddress<ILdapClient, LdapClient>(config.LdapClient.Url);

        services.AddHttpClientWithBaseAddress<IKeycloakAdministrationClient, KeycloakAdministrationClient>(config.Keycloak.AdministrationUrl)
            .WithBearerToken(new KeycloakAdministrationClientCredentials
            {
                Address = config.Keycloak.TokenUrl,
                ClientId = config.Keycloak.AdministrationClientId,
                ClientSecret = config.Keycloak.AdministrationClientSecret
            });

        services.AddHttpClientWithBaseAddress<IJustinParticipantClient, JustinParticipantClient>(config.JustinParticipantClient.Url);

        services.AddTransient<ISmtpEmailClient, SmtpEmailClient>();

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
