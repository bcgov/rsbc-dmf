using Xunit;

namespace Rsbc.Dmf.IcbcAdapter.Tests
{
    [CollectionDefinition(nameof(HttpClientCollection))]
    public class HttpClientCollection : ICollectionFixture<HttpClientFixture>
    { }
}
