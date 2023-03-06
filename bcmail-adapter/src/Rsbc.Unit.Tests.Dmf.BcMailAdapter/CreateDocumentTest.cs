using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace Rsbc.Dmf.BcMailAdapter.Tests
{
    public class CreateDocumentTest
    {

        [Fact]
        public void TestCreateDocument()
        {

            // Arrange

            var body = "PCFET0NUWVBFIGh0bWw+CjxodG1sPgo8Ym9keT4KPHRhYmxlIHN0eWxlPSJoZWlnaHQ6IDkwcHg7IHdpZHRoOiA5OCAlOyBib3JkZXIgLSBjb2xsYXBzZTogY29sbGFwc2U7IGJvcmRlciAtIHN0eWxlOiBub25lOyIgYm9yZGVyPSIwIj4KPHRib2R5Pgo8dHIgc3R5bGU9ImhlaWdodDogMThweDsiPgo8dGQgc3R5bGU9IndpZHRoOiA1MCU7IGhlaWdodDogMThweDsiPkpBUlJFRCBLRVZJTjwvdGQ+CjwvdHI+Cjx0ciBzdHlsZT0iaGVpZ2h0OiAxOHB4OyI+Cjx0ZCBzdHlsZT0id2lkdGg6IDUwJTsgaGVpZ2h0OiAxOHB4OyI+UE8gQk9YIFYyUyAwQTggNjggTUVMQU4gQ1JUPC90ZD4KPC90cj4KPHRyIHN0eWxlPSJoZWlnaHQ6IDE4cHg7Ij4KPHRkIHN0eWxlPSJ3aWR0aDogNTAlOyBoZWlnaHQ6IDE4cHg7Ij5BQkJPVFNGT1JEIEJDICZsdDtQT1NUQUwgQ09ERSZndDs8L3RkPgo8L3RyPgo8dHIgc3R5bGU9ImhlaWdodDogMThweDsiPgo8dGQgc3R5bGU9IndpZHRoOiA1MCU7IGhlaWdodDogMThweDsiPiZuYnNwOzwvdGQ+CjwvdHI+CjwvdGJvZHk+CjwvdGFibGU+CjxwPiZuYnNwOzwvcD4KPHRhYmxlIHN0eWxlPSJoZWlnaHQ6IDM0cHg7IHdpZHRoOiAxMDAlOyBib3JkZXItY29sbGFwc2U6IGNvbGxhcHNlOyBib3JkZXItc3R5bGU6IG5vbmU7IGZsb2F0OiByaWdodDsiIGJvcmRlcj0iMCI+Cjx0Ym9keT4KPHRyIHN0eWxlPSJoZWlnaHQ6IDE4cHg7Ij4KPHRkIHN0eWxlPSJ3aWR0aDogNTAlOyBoZWlnaHQ6IDE4cHg7Ij5EZWFyIEpBUlJFRCBLRVZJTjwvdGQ+Cjx0ZCBzdHlsZT0id2lkdGg6IDUwJTsgaGVpZ2h0OiAxOHB4OyI+RFJJVkVSJ1MgTElDRU5DRSAjMTQ4NjU0OTwvdGQ+CjwvdHI+Cjx0ciBzdHlsZT0iaGVpZ2h0OiAxOHB4OyI+Cjx0ZCBzdHlsZT0id2lkdGg6IDUwJTsgaGVpZ2h0OiAxNnB4OyI+UkU6IDE0ODY1NDktIEtFVklOLSBETUVSLSAxPC90ZD4KPHRkIHN0eWxlPSJ3aWR0aDogNTAlOyBoZWlnaHQ6IDE2cHg7Ij4mbmJzcDs8L3RkPgo8L3RyPgo8L3Rib2R5Pgo8L3RhYmxlPgo8cD4mbmJzcDs8L3A+CjxwPiZuYnNwO2hqa2w7YnZoamtsbmhnY3ZibjwvcD4KPHA+amhnZnhkZ2hqa2w7a2poPC9wPgo8cD5qa2hnZmRnY2h2amtsO2tqaGdmaGo8L3A+CjxwPmpoZ2ZoamtsbWpoZzwvcD4KPHA+bm12Y2doamtsaGdqazwvcD4KPHA+Jm5ic3A7PC9wPgo8cD4mbmJzcDs8L3A+Cjx0YWJsZSBzdHlsZT0iYm9yZGVyLWNvbGxhcHNlOiBjb2xsYXBzZTsgd2lkdGg6IDEwMCU7IiBib3JkZXI9IjAiPgo8dGJvZHk+Cjx0cj4KPHRkIHN0eWxlPSJ3aWR0aDogMTAwJTsiPk1hbm92aWthcyBBbnVwb2p1PC90ZD4KPC90cj4KPHRyPgo8dGQgc3R5bGU9IndpZHRoOiAxMDAlOyI+Q29udHJhY3RvciwgRHJpdmVyIE1lZGljYWwgRml0bmVzcyBQcm9ncmFtPC90ZD4KPC90cj4KPHRyPgo8dGQgc3R5bGU9IndpZHRoOiAxMDAlOyI+Um9hZFNhZmV0eUJDPC90ZD4KPC90cj4KPC90Ym9keT4KPC90YWJsZT4KPC9ib2R5Pgo8L2h0bWw+";
            var header = "PGhlYWRlcj4KICA8aDE+TWFpbiBwYWdlIGhlYWRpbmcgaGVyZTwvaDE+CiAgPHA+UG9zdGVkIGJ5IEpvaG4gRG9lPC9wPgo8L2hlYWRlcj4=";
            var footer = "PGZvb3Rlcj4KICA8cD5UaGlzIGlzIGZvb3RlcjwvcD4KPC9mb290ZXI+";

            byte[] bodyData = Convert.FromBase64String(body);
            string decodedbody = Encoding.UTF8.GetString(bodyData);

            // Decode Header
            byte[] headerData = Convert.FromBase64String(header);
            string decodedHeader = Encoding.UTF8.GetString(headerData);

            // Decode Footer
            byte[] footerData = Convert.FromBase64String(footer);
            string decodedFooter = Encoding.UTF8.GetString(footerData);

            // Act
            var data = DocumentUtils.CreateDocument(decodedbody, decodedHeader, decodedFooter);
            File.WriteAllBytes("test2.docx", data);
            // Assert
           Assert.True(data.Length > 0);
            
        }
    }
}
