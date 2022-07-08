using Xunit;

namespace Rsbc.Unit.Tests.Dmf.LegacyAdapter
{
    [CollectionDefinition(nameof(HttpClientCollection))]
    public class HttpClientCollection : ICollectionFixture<HttpClientFixture>
    { }
}
