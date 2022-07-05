using Moq;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;

namespace Pssg.Unit.Tests.Interfaces.Icbc.Helpers
{
    public static class IcbcHelper
    {
        public static IIcbcClient CreateMock()
        {
            var icbcClient = new Mock<IIcbcClient>();
            icbcClient
                .Setup(x => x.GetDriverHistory(It.IsAny<string>()))
                .Returns<string>(x => new CLNT() {  DR1MST = new DR1MST() { LNUM = int.Parse(x) } }); // clientResult.DriverMasterStatus.LicenceNumber
            return icbcClient.Object;
        }
    }
}
