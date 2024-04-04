using Rsbc.Dmf.DriverPortal.Api;
using Xunit;

namespace Rsbc.Dmf.DriverPortal.Tests.Unit
{
    public class MemoryCacheServiceTests
    {
        private readonly MemoryCacheService _memoryCacheService;

        public MemoryCacheServiceTests(ICacheService cacheService) 
        {
            _memoryCacheService = (MemoryCacheService)cacheService;
        }

        [Fact]
        public void TryGetValue() 
        {
            _memoryCacheService.Set("test1", "key", "value");
            _memoryCacheService.Set("test2", "key", 267);

            _memoryCacheService.TryGetValue("test1", "key", out string value1);
            _memoryCacheService.TryGetValue("test2", "key", out int value2);

            Assert.Equal("value", value1);
            Assert.Equal(267, value2);
        }
    }
}
