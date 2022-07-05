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
                .Returns<string>(x => {
                    int licenceNumber = 0;
                    
                    int.TryParse(x, out licenceNumber );

                    return new CLNT() {  DR1MST = new DR1MST() { LNUM = licenceNumber } };


                    }); // clientResult.DriverMasterStatus.LicenceNumber
            return icbcClient.Object;
        }
    }
}
