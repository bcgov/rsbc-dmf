using Xunit;

namespace Rsbc.Unit.Tests.Dmf.BcMailAdapter
{
    [CollectionDefinition(nameof(HttpClientCollection))]
    public class HttpClientCollection : ICollectionFixture<HttpClientFixture>
    { }
}
