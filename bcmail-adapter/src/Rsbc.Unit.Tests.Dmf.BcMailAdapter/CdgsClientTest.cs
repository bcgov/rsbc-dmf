using System;
using System.Net.Http;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Rsbc.Dmf.BcMailAdapter.Tests.Helpers;
using Rsbc.Dmf.BcMailAdapter.ViewModels;
using Rsbc.Interfaces;
using Rsbc.Interfaces.CdgsModels;
using Xunit;

namespace Rsbc.Dmf.BcMailAdapter.Tests
{
    [Collection(nameof(HttpClientCollection))]
    public class CdgsClientTest : ApiIntegrationTestBase
    {
        protected HttpClient _client { get; }  
        protected readonly ICdgsClient _cdgsClient;
        protected string _cdgsServiceUri;
      
        public CdgsClientTest(HttpClientFixture fixture)
            : base(fixture)
        {
            _client = fixture.Client;
            _cdgsServiceUri = Configuration["CDGS_SERVICE_URI"] ?? string.Empty;
            

            if (Configuration["CDGS_SERVICE_URI"] != null)
            {
                _cdgsClient = new CdgsClient(Configuration);
            }
            else
            {
                _cdgsClient = CdgsClientHelper.CreateMock(Configuration);
            }

            if (!string.IsNullOrEmpty(_cdgsServiceUri))
            {
                _client.BaseAddress = new Uri(_cdgsServiceUri);
            }            
        }

        /// <summary>
        /// Test the CDGS Client
        /// </summary>
        [Fact]
        public async void TestCdgsClient()
        {

            string sampleContent = "test";

            byte[] sampleBytes = Encoding.UTF8.GetBytes(sampleContent); 

            var cdgsRequest = new CdgsRequest
            {
                Data = new Data
                {                    
                },
                /* Formatters = "",*/
                Options = new Options
                {
                    ConvertTo = "pdf",
                    Overwrite = true,
                    ReportName = "testCdgs"
                },
                Template = new Template()

                {
                    //Content = attachment.Body ?? string.Empty,
                    // Content = new String(Encoding.UTF8.GetString(docx)),
                    Content = Convert.ToBase64String(sampleBytes),
                    EncodingType = "base64",
                    FileType = "html"

                }
            };
            var responsestream = await _cdgsClient.TemplateRender(cdgsRequest);

            byte[] data = responsestream.ReadAllBytes();

            Assert.True(data.Length > 0);

            //Assert.Equal(bcMail.Attachments[0].Body, Encoding.ASCII.GetBytes(testString));


        }

        /// <summary>
        /// Test the CDGS Client
        /// </summary>
        [Fact]
        public async void TestCdgsClientDocX()
        {

            var body = @"<!DOCTYPE html>
<html>
<body>
<table style=""height: 90px; width: 98 %; border - collapse: collapse; border - style: none;"" border=""0"">
<tbody>
<tr style=""height: 18px;"">
<td style=""width: 50%; height: 18px;"">JARRED KEVIN</td>
</tr>
<tr style=""height: 18px;"">
<td style=""width: 50%; height: 18px;"">PO BOX V2S 0A8 68 MELAN CRT</td>
</tr>
<tr style=""height: 18px;"">
<td style=""width: 50%; height: 18px;"">ABBOTSFORD BC &lt;POSTAL CODE&gt;</td>
</tr>
<tr style=""height: 18px;"">
<td style=""width: 50%; height: 18px;"">&nbsp;</td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<table style=""height: 34px; width: 100%; border-collapse: collapse; border-style: none; float: right;"" border=""0"">
<tbody>
<tr style=""height: 18px;"">
<td style=""width: 50%; height: 18px;"">Dear JARRED KEVIN</td>
<td style=""width: 50%; height: 18px;"">DRIVER'S LICENCE #1486549</td>
</tr>
<tr style=""height: 18px;"">
<td style=""width: 50%; height: 16px;"">RE: 1486549- KEVIN- DMER- 1</td>
<td style=""width: 50%; height: 16px;"">&nbsp;</td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<p>&nbsp;hjkl;bvhjklnhgcvbn</p>
<p>jhgfxdghjkl;kjh</p>
<p>jkhgfdgchvjkl;kjhgfhj</p>
<p>jhgfhjklmjhg</p>
<p>nmvcghjklhgjk</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<table style=""border-collapse: collapse; width: 100%;"" border=""0"">
<tbody>
<tr>
<td style=""width: 100%;"">Manovikas Anupoju</td>
</tr>
<tr>
<td style=""width: 100%;"">Contractor, Driver Medical Fitness Program</td>
</tr>
<tr>
<td style=""width: 100%;"">RoadSafetyBC</td>
</tr>
</tbody>
</table>
</body>
</html>";
            var header = "HEADER";
            var footer = "FOOTER";

            
            body = "TEST";

            // Act
            var sampleBytes = DocumentUtils.CreateDocument(body, header, footer);

            System.IO.File.WriteAllBytes("testTemplate.docx", sampleBytes);

            var cdgsRequest = new CdgsRequest
            {
                Data = new Data
                {
                },
                /* Formatters = "",*/
                Options = new Options
                {
                    CacheReport = false,
                    ConvertTo = "odt",
                    Overwrite = true,
                    //ReportName = "testCdgs.docx"
                },
                Template = new Template()

                {
                    //Content = attachment.Body ?? string.Empty,
                    // Content = new String(Encoding.UTF8.GetString(docx)),
                    Content = Convert.ToBase64String(sampleBytes),
                    EncodingType = "base64",
                    FileType = "docx"
                }
            };
            var responsestream = await _cdgsClient.TemplateRender(cdgsRequest);

            byte[] odtData = responsestream.ReadAllBytes();

            cdgsRequest = new CdgsRequest
            {
                Data = new Data
                {
                },
                /* Formatters = "",*/
                Options = new Options
                {
                    CacheReport = false,
                    ConvertTo = "pdf",
                    Overwrite = true,
                    //ReportName = "testCdgs.docx"
                },
                Template = new Template()

                {
                    //Content = attachment.Body ?? string.Empty,
                    // Content = new String(Encoding.UTF8.GetString(docx)),
                    Content = Convert.ToBase64String(sampleBytes),
                    EncodingType = "base64",
                    FileType = "odt"
                }
            };
            responsestream = await _cdgsClient.TemplateRender(cdgsRequest);

            byte[] pdfData = responsestream.ReadAllBytes();

            System.IO.File.WriteAllBytes("test.pdf",pdfData);

            Assert.True(pdfData.Length > 0);

            //Assert.Equal(bcMail.Attachments[0].Body, Encoding.ASCII.GetBytes(testString));


        }
    }
}
