using Rsbc.Dmf.DriverPortal.Tests;
using Xunit;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    [CollectionDefinition(nameof(HttpClientCollection))]
    public class HttpClientCollection : ICollectionFixture<HttpClientFixture> { }
}