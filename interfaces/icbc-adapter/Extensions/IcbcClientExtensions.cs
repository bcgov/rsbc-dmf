using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Rsbc.Dmf.Interfaces.IcbcAdapter
{
    public partial class IcbcClient : ServiceClient<IcbcClient>, IIcbcClient
    {
        [ActivatorUtilitiesConstructor]
        public IcbcClient(HttpClient httpClient, IConfiguration configuration)
        {
            string icbc_adapter_base_uri = configuration["ICBC_ADAPTER_BASE_URI"];
            // string bearer_token = $"Bearer {configuration["ICBC_ADAPTER_JWT_TOKEN"]}";

            BaseUri = new Uri(icbc_adapter_base_uri);

            // configure the HttpClient that is used for our direct REST calls.
            HttpClient = httpClient;
            // HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            // HttpClient.DefaultRequestHeaders.Add("Authorization", bearer_token);
        }
    }

}
