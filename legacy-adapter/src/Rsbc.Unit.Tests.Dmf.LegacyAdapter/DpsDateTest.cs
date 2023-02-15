using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Pssg.Interfaces.Icbc.Helpers;
using Pssg.Interfaces.Icbc.Models;
using Pssg.Interfaces;
using Rsbc.Dmf.CaseManagement.Helpers;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.LegacyAdapter;
using System.Net.Http;
using System;
using Xunit;
using System.Net;
using Rsbc.Dmf.LegacyAdapter.ViewModels;

namespace Rsbc.Unit.Tests.Dmf.LegacyAdapter
{
    

    public class DpsDateTest

    {

        

        [Fact]
        public void TestNearMidnight()
        {
            string input = "2023-02-09T0:20:07Z";
            DateTimeOffset dto = DocumentUtils.ParseDpsDate(input);

            Assert.True(dto.Day == 9);
        }


    }
}
