using Microsoft.Extensions.Configuration;
using Rsbc.Dmf.PartnerPortal.Api.ViewModels;
using Xunit;

public class DriverTests : ApiIntegrationTestBase
{
    public DriverTests(IConfiguration configuration) : base(configuration) { }

    [Fact]
    public async Task Get_Driver_History()
    {
        var driverLicenceNumber = _configuration["TEST_ICBC_DL"];
        if (string.IsNullOrEmpty(driverLicenceNumber))
            return;

        var request = new HttpRequestMessage(HttpMethod.Get, $"{DRIVER_API_BASE}/info/{driverLicenceNumber}");
        var clientResult = await HttpClientSendRequest<Document>(request);

        Assert.Equal(driverLicenceNumber, clientResult.Driver.LicenseNumber);
    }
}
