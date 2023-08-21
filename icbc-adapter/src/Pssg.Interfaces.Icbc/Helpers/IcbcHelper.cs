using Microsoft.Extensions.Configuration;
using Moq;
using Pssg.Interfaces;
using Pssg.Interfaces.Icbc.Models;

namespace Pssg.Interfaces.Icbc.Helpers
{
    public static class IcbcHelper
    {
        public static IIcbcClient CreateMock()
        {
            var icbcClient = new Mock<IIcbcClient>();
            icbcClient.Setup(x => x.NormalizeDl(It.IsAny<string>(), It.IsAny <IConfiguration>()))
                .Returns<string,IConfiguration>((x,c) => {  return x; });

            icbcClient
                .Setup(x => x.GetDriverHistory(It.IsAny<string>()))
                .Returns<string>(x => {
                    string licenceNumber = $"{x}";
                    
                    

                    return new CLNT() {  DR1MST = new DR1MST() { LNUM = licenceNumber } };


                    }); // clientResult.DriverMasterStatus.LicenceNumber
            return icbcClient.Object;
        }
    }
}
