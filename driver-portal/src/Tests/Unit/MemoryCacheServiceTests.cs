using Rsbc.Dmf.DriverPortal.Api.Services;
using Xunit;
using static Rsbc.Dmf.IcbcAdapter.IcbcAdapter;

namespace Rsbc.Dmf.DriverPortal.Tests.Unit
{
    public class MemoryCacheServiceTests
    {
        private readonly MemoryCacheService _memoryCacheService;

        public MemoryCacheServiceTests(ICacheService cacheService) 
        {
            _memoryCacheService = (MemoryCacheService)cacheService;
            _memoryCacheService.Dispose();
        }

        [Fact]
        public void TryGetValue() 
        {

        }

        [Fact]
        public void Set()
        {
            //_memoryCacheService.
        }
    }
}
