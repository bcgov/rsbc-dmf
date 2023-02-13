using Rsbc.Interfaces.CdgsModels;
using System.IO;
using System.Threading.Tasks;

namespace Rsbc.Interfaces
{
    public interface ICdgsClient
    {
       /* public CLNT GetDriverHistory(string dlNumber);

        public string SendMedicalUpdate(IcbcMedicalUpdate item);*/

        public Task<Stream> PreviewBcMailDocument(LetterGenerationRequest request);
    }
}
