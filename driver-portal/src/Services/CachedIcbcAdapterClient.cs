using Rsbc.Dmf.IcbcAdapter;
using static Rsbc.Dmf.IcbcAdapter.IcbcAdapter;

namespace Rsbc.Dmf.DriverPortal.Api.Services
{
    public interface ICachedIcbcAdapterClient
    {
        Task<DriverInfoReply> GetDriverInfoAsync(DriverInfoRequest request);
    }

    public class CachedIcbcAdapterClient : ICachedIcbcAdapterClient
    {
        private readonly ICacheService _cacheService;
        private readonly IcbcAdapterClient _icbcAdapterClient;

        public CachedIcbcAdapterClient(ICacheService cacheService, IcbcAdapterClient icbcAdapterClient)
        {
            _cacheService = cacheService;
            _icbcAdapterClient = icbcAdapterClient;
        }

        public async Task<DriverInfoReply> GetDriverInfoAsync(DriverInfoRequest request)
        {
            DriverInfoReply reply = null;
            var serviceName = nameof(IcbcAdapterClient.GetDriverInfo);
            if (!_cacheService.TryGetValue(serviceName, request.DriverLicence, out reply))
            {
                reply = await _icbcAdapterClient.GetDriverInfoAsync(request);
                _cacheService.Set(serviceName, request.DriverLicence, reply);
            }

            return reply;
        }
    }
}
