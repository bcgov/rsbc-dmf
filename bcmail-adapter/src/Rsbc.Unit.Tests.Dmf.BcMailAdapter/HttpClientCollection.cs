using Xunit;

namespace Rsbc.Dmf.BcMailAdapter.Tests
{
    [CollectionDefinition(nameof(HttpClientCollection))]
    public class HttpClientCollection : ICollectionFixture<HttpClientFixture>
    { }
}
